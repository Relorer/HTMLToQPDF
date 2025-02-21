using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components
{
    public delegate byte[]? GetImgBySrc(string src);

    internal class HTMLComponentsArgs
    {
        public Dictionary<string, TextStyle> TextStyles { get; }
        public Dictionary<string, Func<IContainer, IContainer>> ContainerStyles { get; }
        public Dictionary<string, TextHorizontalAlignment> TextAlignments { get; }
        public float ListVerticalPadding { get; }
        public GetImgBySrc GetImgBySrc { get; }

        public HTMLComponentsArgs(Dictionary<string, TextStyle> textStyles, Dictionary<string, Func<IContainer, IContainer>> containerStyles, Dictionary<string, TextHorizontalAlignment> textAlignments, float listVerticalPadding, GetImgBySrc getImgBySrc)
        {
            TextStyles = textStyles;
            ContainerStyles = containerStyles;
            TextAlignments = textAlignments;
            ListVerticalPadding = listVerticalPadding;
            GetImgBySrc = getImgBySrc;
        }
    }
}