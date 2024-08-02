using System.Net;

namespace HTMLToQPDF.Utils
{
    internal static class ImgUtils
    {

        private static HttpClient _SingletonClient;

        static ImgUtils() {

            if (_SingletonClient == null)
            {
                _SingletonClient = new HttpClient();
            }
        }

        /// <summary>
        /// Call this manually at the end of the appliction life : Static classes don't auto Dispose.  
        /// </summary>
        public static void Dispose() {

            if (_SingletonClient != null)
            {
                _SingletonClient.Dispose();
            }
        }
        
        public static byte[]? GetImgBySrc(string src)
        {
            try
            {
                if (src.Contains("base64"))
                {
                    var base64 = src.Substring(src.IndexOf("base64,") + "base64,".Length);
                    return Convert.FromBase64String(base64);
                }

                else {
                    return Download(src).Result;
                }

           
            }
            catch
            {
                return null;
            }
        }

        private static Task<byte[]> Download(string src)
        {

            if (_SingletonClient != null)
            {
                Uri uri = new Uri(src);
                return _SingletonClient.GetByteArrayAsync(uri);
            }
            else
            {
                // To prevent memory and IO socket Leaks HttpClient should be a singleton for life. 
                throw new Exception("HttpClient not Available");
            }
        }
     
    }
}