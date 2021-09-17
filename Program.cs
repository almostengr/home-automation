using System;
using System.Collections.Generic;
using System.IO;

namespace Almostengr.RhtServices.TranscriptCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            string TRANSCRIPT_FILE_PATH = args[0];

            try
            {
                if (TRANSCRIPT_FILE_PATH == string.Empty || TRANSCRIPT_FILE_PATH == null)
                {
                    throw new ArgumentException("Transcript file path not provided");
                }

                string line = string.Empty;
                List<string> lines = new List<string>();

                // read the file

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
            
                TRANSCRIPT_FILE_PATH = TRANSCRIPT_FILE_PATH.Replace(".sbv", string.Empty);

                // write transcript

                string OUTPUT_FILE_PATH = TRANSCRIPT_FILE_PATH + ".youtube.sbv";
                StreamWriter outputFile = new System.IO.StreamWriter(OUTPUT_FILE_PATH);
                foreach (string lineItem in lines)
                {
                    outputFile.WriteLine(lineItem);
                }
                outputFile.Close();

                Console.WriteLine("Transcript written to: " + OUTPUT_FILE_PATH);

                // write blog post
                
                OUTPUT_FILE_PATH = TRANSCRIPT_FILE_PATH + ".md";
                outputFile = new System.IO.StreamWriter(OUTPUT_FILE_PATH);
                foreach (string lineItem in lines)
                {
                    if (lineItem.StartsWith("0:") || lineItem == string.Empty)
                    {
                        continue;
                    }

                    outputFile.WriteLine(lineItem);
                }
                outputFile.Close();

                Console.WriteLine("Blog post written to: " + OUTPUT_FILE_PATH);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Application Usage");
            Console.WriteLine(string.Empty);
            Console.WriteLine("Usage: transcriptcleaner <transcript file>");
            Console.WriteLine(string.Empty);
            Console.WriteLine("Example: transcriptcleaner C:\\Users\\almostengr\\Desktop\\transcript.sbv");
        }
    }
}
