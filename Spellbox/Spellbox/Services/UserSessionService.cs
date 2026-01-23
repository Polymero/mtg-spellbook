using Microsoft.EntityFrameworkCore;

using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{
    public sealed class UserSessionService
    {
        private UserProfile? _profile;

        private readonly IUserProfileService _service;


        public UserSessionService(IUserProfileService service)
        {
            _service = service;
        }


        public async Task<UserProfile> GetProfileAsync()
        {
            if (_profile != null)
                return _profile;

            _profile = await _service.GetOrCreateAsync();
            return _profile;
        }


        public async Task UpdateDisplayNameAsync(string name)
        {
            await _service.UpdateDisplayNameAsync(name);

            if (_profile != null)
                _profile.DisplayName = name;
        }
    }
}