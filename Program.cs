using HtmlAgilityPack;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spyder
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        async static Task MainAsync(string[] args)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://endic.naver.com/search_example.nhn?sLn=en&examType=example&query=b&pageNo=2&ui=lite");
            string pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            string targetText = pageDocument.DocumentNode.SelectSingleNode("(//ul[contains(@class, 'list_a list_a_mar')]//input[contains(@type, 'hidden')])").GetAttributeValue("value", "");






            //using (MemoryStream memoryStream = new MemoryStream(targetText.))
            //using (FileStream fileStream = new FileStream("b2.txt", FileMode.Create, FileAccess.Write))
            //{
            //    memoryStream.CopyTo(fileStream);
            //}

            Console.WriteLine(targetText);
            Console.ReadLine();
        }
    }
}
