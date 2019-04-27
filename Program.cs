using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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
            string url = string.Empty;
            //HttpClient client = new HttpClient();
            List<string> words = Words.GetAll();        // native speaker knows 20,000 words actively, 40,000 words passively

            for (int i = 0; i < words.Count; i++)
            {
                string targetWord = words[i];       // a to z & a words to z words (commonly used)
                string initialPage = "1";        // max 100 if example count is larger than 1980

                url = UrlHelper.GetUrl(targetWord, initialPage);
                HtmlDocument document = await HtmlHelper.LoadSiteAsync(url);

                int exampleCount = PageHelper.GetExampleCount(document);
                int pageCount = PageHelper.GetPageCount(exampleCount);

                Word word = new Word
                {
                    Index = i + 1,
                    Url = url,
                    Phrase = targetWord,
                    PageCount = pageCount,
                    ExampleCount = exampleCount,
                    EstimatedCount = pageCount * 20     // 20 = max words per page
                };
                using (StreamWriter logWriter = File.AppendText("log.txt"))
                {
                    // write into an excel file if possible
                    string wordInfo = $"[Access Time: {DateTime.UtcNow}][Index: {word.Index}], [Word: {word.Phrase}], [Usage Examples: {word.ExampleCount}], " +
                        $"[Pages Visited: {word.PageCount}], [Collected (est.): {word.EstimatedCount}], [Link: {word.Url}]";

                    logWriter.WriteLine(wordInfo);
                }

                using (TextWriter writer = new StreamWriter($"words/{word.Phrase}.txt"))
                {
                    for (int j = 1; j < pageCount + 1; j++)
                    {
                        url = UrlHelper.GetUrl(targetWord, j.ToString());
                        document = await HtmlHelper.LoadSiteAsync(url);

                        Dictionary<string, string> sentences = PageHelper.GetSentences(document);

                        if (sentences != null)
                        {
                            foreach (KeyValuePair<string, string> kvp in sentences)
                            {
                                Console.WriteLine($"{kvp.Key}\t\t{kvp.Value}");
                                writer.WriteLine($"{kvp.Key}\t\t{kvp.Value}");
                            }
                        }
                    }
                }
            }


            // write to two files one for word and count, how many exist, how many read since its maxed

            //TextWriter tw = new StreamWriter("saveList.txt");
            //foreach (string s in newLines)
            //{
            //    tw.WriteLine(s);
            //}
            //tw.Close();
            Console.WriteLine("Process Done.");
            Console.ReadLine();
        }
    }
}
