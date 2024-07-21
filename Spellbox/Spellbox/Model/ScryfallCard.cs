namespace Spellbox.Model
{
    public class ScryfallCard
    {
        public string? Layout { get; set; }
        public string? Lang { get; set; }
        public string? Oracle_Id { get; set; }
        public List<ScryfallCardFace>? Card_Faces { get; set; }
        public decimal? CMC { get; set; }
        public List<string>? Color_Identity { get; set; }
        public List<string>? Colors { get; set; }
        public List<string>? Keywords { get; set; }
        public Legalities? Legalities { get; set; }
        public string? Mana_Cost { get; set; }
        public string? Name { get; set; }
        public string? Oracle_Text { get; set; }
        public string? Power { get; set; }
        public string? Toughness { get; set; }
        public string? Type_Line { get; set; }
        public string? Artist { get; set; }
        public string? Collector_Number { get; set; }
        public List<string>? Finishes { get; set; }
        public string? Flavor_Name { get; set; }
        public string? Flavor_Text { get; set; }
        public List<string>? Games { get; set; }
        public ScryfallCardImageUris? Image_Uris { get; set; }
        public string? Rarity { get; set; }
        public string? Released_At { get; set; }
        public string? Set_Name { get; set; }
        public string? Set { get; set; }
    }

    public class ScryfallCardFace
    {
        public string? Artist { get; set; }
        public decimal? CMC { get; set; }
        public List<string>? Colors { get; set; }
        public string? Flavor_Text { get; set; }
        public ScryfallCardImageUris? Image_Uris { get; set; }
        public string? Mana_Cost { get; set; }
        public string? Name { get; set; }
        public string? Oracle_Id { get; set; }
        public string? Power { get; set; }
        public string? Toughness { get; set; }
        public string? Type_Line { get; set; }
    }

    public class ScryfallCardImageUris
    {
        public string? Small { get; set; }
        public string? Normal { get; set; }
        public string? Large { get; set; }
        public string? PNG { get; set; }
        public string? Art_Crop { get; set; }
        public string? Border_Crop { get; set; }
    }
}