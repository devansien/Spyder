using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spyder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        async static Task MainAsync(string[] args)
        {

            Console.WriteLine($"Process start: {DateTime.UtcNow}");

            string[] engFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Sentences/English");
            string[] korFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Sentences/Korean");

            Console.WriteLine($"Total English files: {engFilePaths.Length}");
            Console.WriteLine($"Total Korean files: {korFilePaths.Length}");

            int fileCount = engFilePaths.Length == korFilePaths.Length ? engFilePaths.Length : 0;
            int engTotalLineCount = 0;
            int korTotalLineCount = 0;

            StreamWriter engWriter = File.AppendText("corpus.ansien.eng");
            StreamWriter korWriter = File.AppendText("corpus.ansien.kor");

            for (int i = 0; i < fileCount; i++)
            {
                string[] engLines = File.ReadLines(engFilePaths[i]).ToArray();
                string[] korLines = File.ReadLines(korFilePaths[i]).ToArray();

                if (engLines.Length == korLines.Length)
                {
                    for (int j = 0; j < engLines.Length; j++)
                    {
                        if (!string.IsNullOrWhiteSpace(engLines[j]) && !string.IsNullOrWhiteSpace(korLines[j]))
                        {
                            await engWriter.WriteLineAsync(engLines[j]);
                            await korWriter.WriteLineAsync(korLines[j]);

                            engTotalLineCount++;
                        }

                        korTotalLineCount++;
                    }
                }
                else
                    Console.WriteLine(engFilePaths[i]); // log if number of collected strings are not matching
            }

            engWriter.Close();
            korWriter.Close();

            Console.WriteLine($"Total English lines: {engTotalLineCount}");
            Console.WriteLine($"Total Korean lines: {korTotalLineCount}");


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






            //string url = string.Empty;
            //List<string> words = Words.GetAll();

            //for (int i = 0; i < words.Count; i++)
            //{
            //    if (!File.Exists($"sentences/english/eng_{words[i]}.txt"))
            //    {
            //        string targetWord = words[i];       // a to z & a words to z words (commonly used)
            //        string initialPage = "1";        // max 100 if example count is larger than 1980

            //        url = UrlHelper.GetUrl(targetWord, initialPage);
            //        HtmlDocument document = await HtmlHelper.LoadSiteAsync(url);

            //        if (document != null)
            //        {
            //            int exampleCount = PageHelper.GetExampleCount(document);
            //            int pageCount = PageHelper.GetPageCount(exampleCount);

            //            Word word = new Word
            //            {
            //                Index = i + 1,
            //                Url = url,
            //                Phrase = targetWord,
            //                PageCount = pageCount,
            //                ExampleCount = exampleCount,
            //                EstimatedCount = pageCount * 20     // 20 = max words per page
            //            };

            //            using (StreamWriter logWriter = File.AppendText("logs.txt"))
            //            {
            //                string wordInfo = $"[Index: {word.Index.ToString().PadLeft(4, '0')}], [Access Time: {DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}], ";
            //                await logWriter.WriteAsync(wordInfo);
            //            }


            //            int collectCount = 0;
            //            int actPageCount = 0;

            //            TextWriter engWriter = new StreamWriter($"sentences/english/eng_{word.Phrase}.txt");
            //            TextWriter korWriter = new StreamWriter($"sentences/korean/kor_{word.Phrase}.txt");

            //            // if file exist go for next should be implemented

            //            for (int j = 1; j < pageCount + 1; j++)
            //            {
            //                url = UrlHelper.GetUrl(targetWord, j.ToString());
            //                document = await HtmlHelper.LoadSiteAsync(url);

            //                Dictionary<string, string> sentences = PageHelper.GetSentences(document);

            //                actPageCount++;
            //                if (sentences != null)
            //                {
            //                    foreach (KeyValuePair<string, string> kvp in sentences)
            //                    {
            //                        collectCount++;
            //                        Console.WriteLine($"{kvp.Key}\t\t{kvp.Value}");
            //                        engWriter.WriteLine($"{kvp.Key}");
            //                        korWriter.WriteLine($"{kvp.Value}");
            //                    }
            //                }
            //            }

            //            // write into an excel file if possible

            //            using (StreamWriter logWriter = File.AppendText("logs.txt"))
            //            {
            //                string wordInfo = $"[Complete Time: {DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}], [Word: {word.Phrase}], [Usage Examples: {word.ExampleCount}], [Pages Visited: {actPageCount}], [Sentences Collected: {collectCount}], [Link: {word.Url}]";

            //                await logWriter.WriteLineAsync(wordInfo);
            //            }


            //            engWriter.Close();
            //            korWriter.Close();
            //        }
            //    }
            //}

















            // write to two files one for word and count, how many exist, how many read since its maxed

            //TextWriter tw = new StreamWriter("saveList.txt");
            //foreach (string s in newLines)
            //{
            //    tw.WriteLine(s);
            //}
            //tw.Close();
            Console.WriteLine($"Process Done: {DateTime.UtcNow}");
            Console.ReadLine();
        }
    }
}
