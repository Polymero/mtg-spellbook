using Spellbox.Model;


namespace Spellbox.Services
{

    public interface IPricingRouter
    {
        Task<(decimal?, decimal?)?> GetPriceByPriceIdAsync(
            int priceId
        );

        Task<decimal?> GetPriceAsync(
            Guid variantId,
            CardFinish finish,
            CardLanguage language = CardLanguage.English,
            CardCondition condition = CardCondition.NearMint
        );

        Task<Dictionary<Guid, decimal?>> GetPriceBatchAsync(
            IEnumerable<CollectionAllocationDto> allocationDtos
        );

        Task<(decimal, int)> GetUnassignedValueAsync();

        Task<(decimal, int)> GetBinderValueAsync(
            Guid binderId
        );

        Task<(decimal, int)> GetDeckValueAsync(
            Guid deckId
        );

        Task<List<PricingEditDto>> GetPricingEditsAsync();
    }


    public sealed class PricingRouter : IPricingRouter
    {
        private readonly IUserSessionService _session;
        private readonly IEnumerable<IPricingService> _services;

        public PricingRouter(IUserSessionService session, IEnumerable<IPricingService> services)
        {
            _session = session;
            _services = services;
        }


        public async Task<(decimal?, decimal?)?> GetPriceByPriceIdAsync(
            int priceId
        )
        {
            var prefs = (await _session.GetAsync()).PricingSettings;

            var service = _services
                .Single(s => s.Marketplace == prefs.Marketplace);

            return await service.GetPriceByPriceIdAsync(
                priceId,
                prefs.NonFoilMetric,
                prefs.FoilMetric
            );
        }


        public async Task<decimal?> GetPriceAsync(
            Guid variantId,
            CardFinish finish,
            CardLanguage language = CardLanguage.English,
            CardCondition condition = CardCondition.NearMint
        )
        {
            var prefs = (await _session.GetAsync()).PricingSettings;

            var service = _services
                .Single(s => s.Marketplace == prefs.Marketplace);

            return await service.GetPriceAsync(
                variantId,
                finish,
                language,
                condition,
                prefs.NonFoilMetric,
                prefs.FoilMetric
            );
        }

        public async Task<Dictionary<Guid, decimal?>> GetPriceBatchAsync(
            IEnumerable<CollectionAllocationDto> allocationDtos
        )
        {
            var prefs = (await _session.GetAsync()).PricingSettings;

            var service = _services
                .Single(s => s.Marketplace == prefs.Marketplace);

            return await service.GetPriceBatchAsync(
                allocationDtos,
                prefs.NonFoilMetric,
                prefs.FoilMetric
            );
        }


        public async Task<(decimal, int)> GetUnassignedValueAsync()
        {
            var prefs = (await _session.GetAsync()).PricingSettings;

            var service = _services
                .Single(s => s.Marketplace == prefs.Marketplace);

            return await service.GetUnassignedValueAsync(
                prefs.NonFoilMetric,
                prefs.FoilMetric
            );
        }

        public async Task<(decimal, int)> GetBinderValueAsync(
            Guid binderId
        )
        {
            var prefs = (await _session.GetAsync()).PricingSettings;

            var service = _services
                .Single(s => s.Marketplace == prefs.Marketplace);

            return await service.GetBinderValueAsync(
                binderId,
                prefs.NonFoilMetric,
                prefs.FoilMetric
            );
        }

        public async Task<(decimal, int)> GetDeckValueAsync(
            Guid deckId
        )
        {
            var prefs = (await _session.GetAsync()).PricingSettings;

            var service = _services
                .Single(s => s.Marketplace == prefs.Marketplace);

            return await service.GetDeckValueAsync(
                deckId,
                prefs.NonFoilMetric,
                prefs.FoilMetric
            );
        }


        public async Task<List<PricingEditDto>> GetPricingEditsAsync()
        {
            var prefs = (await _session.GetAsync()).PricingSettings;

            var service = _services
                .Single(s => s.Marketplace == prefs.Marketplace);

            return await service.GetPricingEditsAsync(
                prefs.NonFoilMetric,
                prefs.FoilMetric
            );
        }
    }

}