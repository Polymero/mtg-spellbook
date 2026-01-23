using System.ComponentModel.DataAnnotations;

namespace Spellbox.Model
{
    public sealed class UserProfile
    {
        [Key]
        public Guid Id { get; set; }

        public string DisplayName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    public sealed class UserSettings
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; } = null!;

        // user preferences etc.
    }
}