

using Microsoft.EntityFrameworkCore;
using Spellbox.Contexts;

namespace Spellbox.Services
{
    public sealed class ScryfallSyncWorker : BackgroundService
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromDays(1);
        private static bool _isRunning;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScryfallSyncWorker> _logger;

        public ScryfallSyncWorker(IServiceScopeFactory scopeFactory, ILogger<ScryfallSyncWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), ct);

            while (!ct.IsCancellationRequested)
            {
                if (_isRunning)
                {
                    _logger.LogInformation("Scryfall sync already in progress. Skipping...");
                    await Task.Delay(TimeSpan.FromMinutes(10), ct);
                    continue;
                }

                _isRunning = true;

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OracleDbContext>>();
                    using var oracleDb = await factory.CreateDbContextAsync();

                    var state = await oracleDb.SyncStates
                        .SingleOrDefaultAsync(x => x.Key == "Scryfall", ct);

                    if (state is null)
                    {
                        state = new ScryfallSyncState();

                        oracleDb.SyncStates.Add(state);
                        await oracleDb.SaveChangesAsync(ct);
                    }

                    var now = DateTime.UtcNow;

                    if (now - state.SyncedAt >= Ttl)
                    {
                        _logger.LogInformation("Card database stale. Refreshing...");

                        var svc = scope.ServiceProvider.GetRequiredService<IScryfallImportService>();

                        await svc.ImportAsync(null, ct);

                        state.SyncedAt = now;
                        await oracleDb.SaveChangesAsync(ct);

                        _logger.LogInformation("Card database refresh completed ~ !");
                    }
                    else
                    {
                        var nextRun = state.SyncedAt + Ttl;
                        _logger.LogInformation("Card database fresh. Next refresh at {Time}", nextRun);
                    }

                    var delay = (state.SyncedAt + Ttl) - now;

                    if (delay < TimeSpan.FromMinutes(30))
                        delay = TimeSpan.FromMinutes(30);

                    await Task.Delay(delay, ct);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Stopping ScryfallSyncWorker...");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Card database refresh failed. Retrying in 60 minutes.");
                    await Task.Delay(TimeSpan.FromMinutes(60), ct);
                }
                finally
                {
                    _isRunning = false;
                }
            }
        }
    }


    public sealed class ScryfallSyncState
    {
        public string Key { get; set; } = "Scryfall";
        public DateTime SyncedAt { get; set; } = DateTime.MinValue;
    }
}