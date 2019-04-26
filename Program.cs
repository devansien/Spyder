using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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

            // begining, check how many words a person knows
            string targetWord = "z"; // a to z & a words to z words (commonly used, 100 each?)
            string targetPage = "1"; // max 100 if the usage example has over 1980 entries

            // should be in a loop
            HttpResponseMessage response = await client.GetAsync($"https://endic.naver.com/search_example.nhn?sLn=en&examType=example&query={targetWord}&pageNo={targetPage}&ui=lite");
            string contents = await response.Content.ReadAsStringAsync();

            // write to two files one for word and count, how many exist, how many read since its maxed

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(contents);

            string egCount = document.DocumentNode.SelectSingleNode("(//div[contains(@class, 'word_num_nobor')]//span[contains(@class, 'fnt_k03')])[1]").InnerText;
            //HtmlNode[] nodes = pageDocument.DocumentNode.SelectNodes("(//ul[contains(@class, 'list_a list_a_mar')]//input[contains(@type, 'hidden')])").ToArray();

            //foreach (HtmlNode node in nodes)
            //{
            //    string targetText = node.GetAttributeValue("value", "");
            //    Console.WriteLine(targetText);
            //}

            // max page will be 100
            // max 20 entries per page
            // a to z then commonly used words a from z 


            //using (MemoryStream memoryStream = new MemoryStream(targetText.))
            //using (FileStream fileStream = new FileStream("b2.txt", FileMode.Create, FileAccess.Write))
            //{
            //    memoryStream.CopyTo(fileStream);
            //}
            egCount = Regex.Replace(egCount, "[^.0-9]", "");
            int pageCount = (int.Parse(egCount) / 20);
            if (pageCount > 100)
            {
                pageCount = 100;
            }



            string[] lines = File.ReadAllLines("words.txt");
            List<string> newLines = new List<string>();

            int maxLines = lines.Length / 30;

            for (int i = 0; i < maxLines; i++)
            {
                StringBuilder builder = new StringBuilder();
                for (int j = 0; j < 30; j++)
                {
                    string word = $"\"{lines[j + i * 30]}\", ";
                    builder.Append(word);
                }
                //builder.AppendLine();

                newLines.Add(builder.ToString());
            }


            TextWriter tw = new StreamWriter("saveList.txt");
            foreach(string s in newLines)
            {
                tw.WriteLine(s);
            }
            tw.Close();
            Console.ReadLine();
        }
    }
}
