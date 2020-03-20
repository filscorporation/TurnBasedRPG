using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Localization
{
    public enum Language
    {
        English = 0,
        Russian = 1,
    }

    /// <summary>
    /// Loads and handles all localized values
    /// </summary>
    public class LocalizationManager
    {
        public static Language CurrentLanguage = Language.English;

        private static bool isInitialized = false;
        private static Dictionary<string, List<string>> localization;
        private static readonly string jsonPath = Path.Combine("Localization", "localization");

        public static void Initialize()
        {
            TextAsset file = Resources.Load(jsonPath) as TextAsset;
            if (file == null)
                throw new Exception("Localization file not found");
            string text = file.ToString();
            localization = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text);

            isInitialized = true;
        }

        public static string GetLocalizedValue(string key)
        {
            if (!isInitialized)
                Initialize();

            if (localization.TryGetValue(key, out var value))
            {
                return value[(int) CurrentLanguage];
            }

            return key;
        }
    }
}
