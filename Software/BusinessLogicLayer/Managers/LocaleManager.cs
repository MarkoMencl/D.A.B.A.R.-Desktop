using System;
using System.Globalization;
using System.Threading;
using Entities.SharedServices;

namespace BusinessLogicLayer.Managers
{
    public static class LocaleManager
    {
        public static void SetLanguage(string cultureCode)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
            Console.WriteLine($"Culture set to: {cultureCode}");
        }

        public static string GetLocalizedString(string key)
        {
            return LocalizationService.GetLocalizedString(key);
        }
    }
}