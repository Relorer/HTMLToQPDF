using HtmlAgilityPack;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace HTMLQuestPDF.Components.Tags
{
    internal class ImgComponent : BaseHTMLComponent
    {
        private readonly GetImgBySrc getImgBySrc;
        private readonly float minImgWidth;
        private readonly float maxImgHeight;

        public ImgComponent(HtmlNode node, HTMLComponentsArgs args) : base(node, args)
        {
            this.getImgBySrc = args.GetImgBySrc;
            this.minImgWidth = args.MinImgWidth;
            this.maxImgHeight = args.MaxImgHeight;
        }

        protected override void ComposeSingle(IContainer container)
        {
            var src = node.GetAttributeValue("src", "");
            var img = getImgBySrc(src);

            var item = container.AlignCenter();
            if (img?.Any() ?? false)
            {
                item.Element(e =>
                {
                    var image = SKImage.FromEncodedData(img);

                    if (minImgWidth == 0 && maxImgHeight != 0) return e.MaxHeight(maxImgHeight);
                    if (minImgWidth == 0 || maxImgHeight == 0) return e;

                    var requiredHeight = image.Height * (minImgWidth / image.Width);
                    return requiredHeight > maxImgHeight ? e.MinHeight(maxImgHeight) : e;
                }).Image(img, ImageScaling.FitArea);
            }
            else
            {
                item.Image(Placeholders.Image(200, 100), ImageScaling.FitArea);
            }
        }
    }
}