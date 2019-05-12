using System;
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
            //

            // 2. extract usage examples
            IExtractor extractor = new NaverExtractor();
            await extractor.ExtractAsync();

            // 3. gernerate corpora for training from collected text files
            //IProcessor processor = new NaverProcessor();
            //await processor.ProcessAsync();

            // 4. merge them into a single tsv file if needed
            //await DataBuilder.MergeCorporaToText();

            // 5. split merged text to corpora again if needed
            //await DataBuilder.SplitTextToCorpora();

            // 6. make a validation set
            //await DataBuilder.MakeValidationSet();

            // 7. make open nmt compatible dataset
            //await DataBuilder.MakeOpenNmtDataset();

            Console.WriteLine($"Process done: {DateTime.UtcNow}");
            Console.ReadLine();
        }
    }
}
