using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace PdfToCSVAD210
{
    public class Program
    {
        private static void Main(string[] args)
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
                //Display CSV Row Value of Obstacle
                Console.WriteLine(obstacle + " : ");
            }
        }

        public static void WriteToCSV(string destination, List<List<string>> obstacles)
        {
            using (StreamWriter w = new StreamWriter(destination))
            {
                // Add CSV Header to Make it Easier to Understand
                // What Each Column means
                w.WriteLine("Runway Area Affected,Obstacle Type,Latitude,Longitude,Elevation( in FT),Marking,Remarks");
                foreach (List<string> obstacle in obstacles)
                {
                    // Write A Comma Separated Row to Document
                    w.WriteLine(new Obstacle(obstacle));
                    w.Flush();
                }
            }
        }

        public static List<List<string>> GetObstacleList(string file_path)
        {
            TableExtractionStrategy strategy = new TableExtractionStrategy
            {
                // Modify this value if Required
                NextLineLookAheadDepth = 200,
                NextCharacterThreshold = 1.0F
            };

            // Setting this to true will add the required value
            bool add = false;
            using (PdfReader reader = new PdfReader(file_path))
            {                // Iterate through all the pages
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
                    // This is Done to Reduce Amount of Processing we need to do later
                    if (!add)
                    {
                        strategy.Clear();
                    }
                }
            }
            // The Rest is a Hack
            // A Well Designed Hack that works on All Airports

            List<List<string>> tables = strategy.GetTable();
            strategy.Clear();

            // Remove All Unrequired Data from the Table
            // This Junk Gets Generated Given it's presence in table's periphery
            // We Know this Junk is Useless as
            // It has a Count of 1
            tables.RemoveAll(x => x.Count == 1
            && (x[0] == "In Approach/Take-off/Circling Area and at AD"
            || x[0] == "123456"/*Table Heading Index*/
            // Rest All Are always present
            // At the Top of the page
            || x[0].TrimStart().StartsWith("AD 2 VA")
            || x[0] == ("AIRAC effective date")
            || x[0].TrimStart().StartsWith("Airports Authority of India")
            || x[0].TrimStart().StartsWith("AMDT ")
            || x[0] == "India"
            || x[0].TrimStart().StartsWith("AIP")
            || IsUselessNumericDate(x[0])));

            List<List<string>> result = new List<List<string>>();
            for (int i = 0; i < tables.Count - 1;)
            {
                List<string> table_cur = tables[i];
                List<string> table_next = tables[i + 1];

                // Take care of the Situation when
                // One Line has
                // "In circling area and"
                // And the next line contains most of the Data
                if (table_cur.Count == 1
                    && Obstacle.IsRunwayAreaAffected(table_cur[0])
                    && table_next.Count >= 1)
                {
                    table_cur[0] += table_next[0];
                    for (int j = 1; j < table_next.Count; ++j)
                    {
                        table_cur.Insert(0 + j, table_next[j]);
                    }
                    tables.RemoveAt(i + 1);
                    continue;
                }
                // When we have 3 Elements already
                // And We need to add other 3 Elements
                // Or at least how many we can scrape and find
                if (table_cur.Count == 3 && table_next.Count >= 1)
                {
                    // This is primarily used if the Data is Encoded in such a way
                    // That Half of the Location
                    // Specifically the Lower Half present in the PDF
                    // Is in the next Line
                    // And the Rest of the Data is in the Next Line and the Line after that
                    // Like 222.333N
                    // 4555.45E
                    // This is converted to 222.333N4555.45E
                    if (CoOrdinate.IsLongitude(table_cur[2])
                        && CoOrdinate.IsLatitude(table_next[0]) /*Location*/)
                    {
                        // Increment the Two Locations
                        table_cur[2] += table_next[0];
                        // Increment Rest of Present Values
                        for (int j = 1; j < table_next.Count; ++j)
                        {
                            table_cur.Insert(2 + j, table_next[j]);
                        }
                        // Remove this Given Location
                        tables.RemoveAt(i + 1);
                        if (table_cur.Count + table_next.Count == 6/*If this One and the next one's sum is current count*/
                                                                   // If the Next Element is present within range
                            && i + 1 < tables.Count)
                        {
                            table_next = tables[i + 1];
                            table_cur.InsertRange(table_cur.Count, table_next);
                            tables.RemoveAt(i + 1);
                        }
                        continue;
                    }

                    // This Portion is Primarily used if
                    // Data is Encoded in Such a way that the Split occurs
                    // With Elevation being in the next element List
                    // And Elevation Contains all Elements
                    // We can check for Elements using FT End Value that
                    // All Elevation Params Contain
                    // For Example Data is
                    // In Blah Blah,TREE,222.333N4555.45E
                    // 345 FT
                    // This piece of code combines both of them together
                    if (CoOrdinate.IsLongitudeLatitudeInCSVForm(table_cur[2]) /*Location*/
                        && Obstacle.IsElevation(table_next[0]))
                    {
                        // As this is Confirmed
                        // It implies we are supposed to add the two Ranges to each other
                        table_cur.InsertRange(3, table_next);
                        tables.RemoveAt(i + 1);
                        continue;
                    }
                }
                // There Are many cases when
                // The Next Value contains An Extension of the previous value
                // For Example, Hello World This is Vortex
                // Is a long statement that the parser could have
                // considered as a different statement
                // So we need to merge all of them together
                if (table_cur.Count == 6 && table_next.Count == 1)
                {
                    // In case Next Element is Actually Start of a New Value
                    // Which is in improper format
                    if (Obstacle.IsRunwayAreaAffected(table_next[0]))
                    {
                        ++i;
                        continue;
                    }
                    table_cur[5] += " " + table_next[0];
                    tables.RemoveAt(i + 1);
                }
                else
                {
                    ++i;
                    continue;
                }
            }
            // Trim All Elements
            foreach (List<string> items in tables)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    items[i] = items[i].Trim();
                }
            }
            // Note that we find that every Obstacle Table Element has a count of 6
            // Any Table Element with lesser count is to be removed
            // The Headings also contain 6 Elements
            // Remove this by Passing
            // RWY/Area affected which is the value of the first heading
            tables.RemoveAll(x => x.Count != 6 //6 Columns present
            // Header Column Remove
            || x[0].StartsWith("RWY/Area affected")
            // Check if Position 3 Is Occupied by Elevation
            // If not, it is a Useless 6 Valued Coincidence
            || !Obstacle.IsElevation(x[3])
            // Check if the Second Position is Indeed Location
            // If not, it is a Useless 6 Valued Coincidence
            || !CoOrdinate.IsLongitudeLatitudeInCSVForm(x[2])
            );

            foreach (List<string> items in tables)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    // Replace Double Consecutive Spaces by
                    // Single Space
                    items[i] = Regex.Replace(items[i], @"\s+", " ").Trim();
                    // Also note that for a Specific Airport, BB/Mumbai/BOM we find that
                    // The PDF Has COMMUNICATION but this is written on the next line
                    // As A result to Correct this issue, 
                    // we will replace all occurrences of "COMMUNICATIO N"
                    // with COMMUNICATION
                    // Replace Wrongly Placed Hyphens as well
                    items[i] = items[i].Trim().Replace("COMMUNICATIO N", "COMMUNICATION")
                    // Replace Hyphens
                    .Replace("- ", "-").Replace(" -", "-");
                }
            }

            // This table contains the final values
            return tables;
        }

        // Also read https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tryparseexact?redirectedfrom=MSDN&view=netframework-4.7.2
        private static bool IsUselessNumericDate(string val)
        {
            DateTime dt = new DateTime();
            return DateTime.TryParseExact(val.Trim(), "dd MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        }
    }
}
