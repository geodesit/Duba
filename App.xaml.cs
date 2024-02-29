using System.Configuration;
using System.Data;
using System.Windows;

namespace DubaProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Note: it is not best practice to store API keys in source code.
            // The API key is referenced here for the convenience of this tutorial.
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK392a480cd4054496b7c9332cd55aeec9-OrUEbhhiZYFRGnF3EZymodr8n23Sn9F2vBrM0Nb0w-zFukCMgp3oRMHEup9OnQB";
        }
    }

}
