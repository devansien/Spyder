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

            // 1. (write a text preproccess method to stringfy the word list)

            // 2. extract usage examples
            IExtractor extractor = new NaverExtractor();
            await extractor.ExtractAsync();

            // 3. gernerate corpora for training from collected text files
            //IProcessor processor = new NaverProcessor();
            //await processor.ProcessAsync();

            // 4. merge them into a single tsv file if needed
            //await MergeCorporaToText();

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
