using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Extensions
{
    public static class HTMLComponentExtensions
    {
        public static void HTML(this IContainer container, Action<HTMLDescriptor> handler)
        {
            var htmlPageDescriptor = new HTMLDescriptor();
            handler(htmlPageDescriptor);
            container.Component(htmlPageDescriptor.PDFPage);
        }
    }
}