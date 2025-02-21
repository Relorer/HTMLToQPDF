using HtmlAgilityPack;
using HTMLQuestPDF.Extensions;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components
{
    internal class ParagraphComponent : IComponent
    {
        private readonly List<HtmlNode> lineNodes;
        private readonly Dictionary<string, TextStyle> textStyles;
        private readonly Dictionary<string, TextHorizontalAlignment> textAlignments;

        public ParagraphComponent(List<HtmlNode> lineNodes, HTMLComponentsArgs args)
        {
            this.lineNodes = lineNodes;
            this.textStyles = args.TextStyles;
            this.textAlignments = args.TextAlignments;
        }

        private HtmlNode? GetParentBlock(HtmlNode node)
        {
            if (node == null) return null;
            return node.IsBlockNode() ? node : GetParentBlock(node.ParentNode);
        }

        private HtmlNode? GetListItemNode(HtmlNode node)
        {
            if (node == null || node.IsList()) return null;
            return node.IsListItem() ? node : GetListItemNode(node.ParentNode);
        }

        public void Compose(IContainer container)
        {
            var listItemNode = GetListItemNode(lineNodes.First()) ?? GetParentBlock(lineNodes.First());
            if (listItemNode == null) return;

            var numberInList = listItemNode.GetNumberInList();

            if (numberInList != -1 || listItemNode.GetListNode() != null)
            {
                container.Row(row =>
                {
                    var listPrefix = numberInList == -1 ? "" : numberInList == 0 ? "•  " : $"{numberInList}. ";
                    row.AutoItem().MinWidth(26).AlignCenter().Text(listPrefix);
                    container = row.RelativeItem();
                });
            }

            var first = lineNodes.First();
            var last = lineNodes.First();

            first.InnerHtml = first.InnerHtml.TrimStart();
            last.InnerHtml = last.InnerHtml.TrimEnd();

            container.Text(GetAction(lineNodes));
        }

        private Action<TextDescriptor> GetAction(List<HtmlNode> nodes)
        {
            return text =>
            {
                lineNodes.ForEach(node => GetAction(node).Invoke(text));
            };
        }

        private Action<TextDescriptor> GetAction(HtmlNode node)
        {
            return text =>
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    var span = text.Span(node.InnerText);
                    GetTextSpanAction(node).Invoke(span, text);
                }
                else if (node.IsBr())
                {
                    var span = text.Span("\n");
                    GetTextSpanAction(node).Invoke(span, text);
                }
                else
                {
                    foreach (var item in node.ChildNodes)
                    {
                        var action = GetAction(item);
                        action(text);
                    }
                }
            };
        }

        private Action<TextSpanDescriptor, TextDescriptor> GetTextSpanAction(HtmlNode node)
        {
            return (spanAction, text) =>
            {
                var action = GetTextStyles(node);
                action(spanAction);

                var alignment = GetTextAlignment(node);
                alignment(text);

                if (node.ParentNode != null)
                {
                    var parentAction = GetTextSpanAction(node.ParentNode);
                    parentAction(spanAction, text);

                    var parentAlignment = GetTextAlignment(node.ParentNode);
                    parentAlignment(text);
                }
            };
        }

        public TextSpanAction GetTextStyles(HtmlNode element)
        {
            return (span) => span.Style(GetTextStyle(element));
        }

        public TextStyle GetTextStyle(HtmlNode element)
        {
            return textStyles.TryGetValue(element.Name.ToLower(), out TextStyle? style) ? style : TextStyle.Default;
        }

        public Action<TextDescriptor> GetTextAlignment(HtmlNode element)
        {
            switch (textAlignments.TryGetValue(element.Name.ToLower(), out TextHorizontalAlignment alignment) ? alignment : TextHorizontalAlignment.Left)
            {
                case TextHorizontalAlignment.Left:
                    return block => block.AlignLeft();
                case TextHorizontalAlignment.Center:
                    return block => block.AlignCenter();
                case TextHorizontalAlignment.Right:
                    return block => block.AlignRight();
                case TextHorizontalAlignment.Start:
                    return block => block.AlignStart();
                case TextHorizontalAlignment.End:
                    return block => block.AlignEnd();
                case TextHorizontalAlignment.Justify:
                    return block => block.Justify();
                default:
                    return block => block.AlignLeft();
            }
        }
    }
}