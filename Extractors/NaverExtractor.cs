using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spyder
{
    class NaverExtractor
    {
        private const int MaxWordsPerPage = 20;
        private const int MaxPagesPerWord = 100;


        public static string GetUrl(string word, string page)
        {
            return $"https://endic.naver.com/search_example.nhn?sLn=en&examType=example&query={word}&pageNo={page}&ui=lite";
        }

        public static int GetExampleCount(HtmlDocument document)
        {
            string innerText = document.DocumentNode.SelectSingleNode(NaverNode.UsageExampleCount).InnerText;
            innerText = Regex.Replace(innerText, "[^.0-9]", "");

            int exampleCount = 0;
            if (innerText != null)
                exampleCount = int.Parse(innerText);

            return exampleCount;
        }

        public static int GetPageCount(int exampleCount)
        {
            int pageCount = exampleCount % MaxWordsPerPage == 0 ? exampleCount / MaxWordsPerPage : (exampleCount / MaxWordsPerPage) + 1;

            if (pageCount > MaxPagesPerWord)
                pageCount = MaxPagesPerWord;

            return pageCount;
        }

        public static Dictionary<string, string> GetSentences(HtmlDocument document)
        {

            HtmlNodeCollection korCollection = document.DocumentNode.SelectNodes(NaverNode.KoreanSentence);
            HtmlNodeCollection engCollection = document.DocumentNode.SelectNodes(NaverNode.EnglishSentence);

            return GetSentences(document, korCollection, engCollection);
        }

        private static Dictionary<string, string> GetSentences(HtmlDocument document, HtmlNodeCollection korCollection, HtmlNodeCollection engCollection)
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
