

using Microsoft.EntityFrameworkCore;
using Spellbox.Contexts;

namespace Spellbox.Services
{
    public sealed class CardMarketSyncWorker : BackgroundService
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromHours(12);
        private static bool _isRunning;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CardMarketSyncWorker> _logger;

        public CardMarketSyncWorker(IServiceScopeFactory scopeFactory, ILogger<CardMarketSyncWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), ct);

            while (!ct.IsCancellationRequested)
            {
                if (_isRunning)
                {
                    _logger.LogInformation("Price sync already in progress. Skipping...");
                    await Task.Delay(TimeSpan.FromMinutes(10), ct);
                    continue;
                }

                _isRunning = true;

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CardMarketDbContext>>();
                    
                    using var pricingDb = await factory.CreateDbContextAsync();
                    await pricingDb.Database.MigrateAsync(ct);

                    var state = await pricingDb.SyncStates
                        .SingleOrDefaultAsync(x => x.Key == "PriceGuide", ct);

                    if (state == null)
                    {
                        state = new PricingSyncState
                        {
                            Key = "PriceGuide",
                            SyncedAt = DateTime.MinValue
                        };

                        pricingDb.SyncStates.Add(state);
                        await pricingDb.SaveChangesAsync(ct);
                    }

                    var now = DateTime.UtcNow;

                    if (now - state.SyncedAt >= Ttl)
                    {
                        _logger.LogInformation("Price guide stale. Refreshing...");

                        var svc = scope.ServiceProvider.GetRequiredService<CardMarketPriceGuideService>();

                        await svc.RefreshAsync(ct);

                        state.SyncedAt = now;
                        await pricingDb.SaveChangesAsync(ct);

                        _logger.LogInformation("Price guide refresh completed ~ !");
                    }
                    else
                    {
                        var nextRun = state.SyncedAt + Ttl;
                        _logger.LogInformation("Price guide fresh. Next refresh at {Time}", nextRun);
                    }

                    var delay = (state.SyncedAt + Ttl) - now;

                    if (delay < TimeSpan.FromMinutes(30))
                        delay = TimeSpan.FromMinutes(30);

                    await Task.Delay(delay, ct);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Stopping CardMarketSyncWorker...");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Price guide refresh failed. Retrying in 60 minutes.");
                    await Task.Delay(TimeSpan.FromMinutes(60), ct);
                }
                finally
                {
                    _isRunning = false;
                }
            }
        }
    }


    public sealed class PricingSyncState
    {
        public string Key { get; set; } = "PriceGuide";
        public DateTime SyncedAt { get; set; } = DateTime.MinValue;
    }
}