namespace mtgSpellbook.Client.Data
{
    public class Binder
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Created { get; set; }
        public string? Updated { get; set; }
        public string? Description { get; set; }
        public Card[]? Cards { get; set; }
    }
}