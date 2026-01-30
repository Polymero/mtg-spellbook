using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class OracleService
    {

        private readonly IDbContextFactory<OracleDbContext> _factory;

        public OracleService(IDbContextFactory<OracleDbContext> factory)
        {
            _factory = factory;
        }

        
        // Oracle search
        public async Task<IEnumerable<CardOracleDto>> OracleSearchFuncAsync(
            string search, 
            CancellationToken ct
        )
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 2)
                return Enumerable.Empty<CardOracleDto>();

            using var db = await _factory.CreateDbContextAsync(ct);

            return await db.Oracles
                .AsNoTracking()
                .Where( c =>
                    EF.Functions.Like(c.Name, $"{search}%") ||
                    EF.Functions.Like(c.Name, $"% {search}%"))
                .OrderBy(c =>
                    EF.Functions.Like(c.Name, $"{search}%") ? 0 :
                    EF.Functions.Like(c.Name, $"% {search}%") ? 1 :
                    2)
                .ThenBy(c => c.Name)
                .Select(c => new CardOracleDto
                {
                    OracleId = c.OracleId,
                    Name = c.Name,
                    TypeLine = c.TypeLine
                }) 
                .Take(20)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<CardVariantDto>> GetVariantsByOracleIdAsync(Guid oracleId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Variants
                .Where(v => v.OracleId == oracleId)
                .OrderByDescending(v => v.Released)
                .ThenBy(v => v.CollNum)
                .Select(v => new CardVariantDto
                {
                    ScryfallId = v.ScryfallId,
                    SetName = v.SetName,
                    SetCode = v.SetCode,
                    CollNum = v.CollNum
                })
                .ToListAsync();
        }



        public async Task<Dictionary<Guid, CardVariantDto>> GetVariantsByIdsAsync(IEnumerable<Guid> variantIds)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Variants
                .Where(v => variantIds.Contains(v.ScryfallId))
                .Select(v => new CardVariantDto
                {
                    ScryfallId = v.ScryfallId,
                    OracleId = v.OracleId,
                    Name = v.SearchName,
                    SetName = v.SetName,
                    SetCode = v.SetCode,
                    CollNum = v.CollNum,
                    Thumbs = v.Thumbs,
                    Images = v.Images,
                    Artist = v.Artist,
                    Rarity = v.Rarity,
                    Released = v.Released,
                    FlavorTexts = v.FlavorTexts,
                    Finishes = v.Finishes
                })
                .ToDictionaryAsync(v => v.ScryfallId);
        }


        public async Task<(CardOracleDto, List<CardFaceDto>)> GetSingleOracleAsync(Guid oracleId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var oracle = await db.Oracles
                .Where(o => o.OracleId == oracleId)
                .Select(o => new CardOracleDto
                {
                    OracleId = o.OracleId,
                    Name = o.Name,
                    TypeLine = o.TypeLine,
                    Keywords = o.Keywords,
                    CMC = o.CMC,
                    ColorIdentity = o.ColorIdentity
                })
                .SingleAsync();

            var faces = await db.Faces
                .Where(f => f.OracleId == oracleId)
                .OrderBy(f => f.Order)
                .Select(f => new CardFaceDto
                {
                    OracleId = f.OracleId,
                    Order = f.Order,
                    Name = f.Name,
                    ManaCost = f.ManaCost,
                    TypeLine = f.TypeLine,
                    OracleText = f.OracleText,
                    Power = f.Power,
                    Toughness = f.Toughness,
                    Defense = f.Defense
                })
                .ToListAsync();

            return (oracle, faces);
        }


        public async Task<CardViewerDto> GetCardInfoByVariantIdAsync(Guid variantId)
        {
            var variant = await GetVariantsByIdsAsync(new List<Guid> { variantId });
            var v = variant[variantId];

            (CardOracleDto oracle, List<CardFaceDto> faces) = await GetSingleOracleAsync(v.OracleId);

            return new CardViewerDto
            {
                OracleId = oracle.OracleId,
                Name = oracle.Name,
                Faces = faces,
                ScryfallId = v.ScryfallId,
                SetCode = v.SetCode,
                CollNum = v.CollNum,
                SetName = v.SetName,
                Artist = v.Artist,
                Released = v.Released,
                Rarity = v.Rarity,
                FlavorTexts = v.FlavorTexts,
                Thumbs = v.Thumbs,
                Images = v.Images,
                Finishes = v.Finishes
            };
        }

        public async Task<CardViewerDto?> GetCardViewerBySetCollAsync(
            string setCode,
            string collNum
        )
        {
            using var db = await _factory.CreateDbContextAsync();

            var variant = await db.Variants
                .SingleOrDefaultAsync(v =>
                    v.SetCode == setCode!.ToLower() &&
                    v.CollNum == collNum);
                
            if (variant is null)
                return null;

            (CardOracleDto oracle, List<CardFaceDto> faces) = await GetSingleOracleAsync(variant.OracleId);

            return new CardViewerDto
            {
                OracleId = oracle.OracleId,
                Name = oracle.Name,
                Faces = faces,
                ScryfallId = variant.ScryfallId,
                SetCode = variant.SetCode,
                CollNum = variant.CollNum,
                SetName = variant.SetName,
                Artist = variant.Artist,
                Released = variant.Released,
                Rarity = variant.Rarity,
                FlavorTexts = variant.FlavorTexts,
                Thumbs = variant.Thumbs,
                Images = variant.Images,
                Finishes = variant.Finishes
            };
        }


        public async Task<List<CardVariantDto>> GetVariantsWithMissingCardMarketIdByIdsAsync(IEnumerable<Guid> variantIds)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Variants
                .Where(v => variantIds.Contains(v.ScryfallId) &&
                       v.CardMarketProductId == null)
                .Select(v => new CardVariantDto
                {
                    ScryfallId = v.ScryfallId,
                    OracleId = v.OracleId,
                    Name = v.SearchName,
                    SetName = v.SetName,
                    SetCode = v.SetCode,
                    CollNum = v.CollNum,
                    Thumbs = v.Thumbs,
                    Images = v.Images,
                    CardMarketProductId = v.CardMarketProductId
                })
                .ToListAsync();
        }

    }

    public sealed class CardViewerDto
    {
        // Oracle
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;

        // Face(s)
        public ICollection<CardFaceDto> Faces { get; init; } = null!;

        // Variant
        public Guid ScryfallId { get; init; }
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public string SetName { get; init; } = null!;
        public List<string> Finishes { get; init; } = null!;
        public string? Artist { get; init; }
        public string Released { get; init; } = null!;
        public string Rarity { get; init; } = null!;
        public List<string> FlavorTexts { get; init; } = null!;
        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
    }

}