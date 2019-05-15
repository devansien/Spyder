using System;
using System.IO;
using System.Threading.Tasks;

namespace Spyder
{
    class DataBuilder
    {
        public async static Task MergeCorporaToText()
        {
            string[] engLines = await File.ReadAllLinesAsync("corpus.ansien.eng");
            string[] korLines = await File.ReadAllLinesAsync("corpus.ansien.kor");

            StreamWriter enko = File.AppendText("enko.txt");

            for (int i = 0; i < engLines.Length; i++)
                await enko.WriteLineAsync($"{engLines[i]}\t{korLines[i]}");

            enko.Close();
        }

        async static Task SplitTextToCorpora()
        {
            string[] textLines = await File.ReadAllLinesAsync("engkor.txt");

            StreamWriter engWriter = File.AppendText("corpus.ansien.eng");
            StreamWriter korWriter = File.AppendText("corpus.ansien.kor");

            for (int i = 0; i < textLines.Length; i++)
            {
                string[] textLine = textLines[i].Split('\t');

                if (textLine.Length.Equals(2))
                {
                    await engWriter.WriteLineAsync(textLine[0]);
                    await korWriter.WriteLineAsync(textLine[1]);
                }
                else
                    Console.WriteLine($"Splitting text failed on line: [{i}]");
            }

            engWriter.Close();
            korWriter.Close();
        }

        public async static Task MakeValidationSet()
        {
            int maxLineNum = 3000;
            string[] textLines = await File.ReadAllLinesAsync("engkor.txt");

            Randomize(textLines);

            StreamWriter engWriter = File.AppendText("val.ansien.eng");
            StreamWriter korWriter = File.AppendText("val.ansien.kor");

            for (int i = 0; i < maxLineNum; i++)
            {
                string[] textLine = textLines[i].Split('\t');

                if (textLine.Length.Equals(2))
                {
                    await engWriter.WriteLineAsync(textLine[0]);
                    await korWriter.WriteLineAsync(textLine[1]);
                }
                else
                    Console.WriteLine($"Splitting text failed on line: [{i}]");
            }

            engWriter.Close();
            korWriter.Close();
        }

        async static Task MakeOpenNmtDataset()
        {
            int maxTrainLineNum = 10000;
            int maxValLineNum = 1000;
            int maxTestLineNum = 1000;

            string[] textLines = await File.ReadAllLinesAsync("engkor.txt");

            Randomize(textLines);

            StreamWriter engTrainWriter = File.AppendText("src-train.txt");
            StreamWriter korTrainWriter = File.AppendText("tgt-train.txt");

            StreamWriter engValWriter = File.AppendText("src-val.txt");
            StreamWriter korValWriter = File.AppendText("tgt-val.txt");

            StreamWriter engTestWriter = File.AppendText("src-test.txt");
            StreamWriter korTestWriter = File.AppendText("tgt-test.txt");

            for (int i = 0; i < maxTrainLineNum; i++)
            {
                string[] textLine = textLines[i].Split('\t');

                if (textLine.Length.Equals(2))
                {
                    await engTrainWriter.WriteLineAsync(textLine[0]);
                    await korTrainWriter.WriteLineAsync(textLine[1]);
                }
                else
                    Console.WriteLine($"Splitting text failed on line: [{i}]");
            }

            for (int i = 0; i < maxValLineNum; i++)
            {
                string[] textLine = textLines[i].Split('\t');

                if (textLine.Length.Equals(2))
                {
                    await engValWriter.WriteLineAsync(textLine[0]);
                    await korValWriter.WriteLineAsync(textLine[1]);
                }
                else
                    Console.WriteLine($"Splitting text failed on line: [{i}]");
            }

            for (int i = maxValLineNum; i < maxTestLineNum * 2; i++)
            {
                string[] textLine = textLines[i].Split('\t');

                if (textLine.Length.Equals(2))
                {
                    await engTestWriter.WriteLineAsync(textLine[0]);
                    await korTestWriter.WriteLineAsync(textLine[1]);
                }
                else
                    Console.WriteLine($"Splitting text failed on line: [{i}]");
            }

            engTrainWriter.Close();
            korTrainWriter.Close();

            engValWriter.Close();
            korValWriter.Close();

            engTestWriter.Close();
            korTestWriter.Close();
        }

        static void Randomize<T>(T[] items)
        {
            Random rand = new Random();

            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
    }
}
