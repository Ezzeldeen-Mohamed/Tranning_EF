using Microsoft.Extensions.Localization;

namespace MeterManagement.Application.Resources
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public LocalizationService(
            IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;
        }

        public string GetString(string key)
        {
            return _localizer[key];
        }
    }
}