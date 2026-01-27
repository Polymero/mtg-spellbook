using Microsoft.EntityFrameworkCore;
using Spellbox.Components.Pages;
using Spellbox.Contexts;
using Spellbox.Model;


namespace Spellbox.Services
{

    public interface IUserPricingSettingsService
    {
        Task<UserPricingSettings> GetAsync();
        Task UpdateAsync(UserPricingSettings settings);
    }


    public sealed class UserPricingSettingsService : IUserPricingSettingsService
    {
        private readonly IDbContextFactory<CollectionDbContext> _factory;

        public UserPricingSettingsService(IDbContextFactory<CollectionDbContext> factory)
        {
            _factory = factory;
        }


        public async Task<UserPricingSettings> GetAsync()
        {
            using var db = await _factory.CreateDbContextAsync();

            var settings = await db.UserPricingSettings.SingleOrDefaultAsync();

            if (settings != null)
                return settings;

            settings = new UserPricingSettings
            {
                Id = Guid.NewGuid(),
                NonFoilMetric = PriceMetric.Trend,
                FoilMetric = PriceMetric.Trend,
                UpdatedAt = DateTime.UtcNow
            };

            db.UserPricingSettings.Add(settings);
            await db.SaveChangesAsync();

            return settings;
        }


        public async Task UpdateAsync(UserPricingSettings settings)
        {
            using var db = await _factory.CreateDbContextAsync();

            var data = await db.UserPricingSettings.SingleAsync();

            data.Marketplace = settings.Marketplace;
            data.NonFoilMetric = settings.NonFoilMetric;
            data.FoilMetric = settings.FoilMetric;
            data.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }
    }


}