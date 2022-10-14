using HtmlAgilityPack;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components.Tags
{
    internal class BrComponent : BaseHTMLComponent
    {
        public BrComponent(HtmlNode node, HTMLComponentsArgs args) : base(node, args)
        {
        }

        protected override void ComposeSingle(IContainer container)
        {
            container.Text("");
        }
    }
}