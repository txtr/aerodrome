using System;
using System.Threading.Tasks;

namespace PdfToCSVParser
{
    public class MainClass
    {
        // You use it by running
        // App PDF1 PDF2 PDF3
        // It creates Result CSV in Same Folder( can be customised later)
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("The Usage is APP PDF1 PDF2 ...");
            }
            // Creates an Array of tasks that need to be converted
            Task[] conversion_tasks = new Task[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                // Get Source PDF Location
                // Currently Supplied via Command Line Arguments
                string FileSourcePDF = args[i];
                // Generate CSV Location
                string FileDestinationCSV = args[i].Substring(0, args[i].LastIndexOf('.') + 1) + "csv";
                // Adds to the Conversion Task List
                conversion_tasks[i] = Task.Factory.StartNew(() =>
                {
                    // Create the Parser
                    ParseAD210 parser = new ParseAD210(FileSourcePDF, FileDestinationCSV);
                    // This Performs the parsing operation
                    parser.PerformConversion();
                });
            }
            // Wait till All Conversion Tasks are over
            Task.WaitAll(conversion_tasks);
        }
    }
}
