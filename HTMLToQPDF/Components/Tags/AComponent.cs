using HtmlAgilityPack;
using HTMLToQPDF.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components.Tags
{
    internal class AComponent : BaseHtmlComponent
    {
        public AComponent(HtmlNode node, HtmlComponentsArgs args) : base(node, args)
        {
        }

        protected override IContainer ApplyStyles(IContainer container)
        {
            container = base.ApplyStyles(container);
            return Node.TryGetLink(out var link) ? container.Hyperlink(link) : container;
        }
    }
}