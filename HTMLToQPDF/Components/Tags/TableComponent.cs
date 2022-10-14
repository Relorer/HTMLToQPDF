using HtmlAgilityPack;
using HTMLQuestPDF.Extensions;
using HTMLToQPDF.Components;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Components.Tags
{
    internal class TableComponent : BaseHTMLComponent
    {
        private delegate (uint, uint) GetPositionDelegate(int rowIndex, uint colSpan, uint rowSpan);

        public TableComponent(HtmlNode node, HTMLComponentsArgs args) : base(node, args)
        {
        }

        private HtmlNodeCollection GetCellNodes()
        {
            node.Id = String.IsNullOrEmpty(node.Id) ? Guid.NewGuid().ToString() : node.Id;
            return node.SelectNodes($"(//table[@id=\"{node.Id}\"]//th | //table[@id=\"{node.Id}\"]//td)");
        }

        private List<List<HtmlNode>> GetTableLines()
        {
            var tableItems = GetCellNodes();

            List<List<HtmlNode>> lines = new List<List<HtmlNode>>();

            List<HtmlNode> lastLine = new List<HtmlNode>();
            HtmlNode? lastTr = GetTr(tableItems.First());

            foreach (var item in tableItems)
            {
                var currentTr = GetTr(item);
                if (lastTr != currentTr)
                {
                    lines.Add(lastLine);
                    lastLine = new List<HtmlNode>();
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
                        for (int i = 0; i < maxColumns; i++)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    var getNextPosition = GetFuncGettingNextPosition(maxColumns);

                    foreach (var line in lines)
                    {
                        foreach (var cell in line)
                        {
                            uint colSpan = (uint)cell.GetAttributeValue("colspan", 1);
                            uint rowSpan = (uint)cell.GetAttributeValue("rowspan", 1);

                            (uint col, uint row) = getNextPosition(lines.IndexOf(line), colSpan, rowSpan);

                            table.Cell()
                            .ColumnSpan(colSpan)
                            .Column(col)
                            .Row(row)
                            .RowSpan(rowSpan)
                            .Border(1)
                            .Padding(5)
                            .Component(cell.GetComponent(args));
                        }
                    }
                });
        }

        private GetPositionDelegate GetFuncGettingNextPosition(int maxColumns)
        {
            var rows = new List<bool[]>();
            return (int rowIndex, uint colSpan, uint rowSpan) =>
            {
                uint col = 0;
                uint row = (uint)rowIndex;

                if (rows.Count <= rowIndex) rows.Add(new bool[maxColumns]);

                while (rows[rowIndex][col])
                {
                    col++;
                }

                for (int j = 0; j < rowSpan; j++)
                {
                    for (int i = 0; i < colSpan; i++)
                    {
                        if (rows.Count <= rowIndex + j) rows.Add(new bool[maxColumns]);
                        rows[rowIndex + j][col + i] = true;
                    }
                }

                return (col + 1, row + 1);
            };
        }

        private HtmlNode? GetTr(HtmlNode node)
        {
            if (node.IsTable() || node == null) return null;
            return node.IsTr() ? node : GetTr(node.ParentNode);
        }
    }
}