using System.Net;

namespace HTMLToQPDF.Utils
{
    internal static class ImgUtils
    {
        public static byte[]? GetImgBySrc(string src)
        {
            try
            {
                var webClient = new WebClient();
                return webClient.DownloadData(src);
            }
            catch
            {
                return null;
            }
        }
    }
}