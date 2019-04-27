namespace Spyder
{
    class UrlHelper
    {
        public static string GetUrl(string word, string page)
        {
            return $"https://endic.naver.com/search_example.nhn?sLn=en&examType=example&query={word}&pageNo={page}&ui=lite";
        }
    }
}
