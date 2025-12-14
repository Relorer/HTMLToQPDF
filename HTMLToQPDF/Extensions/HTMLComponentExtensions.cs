using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Extensions
{
    public static class HtmlComponentExtensions
    {
        public static void Html(this IContainer container, Action<HtmlDescriptor> handler)
        {
            var htmlPageDescriptor = new HtmlDescriptor();
            handler(htmlPageDescriptor);
            container.Component(htmlPageDescriptor.PdfPage);
        }
    }
}