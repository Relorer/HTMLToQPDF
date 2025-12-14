using HtmlAgilityPack;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components.Tags
{
    internal class BrComponent : BaseHtmlComponent
    {
        public BrComponent(HtmlNode node, HtmlComponentsArgs args) : base(node, args)
        {
        }

        protected override void ComposeSingle(IContainer container)
        {
            container.Text("");
        }
    }
}