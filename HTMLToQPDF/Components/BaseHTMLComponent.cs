using HtmlAgilityPack;
using HTMLToQPDF.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components
{
    internal class BaseHtmlComponent : IComponent
    {
        protected readonly HtmlComponentsArgs Args;
        protected readonly HtmlNode Node;

        public BaseHtmlComponent(HtmlNode node, HtmlComponentsArgs args)
        {
            Node = node;
            Args = args;
        }

        public void Compose(IContainer container)
        {
            if (!Node.HasContent() || Node.Name.Equals("head", StringComparison.CurrentCultureIgnoreCase)) return;

            container = ApplyStyles(container);

            if (Node.ChildNodes.Count != 0)
            {
                ComposeMany(container);
            }
            else
            {
                ComposeSingle(container);
            }
        }

        protected virtual IContainer ApplyStyles(IContainer container)
        {
            return Args.ContainerStyles.TryGetValue(Node.Name.ToLower(), out var style) ? style(container) : container;
        }

        protected virtual void ComposeSingle(IContainer container)
        {
        }

        protected virtual void ComposeMany(IContainer container)
        {
            container.Column(col =>
            {
                var buffer = new List<HtmlNode>();
                foreach (var item in Node.ChildNodes)
                {
                    if (item.IsBlockNode() || item.HasBlockElement())
                    {
                        ComposeMany(col, buffer);
                        buffer.Clear();

                        col.Item().Component(item.GetComponent(Args));
                    }
                    else
                    {
                        buffer.Add(item);
                    }
                }
                ComposeMany(col, buffer);
            });
        }

        private void ComposeMany(ColumnDescriptor col, List<HtmlNode> nodes)
        {
            switch (nodes.Count)
            {
                case 1:
                    col.Item().Component(nodes.First().GetComponent(Args));
                    break;
                case > 0:
                    col.Item().Component(new ParagraphComponent(nodes, Args));
                    break;
            }
        }
    }
}