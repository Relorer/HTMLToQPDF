using HTMLToQPDF.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Example.Utilities
{
    internal static class PdfCreator
    {
        public static void Create(string html, string path, bool customStyles)
        {

            QuestPDF.Settings.License = LicenseType.Community;
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(0.5f, Unit.Centimetre);
                    page.MarginVertical(1f, Unit.Centimetre);

#pragma warning disable CS0618 // Type or member is obsolete
                    page.DefaultTextStyle(TextStyle.Default.FontFamily("Arial").FontSize(8).Fallback(y => y.FontFamily("Segoe UI Emoji")));
#pragma warning restore CS0618

                  /* page.DefaultTextStyle(TextStyle.Default
                    //    .Fallback(y => y.FontFamily("MS Reference Sans Serif")
                        .Fallback(y => y.FontFamily("Segoe UI Emoji")
                      //  .Fallback(y => y.FontFamily("Microsoft YaHei")))
                    ));
                  */

                    page.Content().Column(col =>
                    {
                        col.Item().Html(handler =>
                         {
                             if (customStyles)
                             {
                                 handler.SetTextStyleForHtmlElement("h1", TextStyle.Default.FontColor(Colors.DeepOrange.Accent4).FontSize(32).Bold());
                                 handler.SetContainerStyleForHtmlElement("div", c => c.Background(Colors.Teal.Lighten5));
                                 handler.SetContainerStyleForHtmlElement("img", c => c.MaxHeight(7, Unit.Centimetre));
                                 handler.SetContainerStyleForHtmlElement("table", c => c.Background(Colors.Pink.Lighten5));
                                 handler.SetListVerticalPadding(40);
                             }
                             handler.SetHtml(html);
                         });
                    });
                });
            }).GeneratePdf(path);
        }
    }
}