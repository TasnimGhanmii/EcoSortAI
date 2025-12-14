using sevrer.Api.Enums;

namespace sevrer.Api.ML
{
    public static class WastePredictionMapper
    {
        private static readonly string[] Labels =
        {
            "cardboard", "glass", "metal", "paper", "plastic", "trash"
        };

        public static string GetLabelFromKey(uint key)
        {
            return key < Labels.Length ? Labels[key] : "unknown";
        }

        public static WasteCategory MapKeyToCategory(uint key)
        {
            var label = GetLabelFromKey(key);
            return label.ToLowerInvariant() switch
            {
                "plastic" or "glass" or "metal" or "paper" or "cardboard" => WasteCategory.Recyclable,
                "organic" or "food" or "compost" or "banana" or "apple" => WasteCategory.Compostable,
                "battery" or "chemical" or "electronic" or "mercury" => WasteCategory.Hazardous,
                "trash" => WasteCategory.General,
                _ => WasteCategory.General
            };
        }
    }
}