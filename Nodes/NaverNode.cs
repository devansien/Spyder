namespace Spyder
{
    class NaverNode
    {
        public const string RegexPattern = @"&quot;|&amp;|\[(.*?)\]|\((.*?)\)|\<(.*?)\>|\((.*?)\)|\[(.*?)\〕|[^\w\s,'?!=]";
        public const string KoreanSentence = "(//ul[contains(@class, 'list_a list_a_mar')]//div[contains(@class, 'fnt_k10')]//a[contains(@class, 'N=a:xmp.detail')])";
        public const string EnglishSentence = "(//ul[contains(@class, 'list_a list_a_mar')]//input[contains(@type, 'hidden')])";
        public const string UsageExampleCount = "(//div[contains(@class, 'word_num_nobor')]//span[contains(@class, 'fnt_k03')])[1]";
    }
}
