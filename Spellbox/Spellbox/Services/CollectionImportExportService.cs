using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{

    public class ImportResult
    {
        public List<ImportAllocation> Cards { get; init; } = [];
        public List<string> Errors { get; init; } = [];
        public bool Success => Errors.Count == 0;

        // public int Quantity => Cards
        //     .Sum(c => c.Quantity);
        // public int VariantCount => Cards.Count;
        // public int OracleCount => Cards
        //     .Select(c => c.OracleId)
        //     .Distinct()
        //     .Count();
    }

    public class ImportAllocation
    {
        public string? Name { get; init; }
        public Guid? ScryfallId { get; init; }
        public string? SetCode { get; init; }
        public string? CollNum { get; init; }
        public int Quantity { get; init; } = 1;
        public CardFinish Finish { get; init; } = CardFinish.Unknown;
        public CardCondition Condition { get; init; } = CardCondition.Unknown;
        public CardLanguage Language { get; init; } = CardLanguage.Unknown;
        public bool IsSigned { get; init; } = false;
        public bool IsAltered { get; init; } = false;
        public bool IsStamped { get; init; } = false;
        public bool IsMisprint { get; init; } = false;
        public decimal? BoughtFor { get; init; }
    }

    public sealed class ImportAllocationMap : ClassMap<ImportAllocation>
    {
        public ImportAllocationMap()
        {
            Map(a => a.Name)
                .Name("Name", "Card Name", "Card name", "card_name")
                .Optional();
            Map(a => a.ScryfallId)
                .Name("ScryfallId", "Scryfall ID", "scryfall_id")
                .Optional();
            Map(a => a.SetCode)
                .Name("SetCode", "Set Code", "Set code", "set_code")
                .Optional();
            Map(a => a.CollNum)
                .Name("CollNum", "Collector number", "collector_number")
                .Optional();
            Map(a => a.Quantity)
                .Name("Quantity", "Count", "Qty", "quantity", "number")
                .Optional();
            Map(a => a.Finish)
                .Name("Finish", "Foil", "finish", "Card Finish", "Foiling")
                .TypeConverter<CardFinishConverter>()
                .Optional();
            Map(a => a.Condition)
                .Name("Condition", "Grade", "condition")
                .TypeConverter<CardConditionConverter>()
                .Optional();
            Map(a => a.Language)
                .Name("Language", "lang")
                .TypeConverter<CardLanguageConverter>()
                .Optional();
            Map(a => a.IsSigned)
                .Name("IsSigned")
                .Optional();
            Map(a => a.IsAltered)
                .Name("IsAltered", "Altered")
                .Optional();
            Map(a => a.IsStamped)
                .Name("IsStamped")
                .Optional();
            Map(a => a.IsMisprint)
                .Name("IsMisprint", "Misprint")
                .Optional();
            Map(a => a.BoughtFor)
                .Name("BoughtFor", "Purchase price")
                .Optional();
        }
    }


    public sealed class CollectionImportService
    {
        private readonly IDbContextFactory<OracleDbContext> _oracle;

        public CollectionImportService(IDbContextFactory<OracleDbContext> oracle)
        {
            _oracle = oracle;
        }


        public async Task<ImportResult> ParseStreamAsync(
            Stream stream,
            CancellationToken ct = default
        )
        {
            var cards = new List<ImportAllocation>();
            var errors = new List<string>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = ctx => errors.Add($"Row {ctx.Context.Parser?.Row}: BadDataFound in {ctx.RawRecord.Trim()}"),
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true
            };

            try
            {
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, config);

                csv.Context.RegisterClassMap<ImportAllocationMap>();

                await foreach (var record in csv.GetRecordsAsync<ImportAllocation>(ct))
                {
                    // Guid? variantId = null;

                    // if (record.ScryfallId.HasValue)
                    //     variantId = record.ScryfallId.Value;
                    // else
                    cards.Add(record);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"FATAL PARSE ERROR: {ex.Message}");
            }

            return new ImportResult
            {
                Cards = cards,
                Errors = errors
            };
        }

    }


    public partial class CollectionExportService
    {
        


    }


    public class CardFinishConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return text?.Trim().ToLowerInvariant().Replace("-", "").Replace("_", "").Replace(" ", "") switch
            {
                "normal" or "nonfoil" or "nf" => CardFinish.NonFoil,
                "foil" or "f" => CardFinish.Foil,
                "etched" or "etchedfoil" => CardFinish.EtchedFoil,
                "galaxy" or "galaxyfoil" => CardFinish.GalaxyFoil,
                "textured" or "texturedfoil" => CardFinish.TexturedFoil,
                "surge" or "surgefoil" => CardFinish.SurgeFoil,
                "neon" or "neonfoil" or "neonink" or "neoninkfoil" => CardFinish.NeonInkFoil,
                _ => CardFinish.Unknown
            };
        }

        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return value is CardFinish f ? f.ToString() : CardFinish.Unknown.ToString();
        }
    }

    public class CardConditionConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return text?.Trim().ToLowerInvariant().Replace("-", "").Replace("_", "").Replace(" ", "") switch
            {
                "mint" or "mt" => CardCondition.Mint,
                "nearmint" or "nm" => CardCondition.NearMint,
                "excellent" or "ex" => CardCondition.Excellent,
                "good" or "gd" => CardCondition.Good,
                "lightplayed" or "lp" => CardCondition.LightPlayed,
                "played" or "pl" => CardCondition.Played,
                "poor" or "po" => CardCondition.Poor,
                _ => CardCondition.Unknown
            };
        }

        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return value is CardCondition c ? c.ToString() : CardCondition.Unknown.ToString();
        }
    }

    public class CardLanguageConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return text?.Trim().ToLowerInvariant().Replace("-", "").Replace("_", "").Replace(" ", "") switch
            {
                "english" or "en" => CardLanguage.English,
                "japanese" or "jp" => CardLanguage.Japanese,
                "german" or "de" => CardLanguage.German,
                "french" or "fr" => CardLanguage.French,
                "italian" or "it" => CardLanguage.Italian,
                "spanish" or "sp" => CardLanguage.Spanish,
                "portuguese" or "pt" => CardLanguage.Portuguese,
                "russian" or "ru" => CardLanguage.Russian,
                "korean" or "ko" => CardLanguage.Korean,
                "chinesesimplified" or "simplifiedchinese" or "zhcn" or "zhhans" => CardLanguage.ChineseSimplified,
                "chinesetraditional" or "traditionalchinese" or "zhhant" or "zhtw" or "zhhk" => CardLanguage.ChineseTraditional,
                "phyrexian" or "ph" => CardLanguage.Phyrexian,
                _ => CardLanguage.Unknown
            };
        }

        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return value is CardLanguage l ? l.ToString() : CardLanguage.Unknown.ToString();
        }
    }
}