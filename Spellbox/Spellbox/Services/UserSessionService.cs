using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{

    public sealed class UserSession
    {
        public Guid UserProfileId { get; set; }
        public string DisplayName { get; set; } = null!;
        public UserPricingSettings PricingSettings { get; set; } = null!;
        public DateTime LoadedAt { get; set; }
    }


    public interface IUserSessionService
    {
        Task<UserSession> GetAsync();
        Task UpdateDisplayNameAsync(
            string name
        );
        Task UpdatePricingSettingsAsync(
            UserPricingSettings settings
        );
    }

    public sealed class UserSessionService : IUserSessionService
    {
        private readonly IUserProfileService _profile;
        private readonly IUserPricingSettingsService _pricing;

        private UserSession? _session;


        public UserSessionService(IUserProfileService profile, IUserPricingSettingsService pricing)
        {
            _profile = profile;
            _pricing = pricing;
        }


        public async Task<UserSession> GetAsync()
        {
            if (_session != null)
                return _session;

            var profile = await _profile.GetAsync();
            var pricing = await _pricing.GetAsync();

            _session = new UserSession
            {
                UserProfileId = profile.Id,
                DisplayName = profile.DisplayName,
                PricingSettings = pricing,
                LoadedAt = DateTime.UtcNow
            };

            return _session;
        }


        public async Task UpdateDisplayNameAsync(string name)
        {
            await _profile.UpdateDisplayNameAsync(name);

            if (_session != null)
                _session.DisplayName = name;
        }


        public async Task UpdatePricingSettingsAsync(UserPricingSettings settings)
        {
            await _pricing.UpdateAsync(settings);

            if (_session != null)
                _session.PricingSettings = settings;
        }
        
    }

}