using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


namespace Spellbox.Model
{

    public class Symbol
    {
        [Key]
        public Guid Id { get; set; }

        public string Code { get; set; } = null!;
        public string? SvgData { get; set; }
        public string? Tip { get; set; }
    }

    public class SymbolDto
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = null!;
        public string? SvgData { get; init; }
        public string? Tip { get; init; }

        public static Expression<Func<Symbol, SymbolDto>> FromEntity => e
            => new()
            {
                Id = e.Id,
                Code = e.Code,
                SvgData = e.SvgData,
                Tip = e.Tip
            };
    }

}