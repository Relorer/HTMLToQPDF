using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components
{
    public delegate byte[]? GetImgBySrc(string src);

    internal record HTMLComponentsArgs(
        Dictionary<string, Func<TextStyle, TextStyle>> TextStyles,
        Dictionary<string, Func<TextStyle, TextStyle>> CssStyles,
        Dictionary<string, Func<IContainer, IContainer>> ContainerStyles,
        float ListVerticalPadding,
        GetImgBySrc GetImgBySrc);
}