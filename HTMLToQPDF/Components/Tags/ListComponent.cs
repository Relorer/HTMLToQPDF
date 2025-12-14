using HtmlAgilityPack;
using HTMLToQPDF.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components.Tags
{
    internal class ListComponent : BaseHtmlComponent
    {
        public ListComponent(HtmlNode node, HtmlComponentsArgs args) : base(node, args)
        {
        }

        protected override IContainer ApplyStyles(IContainer container)
        {
            var first = IsFirstList(Node);
            return base.ApplyStyles(container).Element(e => first ? e.PaddingVertical(Args.ListVerticalPadding) : e);
        }

        private static bool IsFirstList(HtmlNode node)
        {
            if (node.ParentNode == null) return true;
            return !node.ParentNode.IsList() && IsFirstList(node.ParentNode);
        }
    }
}