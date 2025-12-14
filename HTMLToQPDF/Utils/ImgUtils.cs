namespace HTMLToQPDF.Utils
{
    internal static class ImgUtils
    {

        private static readonly HttpClient SingletonClient;

        static ImgUtils()
        {
            SingletonClient ??= new HttpClient();
        }

        /// <summary>
        /// Call this manually at the end of the appliction life : Static classes don't auto Dispose.  
        /// </summary>
        public static void Dispose() {

            if (SingletonClient != null)
            {
                SingletonClient.Dispose();
            }
        }
        
        public static byte[]? GetImgBySrc(string src)
        {
            try
            {
                if (src.Contains("base64"))
                {
                    var base64 = src[(src.IndexOf("base64,", StringComparison.Ordinal) + "base64,".Length)..];
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

            if (SingletonClient != null)
            {
                var uri = new Uri(src);
                return SingletonClient.GetByteArrayAsync(uri);
            }
            else
            {
                // To prevent memory and IO socket Leaks HttpClient should be a singleton for life. 
                throw new Exception("HttpClient not Available");
            }
        }
     
    }
}