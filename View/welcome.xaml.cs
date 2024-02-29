using DubaProject;
using DubaProject.Files;
using System.Windows;


namespace DubaProject
{
    /// <summary>
    /// Interaction logic for welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
            this.Title = "app";
        }

        private void SelectMMPK_click(object sender, RoutedEventArgs e)
        {
            GlobalVariebles.pathToMobileMapPackage = Functions.FileDiolog(".mmpk");
           
            MainWindow main = new MainWindow(); 
            main.Show();
            Close();
        }
    }
}
