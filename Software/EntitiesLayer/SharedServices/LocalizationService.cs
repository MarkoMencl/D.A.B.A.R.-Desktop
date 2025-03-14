using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Entities.SharedServices
{
    public static class LocalizationService
    {
        public static string GetLocalizedString(string key)
        {
            try
            {
                Assembly presentationAssembly = Assembly.Load("PresentationLayer");
                Console.WriteLine($"Loaded Assembly: {presentationAssembly.FullName}");

                ResourceManager resourceManager = new ResourceManager("PresentationLayer.Resources.Strings", presentationAssembly);

                var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                if (resourceSet != null)
                {
                    Console.WriteLine("Available Resource Keys:");
                    foreach (System.Collections.DictionaryEntry entry in resourceSet)
                    {
                        Console.WriteLine(entry.Key);
                    }
                }

                string localizedString = resourceManager.GetString(key, CultureInfo.CurrentUICulture);
                Console.WriteLine($"Localized String for '{key}': {localizedString}");

                return localizedString ?? $"[Missing resource: {key}]";
            }
            catch (MissingManifestResourceException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"[Missing resource: {key}]";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return $"[Error: {key}]";
            }
        }
    }
}