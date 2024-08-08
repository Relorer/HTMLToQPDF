using HtmlAgilityPack;
using HTMLQuestPDF.Extensions;
using HTMLToQPDF.Components;
using HTMLToQPDF.Utils;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components
{
    internal class ParagraphComponent : IComponent
    {
        private readonly List<HtmlNode> lineNodes;
        private readonly Dictionary<string, TextStyle> textStyles;

        public ParagraphComponent(List<HtmlNode> lineNodes, HTMLComponentsArgs args)
        {
            this.lineNodes = lineNodes;
            this.textStyles = args.TextStyles;
        }

        private HtmlNode? GetParrentBlock(HtmlNode node)
        {
            if (node == null) return null;
            return node.IsBlockNode() ? node : GetParrentBlock(node.ParentNode);
        }

        private HtmlNode? GetListItemNode(HtmlNode node)
        {
            if (node == null || node.IsList()) return null;
            return node.IsListItem() ? node : GetListItemNode(node.ParentNode);
        }

        public void Compose(IContainer container)
        {
            var listItemNode = GetListItemNode(lineNodes.First()) ?? GetParrentBlock(lineNodes.First());
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
                if (node.ParentNode != null)
                {
                    var parrentAction = GetTextSpanAction(node.ParentNode);
                    parrentAction(spanAction);
                }
            };
        }

        public TextSpanAction GetTextStyles(HtmlNode element)
        {
            return (span) => span.Style(GetTextStyle(element));
        }

        public TextStyle GetTextStyle(HtmlNode element)
        {
            var s = textStyles.TryGetValue(element.Name.ToLower(), out TextStyle? style) ? style : TextStyle.Default;
            if (element.HasAttributes)
            {
                s = GetTextAttribute(element, s);
            }
            return s;
        }

        private TextStyle GetTextAttribute(HtmlNode element, TextStyle style)
        {
            foreach (var attr in element.Attributes)
            {
                if(attr.Name == "style")
                {
                    style = ParseStyleString(attr.Value, style);
                }
            }
            return style;
        }

        private TextStyle ParseStyleString(string styleString, TextStyle textStyle)
        {
            List<string> styles = new List<string>(styleString.Split(';'));
            foreach (var s in styles)
            {
                var split = s.Split(':');
                if (split.Length == 2)
                {
                    var styleType = split[0].Trim();
                    var styleValue = split[1].Trim();

                    switch (styleType)
                    {
                        case "background-color":
                            string hexColor = ColorUtils.ColorToHex(styleValue);
                            if (hexColor != string.Empty)
                            {
                                textStyle = textStyle.BackgroundColor(hexColor);
                            }
                            break;
                        case "font-family":
                            string font = FontFamilyUtils.formatFontFamily(styleValue);
                            textStyle = textStyle.FontFamily(font); 
                            break;
                        case "color":
                            hexColor = ColorUtils.ColorToHex(styleValue);
                            if (hexColor != string.Empty)
                            {
                                textStyle = textStyle.FontColor(hexColor);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return textStyle;
        }
    }
}