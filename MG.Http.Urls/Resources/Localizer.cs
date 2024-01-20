using System.Globalization;
using System.Resources;

namespace MG.Http.Urls.Resources
{
    internal static class Localizer
    {
        const string DEF_MSG = "An error occurred while attempting to retrieve the localized message.";
        static readonly ResourceManager _resourceManager;

        static Localizer()
        {
            _resourceManager = new(typeof(Messages))
            {
                IgnoreCase = true,
            };
        }

        internal static string GetString(string name)
        {
            return _resourceManager.GetString(name, CultureInfo.CurrentUICulture)
                ?? DEF_MSG;
        }
    }
}

