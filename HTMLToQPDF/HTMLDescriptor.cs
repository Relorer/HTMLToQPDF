using HTMLToQPDF.Components;
using HTMLToQPDF.Utils;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF
{
    public class HtmlDescriptor
    {
        internal HtmlComponent PdfPage { get; } = new();

        public void SetHtml(string html)
        {
            PdfPage.Html = html;
        }

        public void OverloadImgReceivingFunc(GetImgBySrc getImg)
        {
            PdfPage.GetImgBySrc = getImg;
        }

        public void SetTextStyleForHtmlElement(string tagName, TextStyle style)
        {
            PdfPage.TextStyles[tagName.ToLower()] = style;
        }

        public void SetContainerStyleForHtmlElement(string tagName, Func<IContainer, IContainer> style)
        {
            PdfPage.ContainerStyles[tagName.ToLower()] = style;
        }

        public void SetListVerticalPadding(float value, Unit unit = Unit.Point)
        {
            PdfPage.ListVerticalPadding = UnitUtils.ToPoints(value, unit);
        }
    }
}