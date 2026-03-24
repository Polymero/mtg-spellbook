using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Spellbox.Contexts;
using Spellbox.Model;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;


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
                if (_symbolMap.Count > 0)
                    await ClearSymbols(ct);

                await DownloadSymbolsFromScryfall(ct);

                _symbolMap = await GetSymbolsFromDatabase(ct);
            }
            
            _logger.LogInformation("Symbology initialised {Count} symbols.", _symbolMap.Count);
        }

        public MarkupString RenderSymbolText(string? text)
        {
            if (String.IsNullOrEmpty(text))
                return new MarkupString(String.Empty);

            var rendered = SymbolRegex().Replace(text, match =>
            {
                var code = match.Value;
                return _symbolMap.TryGetValue(code, out var symbol)
                    ? $"""<span class="mtg-symbol-wrapper">{symbol.SvgData}</span>"""
                    : code;
            });

            return new MarkupString(rendered);
        }

        public string RenderSymbolIcon(string? text)
        {
            if (String.IsNullOrEmpty(text))
                return "";

            if (!text.StartsWith('{'))
                text = '{' + text;

            if (!text.EndsWith('}'))
                text += '}';

            if (!SymbolRegex().IsMatch(text))
                return "";

            return _symbolMap.TryGetValue(text, out var symbol)
                ? symbol.SvgData ?? ""
                : "";
        }


        private async Task ClearSymbols(
            CancellationToken ct = default
        )
        {
            using var db = await _oracle.CreateDbContextAsync(ct);

            db.Symbols.RemoveRange(await db.Symbols.AsTracking().ToListAsync(ct));

            await db.SaveChangesAsync(ct);
        }

        private async Task<Dictionary<string, SymbolDto>> GetSymbolsFromDatabase(
            CancellationToken ct = default
        )
        {
            using var db = await _oracle.CreateDbContextAsync(ct);

            return await db.Symbols
                .Where(s => !String.IsNullOrWhiteSpace(s.SvgData))
                .Select(SymbolDto.FromEntity)
                .ToDictionaryAsync(s => s.Code, ct);
        }

        private async Task DownloadSymbolsFromScryfall(
            CancellationToken ct = default
        )
        {
            using var db = await _oracle.CreateDbContextAsync(ct);

            var client = _http.CreateClient("Scryfall");

            _logger.LogInformation("Fetching symbols from Scryfall...");

            var symbols = new List<Symbol>();

            await foreach (var symbol in StreamScryfallSymbolsAsync(SymbologyUrl, ct))
            {
                ct.ThrowIfCancellationRequested();

                // if (symbol.TryGetProperty("funny", out var funny) &&
                //     !funny.GetBoolean())
                //     continue;

                var code = symbol.GetProperty("symbol").GetString()!;

                symbols.Add(new Symbol
                {
                    Id = Guid.NewGuid(),
                    Code = code,
                    SvgData = symbol.TryGetProperty("svg_uri", out var svgUri)
                        ? CleanSvg(code, await client.GetStringAsync(svgUri.GetString(), ct))
                        : null,
                    Tip = symbol.TryGetProperty("english", out var tip)
                        ? tip.GetString()
                        : null
                });

                await Task.Delay(RequestDelayMs, ct);
            }

            _logger.LogInformation("Fetched {Count} symbols from Scryfall...", symbols.Count);

            db.Symbols.AddRange(symbols);

            await db.SaveChangesAsync(ct);
        }

        private async IAsyncEnumerable<JsonElement> StreamScryfallSymbolsAsync(
            string url,
            [EnumeratorCancellation] CancellationToken ct
        )
        {
            var client = _http.CreateClient("Scryfall");
            await using var stream = await client.GetStreamAsync(SymbologyUrl, ct);
            using var jdoc = await JsonDocument.ParseAsync(stream, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            }, ct);

            var data = jdoc.RootElement
                .GetProperty("data");

            foreach (var element in data.EnumerateArray())
            {
                ct.ThrowIfCancellationRequested();
                yield return element;
            }
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