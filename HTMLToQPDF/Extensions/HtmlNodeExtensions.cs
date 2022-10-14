using HtmlAgilityPack;

namespace HTMLQuestPDF.Extensions
{
    internal static class HtmlNodeExtensions
    {
        public static HtmlNode? GetListNode(this HtmlNode node)
        {
            if (node.IsList()) return node;
            if (node.ParentNode == null) return null;
            return GetListNode(node.ParentNode);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// -1 - not a list
        /// 0 - marked list
        /// > 0 - number in the list
        /// </returns>
        public static int GetNumberInList(this HtmlNode node)
        {
            HtmlNode? listItem = null;

            if (node != null && node.IsListItem()) listItem = node;
            if (node?.ParentNode != null && node.ParentNode.IsListItem()) listItem = node.ParentNode;

            if (listItem != null)
            {
                var listNode = listItem.GetListNode();
                if (listNode == null || listNode.IsMarkedList()) return 0;

                return listNode.Descendants("li").Where(n => n.GetListNode() == listNode).ToList().IndexOf(listItem) + 1;
            }
            return -1;
        }

        public static List<List<HtmlNode>> GetSlices(this HtmlNode node, List<HtmlNode> slice)
        {
            var result = new List<List<HtmlNode>>();

            if (!node.ChildNodes.Any() || node.NodeType == HtmlNodeType.Text)
            {
                result.Add(slice);
                return result;
            }
            else
            {
                foreach (var item in node.ChildNodes)
                {
                    result.AddRange(GetSlices(item, new List<HtmlNode>(slice) { item }));
                }
            }

            return result;
        }

        public static bool HasBlockElement(this HtmlNode node)
        {
            foreach (var child in node.ChildNodes)
            {
                return child.IsBlockNode() || HasBlockElement(child);
            }
            return false;
        }

        public static bool HasContent(this HtmlNode node)
        {
            foreach (var item in node.ChildNodes)
            {
                if (HasContent(item)) return true;
            }
            return !node.IsEmpty();
        }

        public static bool IsBlockNode(this HtmlNode node)
        {
            return HTMLMapSettings.BlockElements.Contains(node.Name.ToLower());
        }

        public static bool IsBr(this HtmlNode node)
        {
            return node.Name.ToLower() == "br";
        }

        public static bool IsTr(this HtmlNode node)
        {
            return node.Name.ToLower() == "tr";
        }

        public static bool IsEmpty(this HtmlNode node)
        {
            return string.IsNullOrEmpty(node.InnerText) && !node.IsImg() && !node.IsBr();
        }

        public static bool IsImg(this HtmlNode node)
        {
            return node.Name.ToLower() == "img";
        }

        public static bool IsTable(this HtmlNode node)
        {
            return node.Name.ToLower() == "table";
        }

        public static bool IsLineNode(this HtmlNode node)
        {
            return HTMLMapSettings.LineElements.Contains(node.Name.ToLower());
        }

        public static bool IsLink(this HtmlNode node)
        {
            return node.Name.ToLower() == "a";
        }

        public static bool IsList(this HtmlNode node)
        {
            return node.IsMarkedList() || node.IsNumberedList();
        }

        public static bool IsListItem(this HtmlNode node)
        {
            return node.Name.ToLower() == "li";
        }

        public static bool IsMarkedList(this HtmlNode node)
        {
            return node.Name.ToLower() == "ul";
        }

        public static bool IsNumberedList(this HtmlNode node)
        {
            return node.Name.ToLower() == "ol";
        }

        public static bool TryGetLink(this HtmlNode node, out string url)
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