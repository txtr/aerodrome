using System.Collections.Generic;

namespace PdfToCSVAD210
{
    class CoOrdinate
    {
        // Measures North South Value
        public string Latitude { get; }
        // Measures East West Value
        public string Longitude { get; }

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

            Latitude = coordinates.Substring(0, lat_idx+1);
            // Ignore Last Character
            Longitude = coordinates.Substring(lat_idx + 1, coordinates.Length - lat_idx - 1);//.TrimStart('0');
        }
    }
    class Obstacle
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
