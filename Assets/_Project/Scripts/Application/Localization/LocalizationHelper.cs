using UnityEngine;
using UnityEngine.Localization.Settings;

namespace _Project.Application.Localization
{
    public static class LocalizationHelper
    {
        public static void SetupLocalization(int localizationIndex)
        {
            var validLocalizationIndex = GetValidLocalizationIndex(localizationIndex);
            PlayerPrefs.SetInt("LocalizationIndex", validLocalizationIndex);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[validLocalizationIndex];
        }

        public static int GetValidLocalizationIndex(int localizationIndex)
        {
            switch (localizationIndex)
            {
                case 0:
                case 1:
                    return localizationIndex;
                default: return GetSystemLocalizationIndex();
            }
        }

        public static int GetSystemLocalizationIndex()
        {
            return UnityEngine.Application.systemLanguage switch
            {
                SystemLanguage.Russian => 0,
                _ => 1
            };
        }

        public static string GetLanguageByIndex(int localizationIndex)
        {
            return LocalizationSettings.AvailableLocales.Locales[localizationIndex].LocaleName;
        }

        public static int GetMaxLocales()
        {
            return LocalizationSettings.AvailableLocales.Locales.Count;
        }
    }
}
