namespace Spellbox.Model
{
    public class ScryfallCardFace
    {
        public string? Artist { get; set; }
        public decimal? CMC { get; set; }
        public List<string>? Colors { get; set; }
        public string? Flavor_Text { get; set; }
        public string? Flavor_Name { get; set; }
        public ScryfallImageUris? Image_Uris { get; set; }
        public string? Mana_Cost { get; set; }
        public string? Name { get; set; }
        public string? Oracle_Id { get; set; }
        public string? Oracle_Text { get; set; }
        public string? Power { get; set; }
        public string? Toughness { get; set; }
        public string? Defense { get; set; }
        public string? Type_Line { get; set; }
    }
}