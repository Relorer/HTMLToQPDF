namespace HTMLToQPDF.Utils
{
    internal static class ImgUtils
    {
        public static byte[]? GetImgBySrc(string src)
        {
            try
            {
                if (src.Contains("base64"))
                {
                    var base64 = src.Substring(src.IndexOf("base64,") + "base64,".Length);
                    return Convert.FromBase64String(base64);
                }
                var webClient = new HttpClient();
                var response = webClient.GetAsync(src).Result;
                return response.Content.ReadAsByteArrayAsync().Result;
            }
            catch
            {
                return null;
            }
        }
    }
}