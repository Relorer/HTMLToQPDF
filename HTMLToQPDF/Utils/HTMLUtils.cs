using System.Text.RegularExpressions;
using System.Web;

namespace HTMLToQPDF.Utils
{
    internal static partial class HtmlUtils
    {
        private static readonly string SpaceAfterLineElementPattern = @$"\S<\/({string.Join("|", HtmlMapSettings.LineElements)})> ";

        public static string PrepareHtml(string value)
        {
            var result = HttpUtility.HtmlDecode(value);
            result = RemoveExtraSpacesAndBreaks(result);
            result = RemoveSpacesAroundBr(result);
            result = WrapSpacesAfterLineElement(result);
            result = RemoveSpacesBetweenElements(result);
            return result;
        }

        private static string RemoveExtraSpacesAndBreaks(string html)
        {
            return RemoveExtraSpacesAndBreaksRegex().Replace(html, " ");
        }

        private static string RemoveSpacesBetweenElements(string html)
        {
            return RemoveSpacesBetweenElementsRegex().Replace(html, _ => "><").Replace("<space></space>", "<space> </space>");
        }

        private static string RemoveSpacesAroundBr(string html)
        {
            return RemoveSpacesAroundBrRegex().Replace(html, _ => "<br>");
        }

        private static string WrapSpacesAfterLineElement(string html)
        {
            return Regex.Replace(html, SpaceAfterLineElementPattern, m => $"{m.Value[..^1]}<space> </space>");
        }

        [GeneratedRegex(@"[ \r\n]+")]
        private static partial Regex RemoveExtraSpacesAndBreaksRegex();
        [GeneratedRegex(@">\s+<")]
        private static partial Regex RemoveSpacesBetweenElementsRegex();
        [GeneratedRegex(@"\s+<\/?br\s*\/?>\s+")]
        private static partial Regex RemoveSpacesAroundBrRegex();
    }
}