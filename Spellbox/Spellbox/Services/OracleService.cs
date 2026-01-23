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


        public async Task<Dictionary<Guid, CVariantDto>> GetVariantsByIdsAsync(IEnumerable<Guid> variantIds)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.CardVariants
                .Where(v => variantIds.Contains(v.ScryfallId))
                .Select(v => new CVariantDto
                {
                    ScryfallId = v.ScryfallId,
                    OracleCardId = v.OracleCardId,
                    Name = v.SearchName,
                    SetName = v.SetName,
                    SetCode = v.SetCode,
                    CollNum = v.CollNum,
                    Thumbs = v.Thumbs,
                    Images = v.Images,
                    Artist = v.Artist,
                    Rarity = v.Rarity,
                    Released = v.Released,
                    FlavorTexts = v.FlavorTexts
                })
                .ToDictionaryAsync(v => v.ScryfallId);
        }


        public async Task<(OracleDto, List<CFaceDto>)> GetSingleOracleAsync(Guid oracleId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var oracle = await db.OracleCards
                .Where(o => o.OracleId == oracleId)
                .Select(o => new OracleDto
                {
                    OracleId = o.OracleId,
                    Name = o.Name,
                    TypeLine = o.TypeLine,
                    Keywords = o.Keywords,
                    CMC = o.CMC,
                    ColorIdentity = o.ColorIdentity
                })
                .SingleAsync();

            var faces = await db.CardFaces
                .Where(f => f.OracleId == oracleId)
                .OrderBy(f => f.Order)
                .Select(f => new CFaceDto
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

            (OracleDto oracle, List<CFaceDto> faces) = await GetSingleOracleAsync(v.OracleCardId);

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
            };
        }

    }

    public sealed class CardViewerDto
    {
        // Oracle
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;

        // Face(s)
        public ICollection<CFaceDto> Faces { get; init; } = null!;

        // Variant
        public Guid ScryfallId { get; init; }
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public string SetName { get; init; } = null!;
        public string? Artist { get; init; }
        public string Released { get; init; } = null!;
        public string Rarity { get; init; } = null!;
        public List<string> FlavorTexts { get; init; } = null!;
        public List<string> Thumbs { get; init; } = null!;
        public List<string> Images { get; init; } = null!;
    }

}