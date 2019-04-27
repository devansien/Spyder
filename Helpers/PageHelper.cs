using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spyder
{
    class PageHelper
    {
        private const int MaxWordsPerPage = 20;
        private const int MaxPagesPerWord = 100;

        public static int GetExampleCount(HtmlDocument document)
        {
            string innerText = document.DocumentNode.SelectSingleNode(Nodes.UsageExampleCount).InnerText;
            innerText = Regex.Replace(innerText, "[^.0-9]", "");

            int exampleCount = 0;
            if (innerText != null)
                exampleCount = int.Parse(innerText);        // better use try parse?

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
            Dictionary<string, string> sentences = new Dictionary<string, string>();

            HtmlNode[] koreanNodes = null;
            HtmlNode[] englishNodes = null;
            HtmlNodeCollection koreanNodeCollection = document.DocumentNode.SelectNodes(Nodes.KoreanSentence);
            HtmlNodeCollection englishNodeCollection = document.DocumentNode.SelectNodes(Nodes.EnglishSentence);

            if (koreanNodeCollection != null)
                koreanNodes = document.DocumentNode.SelectNodes(Nodes.KoreanSentence).ToArray();
            if (englishNodeCollection != null)
                englishNodes = document.DocumentNode.SelectNodes(Nodes.EnglishSentence).ToArray();


            if (koreanNodeCollection != null && englishNodeCollection != null)
            {
                for (int i = 0; i < koreanNodes.Length; i++)
                {
                    string koreanSentence = koreanNodes[i].InnerText;
                    string englishSentence = englishNodes[i].GetAttributeValue("value", "");

               
                    englishSentence = Regex.Replace(englishSentence, @"\[[^\[\]]+\]", "");
                    englishSentence = Regex.Replace(englishSentence, @"\([^\[\]]+\)", "");
                    englishSentence = englishSentence.Replace("&quot;", "");
                    englishSentence = englishSentence.Replace("&amp;", "");
                    koreanSentence = koreanSentence.Replace("&quot;", "");
                    koreanSentence = koreanSentence.Replace("&amp;", "");

                    int index = englishSentence.IndexOf("=");
                    if (index > 0)
                        englishSentence = englishSentence.Substring(0, index);
                    koreanSentence = Regex.Replace(koreanSentence, @" ?\(.*?\)", "");
                    koreanSentence = Regex.Replace(koreanSentence, @"\<[^\[\]]+\>", "");
                    koreanSentence = Regex.Replace(koreanSentence, @"\[[^\[\]]+\]", "");
                    koreanSentence = Regex.Replace(koreanSentence, @"\[[^\[\]]+\〕", "");

                    englishSentence = Regex.Replace(englishSentence, @"[^\w\s,?!]", "");
                    koreanSentence = Regex.Replace(koreanSentence, @"[^\w\s,?!]", "");

                 

                    int kindex = koreanSentence.IndexOf("=");
                    if (kindex > 0)
                        koreanSentence = koreanSentence.Substring(0, kindex);

                    englishSentence.ToLower();

                    if (!sentences.ContainsKey(englishSentence))
                    {
                        sentences.Add(englishSentence, koreanSentence);
                    }
                }

                return sentences;
            }
            else
                return null;

        }
    }
}
