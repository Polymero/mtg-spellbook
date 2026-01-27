using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetAsync();
        Task UpdateDisplayNameAsync(string name);
    }


    public sealed class UserProfileService : IUserProfileService
    {

        private readonly IDbContextFactory<CollectionDbContext> _factory;

        public UserProfileService(IDbContextFactory<CollectionDbContext> factory)
        {
            _factory = factory;
        }


        public async Task<UserProfile> GetAsync()
        {
            using var db = await _factory.CreateDbContextAsync();

            var profile = await db.UserProfiles.SingleOrDefaultAsync();

            if (profile != null)
                return profile;

            profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                DisplayName = "Spellbox.User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.UserProfiles.Add(profile);
            await db.SaveChangesAsync();

            return profile;
        }


        public async Task UpdateDisplayNameAsync(string name)
        {
            using var db = await _factory.CreateDbContextAsync();

            var profile = await db.UserProfiles.SingleAsync();

            profile.DisplayName = name.Trim();
            profile.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }

    }
}