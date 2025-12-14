using HtmlAgilityPack;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components.Tags
{
    internal class ImgComponent : BaseHtmlComponent
    {
        private readonly GetImgBySrc _getImgBySrc;

        public ImgComponent(HtmlNode node, HtmlComponentsArgs args) : base(node, args)
        {
            _getImgBySrc = args.GetImgBySrc;
        }

        protected override void ComposeSingle(IContainer container)
        {
            var src = Node.GetAttributeValue("src", "");
            var img = _getImgBySrc(src) ?? Placeholders.Image(200, 100);
            container.AlignCenter().Image(img).FitArea();
            
        }
    }
}