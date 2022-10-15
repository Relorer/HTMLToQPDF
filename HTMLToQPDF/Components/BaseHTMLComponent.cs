using HtmlAgilityPack;
using HTMLQuestPDF.Extensions;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components
{
    internal class BaseHTMLComponent : IComponent
    {
        protected readonly HTMLComponentsArgs args;
        protected readonly HtmlNode node;

        public BaseHTMLComponent(HtmlNode node, HTMLComponentsArgs args)
        {
            this.node = node;
            this.args = args;
        }

        public void Compose(IContainer container)
        {
            if (!node.HasContent() || node.Name.ToLower() == "head") return;

            container = ApplyStyles(container);

            if (node.ChildNodes.Any())
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
            return args.ContainerStyles.TryGetValue(node.Name.ToLower(), out var style) ? style(container) : container;
        }

        protected virtual void ComposeSingle(IContainer container)
        {
        }

        protected virtual void ComposeMany(IContainer container)
        {
            container.Column(col =>
            {
                var buffer = new List<HtmlNode>();
                foreach (var item in node.ChildNodes)
                {
                    if (item.IsBlockNode() || item.HasBlockElement())
                    {
                        ComposeMany(col, buffer);
                        buffer.Clear();

                        col.Item().Component(item.GetComponent(args));
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
            if (nodes.Count == 1)
            {
                col.Item().Component(nodes.First().GetComponent(args));
            }
            else if (nodes.Count > 0)
            {
                col.Item().Component(new ParagraphComponent(nodes, args));
            }
        }
    }
}