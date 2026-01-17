using Microsoft.EntityFrameworkCore;

namespace Spellbox.Model
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
                    SetCode = v.SetCode,
                    CollNum = v.CollNum,
                    Thumbs = v.Thumbs,
                    Images = v.Images
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

    }
}