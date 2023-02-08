using HtmlAgilityPack;
using HTMLQuestPDF;
using HTMLQuestPDF.Extensions;
using HTMLQuestPDF.Utils;
using HTMLToQPDF.Utils;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components
{
    internal delegate void ContainerAction(IContainer container);

    internal delegate void TextSpanAction(TextSpanDescriptor textSpan);

    internal class HTMLComponent : IComponent
    {
        public GetImgBySrc GetImgBySrc { get; set; } = ImgUtils.GetImgBySrc;

        public Dictionary<string, Func<TextStyle, TextStyle>> TextStyles { get; } = new()
        {
            { "h1", t => t.FontSize(32).Bold() },
            { "h2", t => t.FontSize(28).Bold() },
            { "h3", t => t.FontSize(24).Bold() },
            { "h4", t => t.FontSize(20).Bold() },
            { "h5", t => t.FontSize(16).Bold() },
            { "h6", t => t.FontSize(12).Bold() },
            { "b", t => t.Bold() },
            { "i", t => t.Italic() },
            { "small", t => t.Light() },
            { "strike", t => t.Strikethrough() },
            { "s", t => t.Strikethrough() },
            { "u", t => t.Underline() },
            { "a", t => t.Underline() },
        };

        public Dictionary<string, Func<TextStyle, TextStyle>> CssStyles { get; } = new();

        public Dictionary<string, Func<IContainer, IContainer>> ContainerStyles { get; } = new()
        {
            { "p", c => c.PaddingVertical(6) },
            { "ul", c => c.PaddingLeft(30) },
            { "ol", c => c.PaddingLeft(30) }
        };

        public float ListVerticalPadding { get; set; } = 12;

        public string HTML { get; set; } = "";

        public void Compose(IContainer container)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(HTMLUtils.PrepareHTML(HTML));
            var node = doc.DocumentNode;

            CreateSeparateBranchesForTextNodes(node);

            var args = new HTMLComponentsArgs(TextStyles, CssStyles, ContainerStyles, ListVerticalPadding, GetImgBySrc);
            container.Component(node.GetComponent(args));
        }

        /// <summary>
        /// Separate branches are created for block and text nodes located in the same linear node
        ///
        /// <p><s><div>div</div>text in s</s>text in p</p>
        /// to
        /// <p><s><div>div</div></s><s>text in s</s>text in p</p>
        ///
        /// This is necessary to avoid extra line breaks
        /// </summary>
        /// <param name="node"></param>
        private void CreateSeparateBranchesForTextNodes(HtmlNode node)
        {
            if (node.IsLineNode() && node.HasBlockElement())
            {
                var slices = node.GetSlices(new List<HtmlNode>() { node });

                var parent = node.ParentNode;
                var children = node.ParentNode.ChildNodes.ToList();

                foreach (var slice in slices)
                {
                    HtmlNode? newNode = null;

                    foreach (var item in slice)
                    {
                        if (newNode == null)
                        {
                            newNode = item.CloneNode(false);
                            children.Insert(children.IndexOf(node), newNode);
                        }
                        else
                        {
                            var temp = item.CloneNode(false);
                            newNode.AppendChild(temp);
                            newNode = temp;
                        }
                    }

                    if (newNode != null)
                    {
                        newNode.InnerHtml = newNode.InnerText.Trim();
                    }
                }

                children.Remove(node);

                node.ParentNode.RemoveAllChildren();
                foreach (var item in children)
                {
                    parent.AppendChild(item);
                }
            }
            else
            {
                foreach (var item in node.ChildNodes.ToList())
                {
                    CreateSeparateBranchesForTextNodes(item);
                }
            }
        }
    }
}