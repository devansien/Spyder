using System;
using System.IO;
using System.Threading.Tasks;

namespace Spyder
{
    class NaverProcessor : IProcessor
    {
        private const string korFolder = "/Sentences/Korean";
        private const string engFolder = "/Sentences/English";

        private const string korCorpus = "Corpora/corpus.ansien.kor";
        private const string engCorpus = "Corpora/corpus.ansien.eng";


        public async Task ProcessAsync()
        {
            await CreateCorporaAsync();
        }

        private async Task CreateCorporaAsync()
        {
            string[] korFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + korFolder);
            string[] engFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + engFolder);

            Console.WriteLine($"Total number of Korean text files: [{korFilePaths.Length}]");
            Console.WriteLine($"Total number of English text files: [{engFilePaths.Length}]");

            int totalLineCount = 0;
            int fileCount = GetFileCount(korFilePaths.Length, engFilePaths.Length);

            await WriteToCorpusAsync(fileCount, totalLineCount, korFilePaths, engFilePaths);
        }

        private int GetFileCount(int korFileCount, int engFileCount)
        {
            return korFileCount == engFileCount ? korFileCount : 0;
        }

        private async Task WriteToCorpusAsync(int fileCount, int lineCount, string[] korPaths, string[] engPaths)
        {
            StreamWriter korWriter = File.AppendText(korCorpus);
            StreamWriter engWriter = File.AppendText(engCorpus);

            for (int i = 0; i < fileCount; i++)
            {
                string[] korLines = await File.ReadAllLinesAsync(korPaths[i]);
                string[] engLines = await File.ReadAllLinesAsync(engPaths[i]);

                if (engLines.Length == korLines.Length)
                {
                    for (int j = 0; j < engLines.Length; j++)
                    {
                        if (!string.IsNullOrWhiteSpace(engLines[j]) && !string.IsNullOrWhiteSpace(korLines[j]))
                        {
                            await engWriter.WriteLineAsync(engLines[j]);
                            await korWriter.WriteLineAsync(korLines[j]);

                            lineCount++;
                        }
                    }
                }
                else
                    Console.WriteLine($"Lines not matching for: {engPaths[i]}");
            }

            Console.WriteLine($"Total lines written: [{lineCount}]");

            engWriter.Close();
            korWriter.Close();
        }
    }
}
