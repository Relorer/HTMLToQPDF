using HtmlAgilityPack;
using HTMLToQPDF.Components;
using HTMLToQPDF.Components.Tags;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF
{
    internal static class HtmlMapSettings
    {
        public static readonly string[] LineElements =
        [
            "a",
            "b",
            "br",
            "del",
            "em",
            "i",
            "s",
            "small",
            "space",
            "strike",
            "strong",
            "sub",
            "sup",
            "tbody",
            "td",
            "th",
            "thead",
            "tr",
            "u",
            "img",
            "text"
        ];

        public static readonly string[] BlockElements =
        [
            "#document",
            "div",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "li",
            "ol",
            "p",
            "table",
            "ul",
            "section",
            "header",
            "footer",
            "head",
            "html"
        ];

        public static IComponent GetComponent(this HtmlNode node, HtmlComponentsArgs args)
        {
            return node.Name.ToLower() switch
            {
                "#text" or "h1" or "h2" or "h3" or "h4" or "h5" or "h6" or "b" or "s" or "strike" or "i" or "small" or "u" or "del" or "em" or "strong" or "sub" or "sup"
                    => new ParagraphComponent([node], args),
                "br" => new BrComponent(node, args),
                "a" => new AComponent(node, args),
                "div" => new BaseHtmlComponent(node, args),
                "table" => new TableComponent(node, args),
                "ul" or "ol" => new ListComponent(node, args),
                "img" => new ImgComponent(node, args),
                _ => new BaseHtmlComponent(node, args),
            };
        }
    }
}