using System.Collections.Generic;

namespace PdfToCSVAD210
{
    internal class CoOrdinate
    {
        // Measures North South Value
        public string Latitude { get; }
        // Measures East West Value
        public string Longitude { get; }

        // Function to Check if the Given String Represents Longitude
        public static bool IsLongitude(string val)
        {
            // Longitude In PDF
            // Always ends with N or S
            // And Contains .
            return (val.TrimEnd().EndsWith('N') || val.TrimEnd().EndsWith('S'))
                && val.Contains(".");
        }
        // Function to Check if the Given String Represents Latitude
        public static bool IsLatitude(string val)
        {
            // Latitude In PDF
            // Always ends with E or W
            // And Contains .
            return (val.TrimEnd().EndsWith('E') || val.TrimEnd().EndsWith('W'))
                && val.Contains(".");
        }
        // Function to Check if the Given String Represents Longitude and Latitude Combined
        public static bool IsLongitudeLatitudeInCSVForm(string val)
        {
            // Longitude & Latitude Together In PDF
            // Always ends with E or W
            // And Contain 'N' or 'S'
            // And Contains .
            return ((val.Contains('N') || val.Contains('S'))
                && val.Contains(".")
                && (val.TrimEnd().EndsWith('E') || val.TrimEnd().EndsWith('W')));
        }
        // As present in CSV File

        // Sample Coordinate Format is
        // 222043.2N0731400.8E
        // So we need to Split it into
        // Latitude and Longitude
        public CoOrdinate(string coordinates)
        {
            int lat_idx = coordinates.IndexOf('N');
            if (lat_idx == -1)
            {
                lat_idx = coordinates.IndexOf('S');
            }

            Latitude = coordinates.Substring(0, lat_idx + 1);
            Longitude = coordinates.Substring(lat_idx + 1, coordinates.Length - lat_idx - 1);//.TrimStart('0');
        }
    }

    internal class Obstacle
    {
        private int elevation;

        // First Unit
        public string RunwayAreaAffected { get; }
        // Second Unit
        public string ObstacleType { get; }
        // Third Unit
        public CoOrdinate CoOrdinate { get; }
        // Fourth Unit
        public string Elevation { get => elevation.ToString(); set => elevation = int.Parse(value.Substring(0, value.IndexOf('F')), System.Globalization.NumberStyles.Integer); }
        // Fifth Unit
        public string Marking { get; }
        // Sixth Unit
        public string Remarks { get; }

        // Code to Verify if the data is a Given Runway/Area Which is Affected
        public static bool IsRunwayAreaAffected(string val)
        {
            return val.TrimStart().StartsWith("In circling area and") || val.Contains("TKOF") || val.Contains("APCH");
        }

        // We Know that Elevation always ends with FT
        public static bool IsElevation(string value)
        {
            return value.TrimEnd().EndsWith("FT");
        }

        // Initialise With List<String>
        // Our Table Extracted from PDF Has Each Element in this form
        public Obstacle(List<string> obstacles)
        {
            RunwayAreaAffected = obstacles[0];
            ObstacleType = obstacles[1];
            CoOrdinate = new CoOrdinate(obstacles[2]);
            Elevation = obstacles[3];
            Marking = obstacles[4];
            Remarks = obstacles[5];
        }
        // Convert to CSV Able Format
        public override string ToString()
        {
            return RunwayAreaAffected + "," + ObstacleType + "," + CoOrdinate.Latitude + "," + CoOrdinate.Longitude + "," + Elevation + "," + Marking + "," + Remarks;
        }
    }
}
