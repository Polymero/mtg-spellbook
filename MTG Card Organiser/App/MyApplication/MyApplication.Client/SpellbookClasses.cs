using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace MyApplication.Client
{
    public class AppCard
    {
        public string? Thumb { get; set; }
        public string? Image { get; set; }
        public string? OracleId { get; set; }
        public string? SetId { get; set; }
        public string? SetName { get; set; }
        public string? CollectorNumber { get; set; }
        public string? Finish { get; set; }
        public string? Language { get; set; }
        public int Quantity { get; set; }
    }

    public class Snapshot
    {
        public AppCard[]? Commanders { get; set; }
        public AppCard[]? Companions { get; set; }
        public AppCard[]? Mainboard { get; set; }
        public AppCard[]? Sideboard { get; set; }
        public AppCard[]? Maybeboard { get; set; }
        public string? Description { get; set; }
        public string? Date { get; set; }
    }

    [SQLite.Table("Decks")]
    public class AppDeck
    {
        [PrimaryKey, AutoIncrement]
        [SQLite.Column("id")]
        public int Id { get; set; }
        [SQLite.Column("name")]
        public string? Name { get; set; }
        [SQLite.Column("type")]
        public string? Type { get; set; }
        [SQLite.Column("created")]
        public string? Created { get; set; }
        [SQLite.Column("updated")]
        public string? Updated { get; set; }
        [SQLite.Column("snapshots")]
        public Snapshot[]? Snapshots { get; set; }
    }

    [SQLite.Table("Collection")]
    public class AppBinder
    {
        [PrimaryKey, AutoIncrement]
        [SQLite.Column("id")]
        public int Id { get; set; }
        [SQLite.Column("name")]
        public string? Name { get; set; }
        [SQLite.Column("created")]
        public string? Created { get; set; }
        [SQLite.Column("updated")]
        public string? Updated { get; set; }
        [SQLite.Column("description")]
        public string? Description { get; set; }
        [SQLite.Column("binder")]
        public AppCard[]? Binder { get; set; }
    }
}