using HtmlAgilityPack;
using HTMLToQPDF.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Components.Tags
{
    internal class TableComponent : BaseHtmlComponent
    {
        private delegate (uint, uint) GetPositionDelegate(int rowIndex, uint colSpan, uint rowSpan);

        public TableComponent(HtmlNode node, HtmlComponentsArgs args) : base(node, args)
        {
        }

        private HtmlNodeCollection GetCellNodes()
        {
            Node.Id = string.IsNullOrEmpty(Node.Id) ? Guid.NewGuid().ToString() : Node.Id;
            return Node.SelectNodes($"(//table[@id=\"{Node.Id}\"]//th | //table[@id=\"{Node.Id}\"]//td)");
        }

        private List<List<HtmlNode>> GetTableLines()
        {
            var tableItems = GetCellNodes();

            var lines = new List<List<HtmlNode>>();

            var lastLine = new List<HtmlNode>();
            var lastTr = GetTr(tableItems.First());

            foreach (var item in tableItems)
            {
                var currentTr = GetTr(item);
                if (lastTr != currentTr)
                {
                    lines.Add(lastLine);
                    lastLine = [];
                    lastTr = currentTr;
                }
                lastLine.Add(item);
            }

            if (lastLine != null) lines.Add(lastLine);

            return lines;
        }

        protected override void ComposeMany(IContainer container)
        {
            container.Table(table =>
                {
                    var lines = GetTableLines();

                    var maxColumns = lines.Max(l => l.Select(n => n.GetAttributeValue("colspan", 1)).Aggregate((a, b) => a + b));

                    table.ColumnsDefinition(columns =>
                    {
                        for (var i = 0; i < maxColumns; i++)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    var getNextPosition = GetFuncGettingNextPosition(maxColumns);

                    foreach (var line in lines)
                    {
                        foreach (var cell in line)
                        {
                            var colSpan = (uint)cell.GetAttributeValue("colspan", 1);
                            var rowSpan = (uint)cell.GetAttributeValue("rowspan", 1);

                            var (col, row) = getNextPosition(lines.IndexOf(line), colSpan, rowSpan);

                            table.Cell()
                            .ColumnSpan(colSpan)
                            .Column(col)
                            .Row(row)
                            .RowSpan(rowSpan)
                            .Border(1)
                            .Padding(5)
                            .Component(cell.GetComponent(Args));
                        }
                    }
                });
        }

        private GetPositionDelegate GetFuncGettingNextPosition(int maxColumns)
        {
            var rows = new List<bool[]>();
            return (rowIndex, colSpan, rowSpan) =>
            {
                uint col = 0;
                var row = (uint)rowIndex;

                if (rows.Count <= rowIndex) rows.Add(new bool[maxColumns]);

                while (rows[rowIndex][col])
                {
                    col++;
                }

                for (var j = 0; j < rowSpan; j++)
                {
                    for (var i = 0; i < colSpan; i++)
                    {
                        if (rows.Count <= rowIndex + j) rows.Add(new bool[maxColumns]);
                        rows[rowIndex + j][col + i] = true;
                    }
                }

                return (col + 1, row + 1);
            };
        }

        private static HtmlNode? GetTr(HtmlNode node)
        {
            while (true)
            {
                if (node.IsTable() || node == null) return null;
                if (node.IsTr()) return node;
                node = node.ParentNode;
            }
        }
    }
}