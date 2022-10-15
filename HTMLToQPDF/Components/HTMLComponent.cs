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

        public Dictionary<string, TextStyle> TextStyles { get; } = new Dictionary<string, TextStyle>()
        {
            { "h1", TextStyle.Default.FontSize(32).Bold() },
            { "h2", TextStyle.Default.FontSize(28).Bold() },
            { "h3", TextStyle.Default.FontSize(24).Bold() },
            { "h4", TextStyle.Default.FontSize(20).Bold() },
            { "h5", TextStyle.Default.FontSize(16).Bold() },
            { "h6", TextStyle.Default.FontSize(12).Bold() },
            { "b", TextStyle.Default.Bold() },
            { "i", TextStyle.Default.Italic() },
            { "small", TextStyle.Default.Light() },
            { "strike", TextStyle.Default.Strikethrough() },
            { "s", TextStyle.Default.Strikethrough() },
            { "u", TextStyle.Default.Underline() },
            { "a", TextStyle.Default.Underline() },
        };

        public Dictionary<string, Func<IContainer, IContainer>> ContainerStyles { get; } = new Dictionary<string, Func<IContainer, IContainer>>()
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

            container.Component(node.GetComponent(new HTMLComponentsArgs(TextStyles, ContainerStyles, ListVerticalPadding, GetImgBySrc)));
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