using HtmlAgilityPack;
using HTMLToQPDF.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components
{
    internal class ParagraphComponent : IComponent
    {
        private readonly List<HtmlNode> _lineNodes;
        private readonly Dictionary<string, TextStyle> _textStyles;

        public ParagraphComponent(List<HtmlNode> lineNodes, HtmlComponentsArgs args)
        {
            _lineNodes = lineNodes;
            _textStyles = args.TextStyles;
        }

        private static HtmlNode? GetParrentBlock(HtmlNode node)
        {
            while (true)
            {
                if (node == null) return null;
                if (node.IsBlockNode()) return node;
                node = node.ParentNode;
            }
        }

        private static HtmlNode? GetListItemNode(HtmlNode node)
        {
            while (true)
            {
                if (node == null || node.IsList()) return null;
                if (node.IsListItem()) return node;
                node = node.ParentNode;
            }
        }

        public void Compose(IContainer container)
        {
            var listItemNode = GetListItemNode(_lineNodes.First()) ?? GetParrentBlock(_lineNodes.First());
            if (listItemNode == null) return;

            var numberInList = listItemNode.GetNumberInList();

            if (numberInList != -1 || listItemNode.GetListNode() != null)
            {
                container.Row(row =>
                {
                    var listPrefix = numberInList switch
                    {
                        -1 => "",
                        0 => "•  ",
                        _ => $"{numberInList}. "
                    };
                    row.AutoItem().MinWidth(13).AlignCenter().Text(listPrefix);
                    container = row.RelativeItem();
                });
            }

            var first = _lineNodes.First();
            var last = _lineNodes.First();

            first.InnerHtml = first.InnerHtml.TrimStart();
            last.InnerHtml = last.InnerHtml.TrimEnd();

            container.Text(GetAction());
        }

        private Action<TextDescriptor> GetAction()
        {
            return text =>
            {
                _lineNodes.ForEach(node => GetAction(node).Invoke(text));
            };
        }

        private Action<TextDescriptor> GetAction(HtmlNode node)
        {
            return text =>
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    var span = text.Span(node.InnerText);
                    GetTextSpanAction(node).Invoke(span);
                }
                else if (node.IsBr())
                {
                    var span = text.Span("\n");
                    GetTextSpanAction(node).Invoke(span);
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

        private TextSpanAction GetTextSpanAction(HtmlNode node)
        {
            return spanAction =>
            {
                var action = GetTextStyles(node);
                action(spanAction);
                if (node.ParentNode == null) return;
                var parrentAction = GetTextSpanAction(node.ParentNode);
                parrentAction(spanAction);
            };
        }

        private TextSpanAction GetTextStyles(HtmlNode element)
        {
            return span => span.Style(GetTextStyle(element));
        }

        private TextStyle GetTextStyle(HtmlNode element)
        {
            return _textStyles.TryGetValue(element.Name.ToLower(), out var style) ? style : TextStyle.Default;
        }
    }
}