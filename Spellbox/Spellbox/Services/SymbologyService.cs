using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public partial class SymbologyService
    {
        private const string SymbologyUrl = "https://api.scryfall.com/symbology";
        private const int RequestDelayMs = 50;
        private IReadOnlyDictionary<string, SymbolDto> _symbolMap = new Dictionary<string, SymbolDto>();

        [GeneratedRegex(@"\{[^}]+\}")]
        private static partial Regex SymbolRegex();
        [GeneratedRegex(@"<\?xml[^?]*\?>")]
        private static partial Regex XmlDeclRegex();
        [GeneratedRegex(@"\s\s*(width|height)=""[^""]*""")]
        private static partial Regex SvgSizeRegex();

        private readonly IHttpClientFactory _http;
        private readonly IDbContextFactory<OracleDbContext> _oracle;
        private readonly ILogger<SymbologyService> _logger;

        public SymbologyService(
            IHttpClientFactory http, 
            IDbContextFactory<OracleDbContext> oracle,
            ILogger<SymbologyService> logger
        )
        {
            _http = http;
            _oracle = oracle;
            _logger = logger;
        }


        public async Task InitialiseAsync(
            bool forceRefresh = false,
            CancellationToken ct = default
        )
        {
            _symbolMap = await GetSymbolsFromDatabase(ct);
            
            if (forceRefresh || _symbolMap.Count == 0)
            {
                await DownloadSymbolsFromScryfall(ct);

                _symbolMap = await GetSymbolsFromDatabase(ct);
            }
            
            _logger.LogInformation("Symbology initialised {Count} symbols.", _symbolMap.Count);
        }

        

        private async Task<Dictionary<string, SymbolDto>> GetSymbolsFromDatabase(
            CancellationToken ct = default
        )
        {
            using var db = await _oracle.CreateDbContextAsync(ct);

            return await db.Symbols
                .Where(s => !String.IsNullOrWhiteSpace(s.SvgData))
                .Select(s => new SymbolDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    SvgData = s.SvgData,
                    Tip = s.Tip
                })
                .ToDictionaryAsync(s => s.Code, ct);
        }

        private async Task DownloadSymbolsFromScryfall(
            CancellationToken ct = default
        )
        {
            using var db = await _oracle.CreateDbContextAsync(ct);

            _logger.LogInformation("Fetching symbols from Scryfall...");

            var client = _http.CreateClient("Scryfall");
            await using var stream = await client.GetStreamAsync(SymbologyUrl, ct);
            using var jdoc = await JsonDocument.ParseAsync(stream, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            }, ct);

            var symbols = new List<Symbol>();

            foreach (var element in jdoc.RootElement.EnumerateArray())
            {
                ct.ThrowIfCancellationRequested();

                if (element.TryGetProperty("funny", out var funny) &&
                    !funny.GetBoolean())
                    continue;

                var code = element.GetProperty("symbol").GetString()!;

                symbols.Add(new Symbol
                {
                    Id = Guid.NewGuid(),
                    Code = code,
                    SvgData = element.TryGetProperty("svg_uri", out var svgUri)
                        ? CleanSvg(code, await client.GetStringAsync(svgUri.GetString(), ct))
                        : null,
                    Tip = element.TryGetProperty("english", out var tip)
                        ? tip.GetString()
                        : null
                });

                await Task.Delay(RequestDelayMs, ct);
            }

            db.Symbols.AddRange(symbols);

            await db.SaveChangesAsync(ct);
        }

        private static string CleanSvg(string symbol, string raw)
        {
            var svg = XmlDeclRegex().Replace(raw, "").Trim();

            svg = SvgSizeRegex().Replace(svg, "");

            svg = svg.Replace("<svg ", $"""<svg class="mtg-symbol" role="img" aria-label="{symbol}" """);

            return svg;
        }
    }

}