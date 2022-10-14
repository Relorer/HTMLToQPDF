using QuestPDF.Fluent;

namespace HTMLQuestPDF.Extensions
{
    public static class HTMLComponentExtensions
    {
        public static void HTML(this PageDescriptor pageDescriptor, Action<HTMLDescriptor> handler)
        {
            var htmlPageDescriptor = new HTMLDescriptor();
            handler(htmlPageDescriptor);
            pageDescriptor.Content().Component(htmlPageDescriptor.PDFPage);
        }
    }
}