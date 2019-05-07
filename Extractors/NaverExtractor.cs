using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
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

            // should create senteces/english & sentences/korean paths
            for (int i = 0; i < words.Count; i++)
            {
                if (!File.Exists($"sentences/english/eng_{words[i]}.txt"))
                {
                    string initialPage = "1";        // max 100 if example count is larger than 1980
                    string targetWord = words[i];       // a to z (no i) & a words to z words (commonly used)

                    url = GetUrl(targetWord, initialPage);
                    HtmlDocument document = await HtmlLoader.LoadSiteAsync(url);
                    StreamWriter logWriter = File.AppendText("logs.txt");

                    if (document != null)
                    {
                        int exampleCount = GetExampleCount(document);
                        int pageCount = GetPageCount(exampleCount);

                        string wordInfo = $"[Index: {(i + 1).ToString().PadLeft(4, '0')}], [Access Time: {DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}], ";
                        await logWriter.WriteAsync(wordInfo);

                        int collectCount = 0;
                        int actualPageCount = 0;

                        TextWriter korWriter = new StreamWriter($"sentences/korean/kor_{targetWord}.txt");
                        TextWriter engWriter = new StreamWriter($"sentences/english/eng_{targetWord}.txt");

                        for (int j = 1; j < pageCount + 1; j++)
                        {
                            actualPageCount++;
                            url = GetUrl(targetWord, j.ToString());
                            document = await HtmlLoader.LoadSiteAsync(url);
                            Dictionary<string, string> sentences = GetSentences(document);

                            if (sentences != null)
                            {
                                foreach (KeyValuePair<string, string> kvp in sentences)
                                {
                                    collectCount++;
                                    engWriter.WriteLine($"{kvp.Key}");
                                    korWriter.WriteLine($"{kvp.Value}");
                                    Console.WriteLine($"{kvp.Key}\t\t{kvp.Value}");
                                }
                            }
                        }

                        await logWriter.WriteLineAsync($"[Complete Time: {DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}], [Word: {targetWord}], [Usage Examples: {exampleCount}], [Pages Visited: {actualPageCount}], [Sentences Collected: {collectCount}], [Link: {url}]");

                        engWriter.Close();
                        korWriter.Close();
                        logWriter.Close();
                    }
                }
            }
        }

        private string GetUrl(string word, string page)
        {
            return $"https://endic.naver.com/search_example.nhn?sLn=en&examType=example&query={word}&pageNo={page}&ui=lite";
        }

        private int GetExampleCount(HtmlDocument document)
        {
            string innerText = document.DocumentNode.SelectSingleNode(NaverNode.UsageExampleCount).InnerText;
            innerText = Regex.Replace(innerText, "[^.0-9]", "");

            int exampleCount = 0;
            if (innerText != null)
                exampleCount = int.Parse(innerText);

            return exampleCount;
        }

        private int GetPageCount(int exampleCount)
        {
            int pageCount = exampleCount % MaxWordsPerPage == 0 ? exampleCount / MaxWordsPerPage : (exampleCount / MaxWordsPerPage) + 1;

            if (pageCount > MaxPagesPerWord)
                pageCount = MaxPagesPerWord;

            return pageCount;
        }

        private Dictionary<string, string> GetSentences(HtmlDocument document)
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
