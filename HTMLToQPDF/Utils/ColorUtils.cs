using System.Globalization;
using System.Text.RegularExpressions;

namespace HTMLToQPDF.Utils
{
    internal static class ColorUtils
    {
        public static string ColorToHex(string color)
        {
            string hexColor = string.Empty;

            try
            {
                // supports color in hexadecimal, RGB, and RGBA format
                if (color.StartsWith("#"))
                {
                    hexColor = CheckHexFormat(color) ? color : string.Empty;
                }
                else if (color.StartsWith("rgba"))
                {
                    var hex = RgbaToHex(color);
                    hexColor = CheckHexFormat(hex) ? hex : string.Empty;
                }
                else if (color.StartsWith("rgb"))
                {
                    var hex = RgbToHex(color);
                    hexColor = CheckHexFormat(hex) ? hex : string.Empty;
                }
            }
            catch (Exception e)
            {
                hexColor = string.Empty;
            }
            
            return hexColor;
        }

        private static bool CheckHexFormat(string color)
        {
            var pattern = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";

            return Regex.IsMatch(color, pattern);
        }

        private static string RgbaToHex(string rgba)
        {
            string rgbaValues = rgba.Replace("rgba(", string.Empty).Replace(")", string.Empty);
            string[] parts = rgbaValues.Split(',');

            int r = int.Parse(parts[0].Trim());
            int g = int.Parse(parts[1].Trim());
            int b = int.Parse(parts[2].Trim());

            double a = double.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);

            string hex = $"#{r:X2}{g:X2}{b:X2}";

            return hex;
        }

        private static string RgbToHex(string rgb)
        {
            string rgbValues = rgb.Replace("rgb(", string.Empty).Replace(")", string.Empty);
            string[] parts = rgbValues.Split(',');

            int r = int.Parse(parts[0].Trim());
            int g = int.Parse(parts[1].Trim());
            int b = int.Parse(parts[2].Trim());

            string hex = $"#{r:X2}{g:X2}{b:X2}";

            return hex;
        }
    }
}
