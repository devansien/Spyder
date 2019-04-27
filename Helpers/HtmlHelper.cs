using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Spyder
{
    class HtmlHelper
    {
        public async static Task<HtmlDocument> LoadSiteAsync(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document =await web.LoadFromWebAsync(url);

            return document;
        }
    }
}
