using HTMLToQPDF.Components;
using HTMLToQPDF.Utils;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF
{
    public class HTMLDescriptor
    {
        internal HTMLComponent PDFPage { get; } = new HTMLComponent();

        public void SetHtml(string html)
        {
            PDFPage.HTML = html;
        }

        public void OverloadImgReceivingFunc(GetImgBySrc getImg)
        {
            PDFPage.GetImgBySrc = getImg;
        }

        public void SetTextStyleForHtmlElement(string tagName, TextStyle style)
        {
            PDFPage.TextStyles[tagName.ToLower()] = style;
        }

        public void SetContainerStyleForHtmlElement(string tagName, Func<IContainer, IContainer> style)
        {
            PDFPage.ContainerStyles[tagName.ToLower()] = style;
        }

        public void SetListVerticalPadding(float value, Unit unit = Unit.Point)
        {
            PDFPage.ListVerticalPadding = UnitUtils.ToPoints(value, unit);
        }
    }
}