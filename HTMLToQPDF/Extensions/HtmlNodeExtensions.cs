using HtmlAgilityPack;

namespace HTMLToQPDF.Extensions;

internal static class HtmlNodeExtensions
{
    /// <param name="node"></param>
    extension(HtmlNode node)
    {
        public HtmlNode? GetListNode()
        {
            if (node.IsList()) return node;
            return node.ParentNode == null ? null : GetListNode(node.ParentNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// -1 - not a list
        /// 0 - marked list
        /// > 0 - number in the list
        /// </returns>
        public int GetNumberInList()
        {
            HtmlNode? listItem = null;

            if (node != null && node.IsListItem()) listItem = node;
            if (node?.ParentNode != null && node.ParentNode.IsListItem()) listItem = node.ParentNode;

            if (listItem == null) return -1;
            var listNode = listItem.GetListNode();
            if (listNode == null || listNode.IsMarkedList()) return 0;

            return listNode.Descendants("li").Where(n => n.GetListNode() == listNode).ToList().IndexOf(listItem) + 1;
        }

        public List<List<HtmlNode>> GetSlices(List<HtmlNode> slice)
        {
            var result = new List<List<HtmlNode>>();

            if (node.ChildNodes.Count == 0 || node.NodeType == HtmlNodeType.Text)
            {
                result.Add(slice);
            }
            else
            {
                foreach (var item in node.ChildNodes)
                {
                    result.AddRange(GetSlices(item, [..slice, item]));
                }
            }

            return result;
        }

        public bool HasBlockElement()
        {
            return node.ChildNodes.Select(child => child.IsBlockNode() || HasBlockElement(child)).FirstOrDefault();
        }

        public bool HasContent()
        {
            if (node.ChildNodes.Any(HasContent))
            {
                return true;
            }

            return !node.IsEmpty();
        }

        public bool IsBlockNode()
        {
            return HtmlMapSettings.BlockElements.Contains(node.Name.ToLower());
        }

        public bool IsBr()
        {
            return node.Name.Equals("br", StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsTr()
        {
            return node.Name.Equals("tr", StringComparison.CurrentCultureIgnoreCase);
        }

        private bool IsEmpty()
        {
            return string.IsNullOrEmpty(node.InnerText) && !node.IsImg() && !node.IsBr();
        }

        private bool IsImg()
        {
            return node.Name.Equals("img", StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsTable()
        {
            return node.Name.Equals("table", StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsLineNode()
        {
            return HtmlMapSettings.LineElements.Contains(node.Name.ToLower());
        }

        private bool IsLink()
        {
            return node.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsList()
        {
            return node.IsMarkedList() || node.IsNumberedList();
        }

        public bool IsListItem()
        {
            return node.Name.Equals("li", StringComparison.CurrentCultureIgnoreCase);
        }

        private bool IsMarkedList()
        {
            return node.Name.Equals("ul", StringComparison.CurrentCultureIgnoreCase);
        }

        private bool IsNumberedList()
        {
            return node.Name.Equals("ol", StringComparison.CurrentCultureIgnoreCase);
        }

        public bool TryGetLink(out string url)
        {
            var current = node;
            while (current != null)
            {
                if (node.IsLink())
                {
                    url = node.GetAttributeValue("href", "");
                    return true;
                }
                current = node.ParentNode;
            }

            url = "";
            return false;
        }
    }
}