namespace UsfoModels.Model
{
    internal static class DefaultRecommendations
    {
        private static readonly List<FormatSet> _defaultRecommendations = new()
        {
            new FormatSet
            {
                MinAge = 7,
                MaxAge = 15,
                Font = "Arial",
                FontSize = 12,
                HeadingFontSize = 14,
                LineSpacing = 1.15M,
                Color = "Black",
                BackgroundColor = "White",
            },
            new FormatSet
            {
                MinAge = 16,
                MaxAge = 39,
                Font = "Arial",
                FontSize = 14,
                HeadingFontSize = 16,
                LineSpacing = 1.5M,
                Color = "Black",
                BackgroundColor = "White",
            },
            new FormatSet
            {
                MinAge = 40,
                MaxAge = 999,
                Font = "Arial",
                FontSize = 16,
                HeadingFontSize = 18,
                LineSpacing = 1.5M,
                Color = "Black",
                BackgroundColor = "White",
            },
        };

        internal static List<FormatSet> Recommentations => _defaultRecommendations;
        internal static FormatSet DefaultSet => _defaultRecommendations[1];
    }
}