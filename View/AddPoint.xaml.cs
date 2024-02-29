using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = DubaProject.Objects.Point;

namespace DubaProject
{
    /// <summary>
    /// Interaction logic for AddPoint.xaml
    /// </summary>
    public partial class AddPoint : Window
    {

        public AddPoint()
        {
 
            InitializeComponent();
            PointNameLabel.Content = "Point Description";
            if (GlobalVariebles.UserPointClicked)
            {

                xCoordinateEntery.Text = GlobalVariebles.UserPoint.X.ToString();
                yCoordinateEntery.Text = GlobalVariebles.UserPoint.Y.ToString();
                if (GlobalVariebles.UserPoint.Z != null )
                {
                    zCoordinateEntery.Text = GlobalVariebles.UserPoint.Z.ToString();
                }

            }
            else if (GlobalVariebles.PUclicked)
            {
                PointNameLabel.Content = "Position Update:";
                namePointEntery.Visibility = Visibility.Hidden;
            }
        }
        private void EnterPointClick(object sender, RoutedEventArgs e)
        {
            try
            {
                double xCoordinate = double.Parse(xCoordinateEntery.Text);
                double yCoordinate = double.Parse(yCoordinateEntery.Text);
                double zCoordinate = double.Parse(zCoordinateEntery.Text);
                string CoordinateName = namePointEntery.Text;

                if (GlobalVariebles.PUclicked)
                {
                    GlobalVariebles.PUPoint = new Point(CoordinateName, xCoordinate, yCoordinate);
                    GlobalVariebles.PUPoint.setZ(zCoordinate);
                    GlobalVariebles.PUentered = true;
                }
                else
                {

                    GlobalVariebles.UserPoints.Add(new Point(CoordinateName, xCoordinate, yCoordinate));
                    WriteIntoTXTFile(GlobalVariebles.UserPoints.Count, xCoordinate, yCoordinate, CoordinateName);

                }
                Close();
            }
            catch { MessageBox.Show("please fill all the fields"); }

        }
        private void WriteIntoTXTFile(int i ,double x, double y, string description)
        {
            string currentDirectory = Environment.CurrentDirectory;
            //string currentDirectory = "C:\\Users\\Geodesy\\source\\repos\\DisplayAMap";
            // Create a subdirectory
            string subdirectoryName = "Logger";
            string subdirectoryPath = currentDirectory + "\\" + subdirectoryName;
            // Check if the subdirectory does not exist, then create it
            if (!Directory.Exists(subdirectoryPath))
            {
                Directory.CreateDirectory(subdirectoryPath);
                Console.WriteLine($"Subdirectory '{subdirectoryName}' created.");
            }
            else
            {
                Console.WriteLine($"Subdirectory '{subdirectoryName}' already exists.");
            }
            // Specify the path to your text file
            string filePath = subdirectoryPath + "\\SavedPoint.txt";
            DateTime currentTime = DateTime.Now;
            // Text to append to the file
            string textToAppend =i+". " +currentTime+", "+x + ", "+y+", "+description;

            // Append text to the file
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(textToAppend);
            }
        }


    }
}
