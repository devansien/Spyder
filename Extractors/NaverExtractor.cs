using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spyder
{
    class NaverExtractor : IExtractor
    {
        private const int MaxWordsPerPage = 20;
        private const int MaxPagesPerWord = 100;


        public async Task ExtractAsync()
        {
            string url = string.Empty;
            List<string> words = Words.GetAll();

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

        }

        public string GetUrl(string word, string page)
        {
            return $"https://endic.naver.com/search_example.nhn?sLn=en&examType=example&query={word}&pageNo={page}&ui=lite";
        }

        public int GetExampleCount(HtmlDocument document)
        {
            string innerText = document.DocumentNode.SelectSingleNode(NaverNode.UsageExampleCount).InnerText;
            innerText = Regex.Replace(innerText, "[^.0-9]", "");

            int exampleCount = 0;
            if (innerText != null)
                exampleCount = int.Parse(innerText);

            return exampleCount;
        }

        public int GetPageCount(int exampleCount)
        {
            int pageCount = exampleCount % MaxWordsPerPage == 0 ? exampleCount / MaxWordsPerPage : (exampleCount / MaxWordsPerPage) + 1;

            if (pageCount > MaxPagesPerWord)
                pageCount = MaxPagesPerWord;

            return pageCount;
        }

        public Dictionary<string, string> GetSentences(HtmlDocument document)
        {

            HtmlNodeCollection korCollection = document.DocumentNode.SelectNodes(NaverNode.KoreanSentence);
            HtmlNodeCollection engCollection = document.DocumentNode.SelectNodes(NaverNode.EnglishSentence);

            return GetSentences(document, korCollection, engCollection);
        }

        private Dictionary<string, string> GetSentences(HtmlDocument document, HtmlNodeCollection korCollection, HtmlNodeCollection engCollection)
        {
            Dictionary<string, string> sentences = new Dictionary<string, string>();

            if (korCollection != null && engCollection != null)
            {
                HtmlNode[] koreanNodes = document.DocumentNode.SelectNodes(NaverNode.KoreanSentence).ToArray();
                HtmlNode[] englishNodes = document.DocumentNode.SelectNodes(NaverNode.EnglishSentence).ToArray();

                if (koreanNodes.Count() == englishNodes.Count())
                {
                    for (int i = 0; i < koreanNodes.Length; i++)
                    {
                        string koreanSentence = koreanNodes[i].InnerText;
                        string englishSentence = englishNodes[i].GetAttributeValue("value", "");

                        if (!string.IsNullOrWhiteSpace(koreanSentence) && !string.IsNullOrWhiteSpace(englishSentence))
                        {
                            int targetIndex = englishSentence.IndexOf("=");
                            if (targetIndex > 0)
                                englishSentence = englishSentence.Substring(0, targetIndex);

                            targetIndex = koreanSentence.IndexOf("=");
                            if (targetIndex > 0)
                                koreanSentence = koreanSentence.Substring(0, targetIndex);

                            englishSentence = Regex.Replace(englishSentence, NaverNode.RegexPattern, string.Empty);
                            koreanSentence = Regex.Replace(koreanSentence, NaverNode.RegexPattern, string.Empty);

                            if (!sentences.ContainsKey(englishSentence))
                                sentences.Add(englishSentence, koreanSentence);
                        }
                    }

                    return sentences;
                }
            }

            return null;
        }
    }
}
