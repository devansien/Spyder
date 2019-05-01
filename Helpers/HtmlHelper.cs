using HtmlAgilityPack;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder
{
    class HtmlHelper
    {
        const int maxRetry = 10;

        public async static Task<HtmlDocument> LoadSiteAsync(string url)
        {
            int retryCount = maxRetry;
            bool success = false;
            HtmlDocument document = new HtmlDocument();
            while (!success && retryCount > 0)
            {
                try
                {
                    HtmlWeb web = new HtmlWeb();
                    document = await web.LoadFromWebAsync(url);
                    success = true;
                }
                catch (Exception ex)
                {
                    retryCount--;

                    using (StreamWriter logWriter = File.AppendText("logs.txt"))
                    {
                        string errorInfo = $"Error: [{ex.InnerException}] @ [{url}], [{ex.StackTrace}]";

                        logWriter.WriteLine(errorInfo);
                    }
                    Thread.Sleep(1000);
                    if (retryCount == 0)
                        throw;

                }
            }

            return document;
        }
    }
}
