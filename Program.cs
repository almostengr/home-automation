using System;
using System.Collections.Generic;
using System.IO;

namespace Almostengr.RhtServices.TranscriptCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string transcriptFilePath = args[0];

                if (transcriptFilePath == string.Empty || transcriptFilePath == null)
                {
                    DisplayHelp();
                    throw new ArgumentNullException("Transcript file path not provided or does not exist");
                }

                List<string> lines = ReadTranscriptContents(transcriptFilePath);

                WriteOutputContent(transcriptFilePath, lines, OutputExtension.Transcript);
                WriteOutputContent(transcriptFilePath, lines, OutputExtension.Markdown);
                WriteOutputContent(transcriptFilePath, lines, OutputExtension.Facebook);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
            }
        }

        private static void WriteOutputContent(string transcriptFilePath, List<string> lines, string extension)
        {
            transcriptFilePath = transcriptFilePath.Replace(".srt", string.Empty);

            string outputFilePath = transcriptFilePath + "." + extension;
            StreamWriter outputFile = new System.IO.StreamWriter(outputFilePath);
            int counter = 0;

            foreach (string lineItem in lines)
            {
                counter = counter >= 4 ? 1 : counter + 1;

                if (extension == OutputExtension.Markdown && counter != 3)
                {
                    continue;
                }

                outputFile.WriteLine(lineItem);
            }

            outputFile.Close();

            Console.WriteLine("Output written to: " + outputFilePath);
        }

        private static List<string> ReadTranscriptContents(string TRANSCRIPT_FILE_PATH)
        {
            string line = string.Empty;
            List<string> lines = new List<string>();

            StreamReader file = new System.IO.StreamReader(TRANSCRIPT_FILE_PATH);
            while ((line = file.ReadLine()) != null)
            {
                line = line
                    .ToUpper()
                    .Replace("UM", string.Empty)
                    .Replace("UH", string.Empty)
                    .Replace("  ", " ")
                    ;

                lines.Add(line);
            }

            file.Close();
            return lines;
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Application Usage");
            Console.WriteLine(string.Empty);
            Console.WriteLine("Usage: transcriptcleaner <transcript file>");
            Console.WriteLine(string.Empty);
            Console.WriteLine("Example: transcriptcleaner C:\\Users\\almostengr\\Desktop\\transcript.sbv");
        }
    }

    public static class OutputExtension
    {
        public static readonly string Markdown = "md";
        public static readonly string Transcript = "yt.srt";
        public static readonly string Facebook = "fb.en_US.srt";
    }
}
