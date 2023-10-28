namespace UsfoModels.Model
{
    public class FormatSet
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string Font { get; set; } = string.Empty;
        public Decimal FontSize { get; set; }
        public Decimal HeadingFontSize { get; set; }
        public Decimal LineSpacing { get; set; }
        public string Color { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
    }
}