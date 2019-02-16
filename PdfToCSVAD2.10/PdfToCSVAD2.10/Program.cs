using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfToCSVAD210
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage is app.exe source csv_dest");
                return;
            }
            string sourceFileName = args[0];
            List<List<string>> obstacles = GetObstacleList(sourceFileName);
            DisplayObstacles(obstacles);
            string destFileName = args[1];
            WriteToCSV(destFileName, obstacles);
        }

        private static void DisplayObstacles(List<List<string>> obstacles)
        {
            foreach (List<string> list_data in obstacles)
            {
                Obstacle obstacle = new Obstacle(list_data);
                // Display CSV Row Value of Obstacle
                Console.WriteLine(obstacle);
            }
        }

        public static void WriteToCSV(string destination, List<List<string>> obstacles)
        {
            using (var w = new StreamWriter(destination))
            {
                // Add CSV Header to Make it Easier to Understand
                // What Each Column means
                w.WriteLine("RunwayAreaAffected,ObstacleType, Latitude, Longitude,Elevation,Marking,Remarks");
                foreach(var obstacle in obstacles)
                {
                    // Write A Comma Separated Row to Document
                    w.WriteLine(new Obstacle(obstacle));
                    w.Flush();
                }
            }
        }

        public static List<List<string>> GetObstacleList(string file_path)
        {
            PdfReader reader = new PdfReader(file_path);
            TableExtractionStrategy strategy = new TableExtractionStrategy
            {
                // Modify this value to take care of counts
                NextLineLookAheadDepth = 300
            };

            // Setting this to true will add the required value
            bool add = false;
            // Iterate through all the pages
            for (int i = 1; i < reader.NumberOfPages; i++)
            {
                // Extract the Page Data According to the pre-decided Strategy
                string page = PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                // As this contains AD 2.10 in Range
                // Start Adding
                if (!add/*If add is true, it's already found*/ && page.Contains("AD 2.10"))
                {
                    add = true;
                }
                //As we were Supposed to Extract only
                // AD2.10 and not anything else
                // Our work here is done
                // This is Because
                // AD2.11 Comes after AD2.10
                if (add/*If add is false, no need to find this*/ && page.Contains("AD 2.11"))
                {
                    break;
                }
                // If Add is Disabled Even now
                // It means we have Not Reached AD 2.10 Stage
                // So Clear Strategy 
                if (!add)
                {
                    strategy.Chunks.Clear();
                }
            }
            List<List<string>> tables = strategy.GetTable();
            // Note that we find that every Obstacle Table Element has a count of 6
            // Any Table Element with lesser count is to be removed
            // The Headings also contain 6 Elements
            // Remove this by Passing
            // RWY/Area affected which is the value of the first heading
            tables.RemoveAll(x => x.Count != 6 || x[0].StartsWith("RWY/Area affected"));
            return tables;
        }
    }
}
