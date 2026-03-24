using Microsoft.EntityFrameworkCore;
using MudBlazor;
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
                .Select(CardOracleDto.FromEntity) 
                .Take(12)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<CardVariantDto>> GetVariantsByOracleIdAsync(Guid oracleId)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Variants
                .Where(v => v.OracleId == oracleId)
                .OrderByDescending(v => v.Released)
                .ThenBy(v => v.SetCode.Length)
                .ThenBy(v => v.SetCode)
                .ThenBy(v => v.CollNum.Length)
                .ThenBy(v => v.CollNum)
                .Select(CardVariantDto.FromEntity)
                .ToListAsync();
        }


        public async Task<Dictionary<Guid, CardOracleDto>> GetOraclesByIdsAsync(IEnumerable<Guid> oracleIds)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Oracles
                .Where(o => oracleIds.Contains(o.OracleId))    
                .Select(CardOracleDto.FromEntity)
                .ToDictionaryAsync(o => o.OracleId);
        }

        public async Task<Dictionary<Guid, CardVariantDto>> GetVariantsByIdsAsync(IEnumerable<Guid> variantIds)
        {
            using var db = await _factory.CreateDbContextAsync();

            return await db.Variants
                .Where(v => variantIds.Contains(v.ScryfallId))
                .Select(CardVariantDto.FromEntity)
                .ToDictionaryAsync(v => v.ScryfallId);
        }

        public async Task<Dictionary<Tuple<string, string>, CardVariantDto>> GetVariantsBySetCollAsync(
            IEnumerable<Tuple<string, string>> setColls
        )
        {
            using var db = await _factory.CreateDbContextAsync();

            var targets = from pair in setColls select pair.Item1 + pair.Item2;

            return await db.Variants
                .Where(v => targets.Contains(v.SetCode + v.CollNum))
                .Select(CardVariantDto.FromEntity)
                .ToDictionaryAsync(v => Tuple.Create(v.SetCode, v.CollNum));
        }

        public async Task<(CardOracleDto?, CardVariantDto?)> GetOracleVariantBySetCollAsync(
            string setCode,
            string collNum
        )
        {
            using var db = await _factory.CreateDbContextAsync();

            var variantQuery = db.Variants
                .Where(v => v.SetCode == setCode.ToLower() && v.CollNum == collNum.ToLower())
                .AsQueryable();

            if (!await variantQuery.AnyAsync())
                return (null, null);

            var variant = await variantQuery
                .Select(CardVariantDto.FromEntity)
                .SingleOrDefaultAsync();

            var oracle = await variantQuery
                .Select(v => v.Oracle)
                .Select(CardOracleDto.FromEntity)
                .SingleOrDefaultAsync();

            return (oracle, variant);
        }


        public async Task<(CardOracleDto, List<CardFaceDto>)> GetSingleOracleAsync(Guid oracleId)
        {
            using var db = await _factory.CreateDbContextAsync();

            var oracle = await db.Oracles
                .Where(o => o.OracleId == oracleId)
                .Select(CardOracleDto.FromEntity)
                .SingleAsync();

            var faces = await db.Faces
                .Where(f => f.OracleId == oracleId)
                .OrderBy(f => f.Order)
                .Select(CardFaceDto.FromEntity)
                .ToListAsync();

            return (oracle, faces);
        }


        public async Task<CardViewerDto> GetCardInfoByVariantIdAsync(Guid variantId)
        {
            var variant = await GetVariantsByIdsAsync([variantId]);
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
                .Where(v =>
                    v.SetCode.ToLower() == setCode.ToLower() &&
                    v.CollNum == collNum)
                .Select(CardVariantDto.FromEntity)
                .SingleOrDefaultAsync();
                
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
                Images = variant.Images,
                Finishes = variant.Finishes
            };
        }


        public async Task<CardColours> GetColorIdentityAsync(
            IEnumerable<Guid> oracleIds
        )
        {
            using var db = await _factory.CreateDbContextAsync();

            var colorIdentities = await db.Oracles
                .Where(o => oracleIds.Contains(o.OracleId))
                .Select(o => o.ColorIdentity)
                .Distinct()
                .ToListAsync();

            var value = 0;

            foreach (var colors in colorIdentities)
                value |= colors;

            return CardColours.FromInt(value);
        }

    }

    public sealed class CardViewerDto
    {
        // Oracle
        public Guid OracleId { get; init; }
        public string Name { get; init; } = null!;

        // Face(s)
        public ICollection<CardFaceDto> Faces { get; init; } = [];

        // Variant
        public Guid ScryfallId { get; init; }
        public string SetCode { get; init; } = null!;
        public string CollNum { get; init; } = null!;
        public string SetName { get; init; } = null!;
        public List<string> Finishes { get; init; } = [];
        public string? Artist { get; init; }
        public string Released { get; init; } = null!;
        public string Rarity { get; init; } = null!;
        public List<string> FlavorTexts { get; init; } = [];
        public CardImage Images { get; init; } = null!;
    }

}