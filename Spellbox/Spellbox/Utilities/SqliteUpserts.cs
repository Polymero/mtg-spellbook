using System;
using Microsoft.EntityFrameworkCore;
using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Utilities
{
    public sealed class SqliteUpserts
    {

        public static Task UpsertOracleAsync(
            OracleDbContext db,
            CardOracle oracle,
            CancellationToken ct
        )
        {
            return db.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO Oracles (
    OracleId,
    Name,
    TypeLine,
    Keywords,
    CMC,
    ColorIdentity
)
VALUES (
    {oracle.OracleId},
    {oracle.Name},
    {oracle.TypeLine},
    {oracle.Keywords},
    {oracle.CMC},
    {oracle.ColorIdentity}
)
ON CONFLICT(OracleId)
DO UPDATE SET
    Name = excluded.Name,
    TypeLine = excluded.TypeLine,
    Keywords = excluded.Keywords,
    CMC = excluded.CMC,
    ColorIdentity = excluded.ColorIdentity
", ct);
        }


        public static Task UpsertVariantAsync(
            OracleDbContext db,
            CardVariant variant,
            CancellationToken ct
        )
        {
            return db.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO Variants (
    ScryfallId,
    OracleId,
    SearchName,
    SetName,
    SetCode,
    CollNum,
    Finishes,
    Artist,
    Released,
    Rarity,
    FlavorTexts,
    Thumbs,
    Images,
    CardMarketProductId
)
VALUES (
    {variant.ScryfallId},
    {variant.OracleId},
    {variant.SearchName},
    {variant.SetName},
    {variant.SetCode},
    {variant.CollNum},
    {variant.Finishes},
    {variant.Artist},
    {variant.Released},
    {variant.Rarity},
    {variant.FlavorTexts},
    {variant.Thumbs},
    {variant.Images},
    {variant.CardMarketProductId}
)
ON CONFLICT(ScryfallId)
DO UPDATE SET
    OracleId = excluded.OracleId,
    SearchName = excluded.SearchName,
    SetName = excluded.SetName,
    SetCode = excluded.SetCode,
    CollNum = excluded.CollNum,
    Finishes = excluded.Finishes,
    Artist = excluded.Artist,
    Released = excluded.Released,
    Rarity = excluded.Rarity,
    FlavorTexts = excluded.FlavorTexts,
    Thumbs = excluded.Thumbs,
    Images = excluded.Images,
    CardMarketProductId =
        COALESCE(excluded.CardMarketProductId, Variants.CardMarketProductId)
", ct);
        }


        public static Task UpsertFaceAsync(
            OracleDbContext db,
            CardFace f,
            CancellationToken ct = default
        )
        {
            return db.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO Faces (
    Id,
    OracleId,
    [Order],
    Name,
    ManaCost,
    TypeLine,
    OracleText,
    Power,
    Toughness,
    Defense,
    Loyalty
)
VALUES (
    {f.Id},
    {f.OracleId},
    {f.Order},
    {f.Name},
    {f.ManaCost},
    {f.TypeLine},
    {f.OracleText},
    {f.Power},
    {f.Toughness},
    {f.Defense},
    {f.Loyalty}
)
ON CONFLICT(OracleId, [Order])
DO UPDATE SET
    Name = excluded.Name,
    ManaCost = excluded.ManaCost,
    TypeLine = excluded.TypeLine,
    OracleText = excluded.OracleText,
    Power = excluded.Power,
    Toughness = excluded.Toughness,
    Defense = excluded.Defense,
    Loyalty = excluded.Loyalty
", ct);
        }

    }
}
