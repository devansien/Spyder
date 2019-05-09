using System;
using System.IO;
using System.Threading.Tasks;

namespace Spyder
{
    class Program
    {
        //static async Task Main()
        //{

        //}

        // or

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        async static Task MainAsync(string[] args)
        {
            Console.WriteLine($"Process start: {DateTime.UtcNow}");

            //IExtractor extractor = new NaverExtractor();
            //await extractor.ExtractAsync();

            // write a text preproccess method to stringfy the word list

            //// gernerate corpora for training from collected text files
            //IProcessor processor = new NaverProcessor();
            //await processor.ProcessAsync();

            await MergeCorporaToText();

            Console.WriteLine($"Process done: {DateTime.UtcNow}");
            Console.ReadLine();
        }

        async static Task MergeCorporaToText()
        {
            string[] engLines = await File.ReadAllLinesAsync("corpus.ansien.eng");
            string[] korLines = await File.ReadAllLinesAsync("corpus.ansien.kor");

            StreamWriter enko = File.AppendText("enko.txt");

            for (int i = 0; i < engLines.Length; i++)
                await enko.WriteLineAsync($"{engLines[i]}\t{korLines[i]}");

            enko.Close();
        }
    }
}
