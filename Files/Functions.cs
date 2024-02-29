using System.IO;
using Point = DubaProject.Objects.Point;

namespace DubaProject.Files
{
    public class Functions
    {
        /// <summary>
        /// This function open a file dialog for a mmpk file and return a string of the file path
        /// </summary>
        /// <returns> string of the file path</returns>
        public static string FileDiolog(string FileType)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = Directory.GetCurrentDirectory(); // Default file name
            dialog.DefaultExt = FileType; // Default file extension
            dialog.Filter = string.Format(" ({0:F3})|*{0:F3}", FileType); // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string pathToMobileMapPackage = dialog.FileName;
                return pathToMobileMapPackage;
            }
            return ".";
        }
        public static Point[] ReadPointsForReouteFile(string filePath)
        {
            if (File.Exists(filePath))
            {

                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);
                if (lines[0].Contains("name"))
                {
                    Point[] points = new Point[lines.Length - 2];
                    // Display each line
                    for (int i = 0; i < points.Length; i++)
                    {
                        string[] values = lines[i + 1].Split(',').Select(value => value.Trim()).ToArray();


                        double x = double.Parse(values[1]);
                        double y = double.Parse(values[2]);

                        points[i] = new Point(values[0], x, y);
                    }
                    return points;
                }
                else
                {
                    Point[] points = new Point[lines.Length - 18];
                    for (int i = 18; i < points.Length+18; i++)
                    {
                        string[] values = lines[i].Split(',').Select(value => value.Trim()).ToArray();

                        double x = double.Parse(values[2]);
                        double y = double.Parse(values[1]);

                        points[i-18] = new Point(values[0], x, y);
                    }
                    return points;
                }
            }
            else
            {
                Console.WriteLine("File not found.");
                return null;
            }


        }
        public static string createLogFile()
        {

            string currentDirectory = Environment.CurrentDirectory + "\\LOG";
            Directory.CreateDirectory(currentDirectory);

            DateTime Datetime = DateTime.Now;
            string fileName = "LogSholderData" + Datetime.ToString("yyyy-dd-M--HH-mm-ss") + ".bin";
            //string fileName = "LogSholderData" + "testttt" + ".txt";

            string text = "Number Of fields: 14;\n" +
                "1  ,REC NUM         ,-\n" +
                "2  ,NorthingUTM     ,m\n" +
                "3  ,EastingUTM      ,m\n" +
                "4  ,Altitude        ,m\n" +
                "5  ,RTC Counter     ,-\n" +
                "6  ,Data Was Sent   ,\n" +
                "7  ,Integrated Mode ,\n" +
                "8  ,WP Data         ,\n" +
                "9  ,IMU Valid       ,\n" +
                "10 ,Motion Flag     ,\n" +
                "11 ,Cycle Number    ,\n" +
                "12 ,GPS Time        ,sec\n" +
                "13 ,GPS Week        ,-\n" +
                "14 ,Traj_Num        ,-\n" +
                ";\n" +
                "Date of Trajectory Data: " + Datetime + "\n" +
                ";\n";
            // using (File.Create(currentDirectory + "\\" + fileName))

            File.WriteAllText(currentDirectory + "\\" + fileName, text);
            return currentDirectory + "\\" + fileName;
        }
        public static void UpdateLogFile(string filePath, double[] data)
        {
            if (File.Exists(filePath))
            {
                string dataLine = "\t\t" + data[0];
                for (int i = 1; i < data.Length; i++)
                {
                    dataLine = dataLine + ",\t" + data[i];
                }
                dataLine = dataLine + "\n";
                File.AppendAllText(filePath, dataLine);
            }
        }
    }
}
