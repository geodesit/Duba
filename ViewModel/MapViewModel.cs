using System;
using System.Collections.Generic;
using System.Text;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BitMiracle.LibTiff;
using Esri.ArcGISRuntime.Portal;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Net.Sockets;
using System.Net;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Symbology;
using Microsoft.Win32;
using Esri.ArcGISRuntime.UI.Controls;
using UnityEngine;

namespace DubaProject.ViewModel
{
    class MapViewModel : ViewModelBase
    {

        public MapViewModel()
        {

            _ = SetupMap();
            // SetUpMapOnline();



        }


        // Use only when there is internet connection... for Dev
        private void SetUpMapOnline()
        {
            Map = new Map(BasemapStyle.ArcGISTopographic);

        }
        /// <summary>
        /// Thw main set up Map function, use a mmpk file
        /// </summary>
        /// <returns></returns>
        private async Task SetupMap()
        {

            string pathmmpk;
            // Process open file dialog box results
            if (GlobalVariebles.pathToMobileMapPackage != ".")
            {
                // Open document
                pathmmpk = GlobalVariebles.pathToMobileMapPackage;
                // Instantiate a new mobile map package.
                MobileMapPackage MobileMapPackage = new MobileMapPackage(pathmmpk);

                // Load the mobile map package.
                await MobileMapPackage.LoadAsync();

                // Show the first map in the mobile map package.
                Map = MobileMapPackage.Maps.FirstOrDefault();


            }
            else
            {
                MessageBox.Show(" No Map Selected");
            }

        }

    }
}