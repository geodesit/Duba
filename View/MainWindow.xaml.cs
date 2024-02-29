using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI.Controls;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Symbology;
using DubaProject.Files;
using DubaProject.ViewModel;
using Point = DubaProject.Objects.Point;
using DubaProject.Convertors;
using System.Data.Common;
using Timer = System.Timers.Timer;
using UnityEngine;
using DubaProject.Objects;
using Esri.ArcGISRuntime.Mapping;
using Aspose.Gis.Rendering;
namespace DubaProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private List<Graphic> routesGraphic = new List<Graphic>();
        private List<Graphic> pointsGraphic= new List<Graphic>();
        private int routeIds = 0;
        private Timer UdpSocket;
        private GraphicsOverlay GraphicOverlayer = new GraphicsOverlay();
        private DubeConnection dubeConnection = new DubeConnection();
        EnvironmentVariableTarget task;
        private
            MapViewModel _viewModel;

        public MainWindow()
        {

            // Set the view model's "GraphicsOverlays" property (will be consumed by the map view).
            InitializeComponent();
            MainMapView.LocationDisplay.IsEnabled = true;
            MainMapView.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Recenter;
            // Create and set the ViewModel with the MapView instance

            // Zoom to the extent of an envelope with some padding (10 pixels).
            _viewModel = new MapViewModel();
            DataContext = _viewModel;

            MainMapView.LocationDisplay.IsEnabled = true;
            MainMapView.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Recenter;
            MainMapView.GeoViewTapped += mouseEvent;
            GraphicOverlayer = new GraphicsOverlay();

            // Add the overlay to a graphics overlay collection.
            GraphicsOverlayCollection overlays = new GraphicsOverlayCollection
            {
                GraphicOverlayer
            };
            MainMapView.GraphicsOverlays = overlays;


        }



        private Polyline CreatePolyline(Point[] routePoints)
        {
            // Create a point collection with coordinates that approximates the border between California and Nevada.
            Esri.ArcGISRuntime.Geometry.PointCollection thePointCollection = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);


            for (int i = 0; i < routePoints.Length; i++)
            {
                double[] GeoPoint = UtmToGeo.convertUTMToGeo(routePoints[i].X, routePoints[i].Y);
                thePointCollection.Add(GeoPoint[1], GeoPoint[0]);

            }


            // Create a polyline from the point collection.
            Polyline thePolyline = new Polyline(thePointCollection);

            // Return the geometry.
            return thePolyline;
        }



        private void DubaPointsThread()
        {
            DubaRecvPoints.Text = "X: " + GlobalVariebles.dubaPoint.X +
                      ", Y: " + GlobalVariebles.dubaPoint.Y +
                      ", altitude: " + GlobalVariebles.dubaPoint.Z +
                      ", Time: " + GlobalVariebles.dubaPoint.Name;

            // Run on all the duba Points List
            //double addingForDisplay = 0;
            for (int i = 0; i < GlobalVariebles.DubaPoints.Count; i+=100)
            {
                // Map point gets only Geo points
                double[] latlon = UtmToGeo.convertUTMToGeo(GlobalVariebles.DubaPoints[i].X, GlobalVariebles.DubaPoints[i].Y);
                //double[] latlon = convertUTMToGeo(GlobalVariebles.DubaPoints[i].X+(i*10), GlobalVariebles.DubaPoints[i].Y + (i * 10));

                var displayPoint = new MapPoint(latlon[1], latlon[0], SpatialReferences.Wgs84);

                // Create a symbol to define how the point is displayed.
                var pointSymbol = new SimpleMarkerSymbol
                {
                    Style = SimpleMarkerSymbolStyle.Circle,
                    Color = System.Drawing.Color.Orange,
                    Size = 10.0
                };

                // Add an outline to the symbol.
                pointSymbol.Outline = new SimpleLineSymbol
                {
                    Style = SimpleLineSymbolStyle.Solid,
                    Color = System.Drawing.Color.Red,
                    Width = 2.0
                };

                var pointGraphic = new Graphic(displayPoint, pointSymbol);

                // Add the point graphic to graphics overlay.
                GraphicOverlayer.Graphics.Add(pointGraphic);
                //addingForDisplay += 0.1;
            }

        }
        /// <summary>
        /// Function that connnect to the Duba and receiving points
        /// </summary>
        /// 
        public void ConnectToDuba()

        {
            try
            {
                // Variables
                int recNum = 0;
                byte[] keepALive = { 0xaa, 0x82, 01, 06, 0xfe, 0xcd }; // const
                int xcoor, ycoor;
                int recv, echo;
                int altitude, azimuth, time;
                byte[] echoKeepAlive = new byte[1024];

                // Create Ip Endspoint for udp socket
                IPEndPoint ipUdp = new IPEndPoint(IPAddress.Any, 6451);
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)(sender);

                // Create a TcpSocket for the keepALive connection
                Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socketTcp.Connect(IPAddress.Parse("192.168.1.133"), 6450);
                // check  if connected
                if (socketTcp.Connected)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ConnectDot.Fill = new SolidColorBrush(Colors.Green);
                        ConnectBtn.Header = "Disconnect";
                    }));
                }
                else
                {
                    throw new Exception("Make sure Duba is connected");
                }

                while (true)
                {

                    if (!GlobalVariebles.running)
                    {
                        socketTcp.Close();
                        return;
                    }
                    else
                    {
                        string logFilePath = Functions.createLogFile();
                        //Create a udp socket for receiving the points
                        Socket socketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socketUdp.Bind(ipUdp);

                        // Send keepalive commeande
                        socketTcp.Send(keepALive);


                        echo = socketTcp.Receive(echoKeepAlive);

                        if (GlobalVariebles.PUentered)
                        {
                            byte[] PUBuffer = DubeConnection.positionUpdateBuffer(GlobalVariebles.PUPoint.X, GlobalVariebles.PUPoint.Y,(double)GlobalVariebles.PUPoint.Z);
                            socketTcp.Send(PUBuffer);
                            GlobalVariebles.PUclicked = false;
                            GlobalVariebles.PUentered = false;
                        }

                        // Recv the points
                        byte[] data = new byte[1024];
                        recv = socketUdp.Receive(data);


                        // Reading bytes
                        byte[] utcTime = { data[7], data[6], data[5], data[4] };
                        byte[] xBytes = { data[11], data[10], data[9], data[8] };
                        byte[] yBytes = { data[15], data[14], data[13], data[12] };
                        byte[] Alt = { data[19], data[18], data[17], data[16] };
                        byte[] Azi = { data[21], data[20] };

                        // Converting Bytes
                        time = BitConverter.ToInt32(utcTime, 0);
                        time = (int)(Math.Pow(2, -12) * time);
                        xcoor = BitConverter.ToInt32(xBytes, 0);
                        ycoor = BitConverter.ToInt32(yBytes, 0);
                        altitude = BitConverter.ToInt32(Alt, 0);
                        //azimuth = BitConverter.ToInt32(Azi, 0);

                        //xcoor = xcoor / 10;
                        //ycoor = ycoor / 10;
                        // Display the point on the map
                        Dispatcher.Invoke(new Action(() =>
                        {
                            DubaRecvPoints.Text = "X: " + xcoor + ", Y: " + ycoor + ", altitude: " + altitude + ", Time: " + time;
                            GlobalVariebles.DubaPoints.Add(new Point(time.ToString(), xcoor, ycoor));
                            DubaPointsThread();
                        }));

                        // Every one Second
                        Thread.Sleep(1000);

                        socketUdp.Close();

                    }

                }


            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }


        }


        /// <summary>
        /// open an add point window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPointManClick(object sender, RoutedEventArgs e)
        {
            AddPoint addPoint = new AddPoint();
            addPoint.Show();
        }
        private void addPointCLKClick(object sender, RoutedEventArgs e)
        {
            GlobalVariebles.UserPointClicked = true;
        }
        private void ShowPointClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GlobalVariebles.UserPoints.Count; i++)
            {
                // Map point gets only Geo points
                double[] latlon = UtmToGeo.convertUTMToGeo(GlobalVariebles.UserPoints[i].X, GlobalVariebles.UserPoints[i].Y);
                //double[] latlon = convertUTMToGeo(GlobalVariebles.DubaPoints[i].X+(i*10), GlobalVariebles.DubaPoints[i].Y + (i * 10));

                var displayPoint = new MapPoint(latlon[1], latlon[0], SpatialReferences.Wgs84);

                // Create a symbol to define how the point is displayed.
                var pointSymbol = new SimpleMarkerSymbol
                {
                    Style = SimpleMarkerSymbolStyle.Circle,
                    Color = System.Drawing.Color.Yellow,
                    Size = 10.0
                };

                // Add an outline to the symbol.
                pointSymbol.Outline = new SimpleLineSymbol
                {
                    Style = SimpleLineSymbolStyle.Solid,
                    Color = System.Drawing.Color.Green,
                    Width = 2.0
                };

                pointsGraphic.Add(new Graphic(displayPoint, pointSymbol));

                // Add the point graphic to graphics overlay.
                GraphicOverlayer.Graphics.Add(pointsGraphic[pointsGraphic.Count - 1]);
            }
        }
        private void HidePointClick(object sender, RoutedEventArgs e)
        {
            try
            {
            GraphicOverlayer.Graphics.Remove(pointsGraphic[pointsGraphic.Count-1]);
            pointsGraphic.Remove(pointsGraphic[pointsGraphic.Count - 1]);
            }
            catch
            {
                MessageBox.Show("No Points to remove");
            }
        }
        public void StartRecevingData()
        {
            while (!GlobalVariebles.connected)
            {
                dubeConnection.BindviaUdp();
                if (GlobalVariebles.DubaPoints == null) { }
                else
                {
                    int lastRecvPoint = GlobalVariebles.DubaPoints.Count - 1;
                    
                    Dispatcher.Invoke(new Action(() =>
                    {
                        DubaRecvPoints.Text = "X: " + GlobalVariebles.DubaPoints[lastRecvPoint].X
                        + ", Y: " + GlobalVariebles.DubaPoints[lastRecvPoint].Y
                        + ", altitude: " + GlobalVariebles.DubaPoints[lastRecvPoint].Z
                        + ", Time: " + GlobalVariebles.DubaPoints[lastRecvPoint].Name;
                        DubaPointsThread();
                    }));
                    if (GlobalVariebles.PUentered)
                    {
                        dubeConnection.setPositionUpdate(GlobalVariebles.PUPoint);

                    }
                }

            }
        }
        /// <summary>
        /// This function is activated when you click connect 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectClick(object sender, RoutedEventArgs e)
        {

            if (GlobalVariebles.connected)
            {
                

                GlobalVariebles.running = true;
                dubeConnection.connectToDuba();
                GlobalVariebles.connected = false;
                Thread thread  = new Thread(StartRecevingData);
                thread.Start();
                //Thread thread = new Thread(ConnectToDuba);
                //thread.Start();
                //Thread.Sleep(3000);
                Dispatcher.Invoke(new Action(() =>
                {
                    ConnectDot.Fill = new SolidColorBrush(Colors.Green);
                    ConnectBtn.Header = "Disconnect";
                }));

            }
            else
            {
                dubeConnection.disconnectDube();
                ConnectDot.Fill = new SolidColorBrush(Colors.Red);
                ConnectBtn.Header = "Connect";
                DubaRecvPoints.Text = null;
                GlobalVariebles.running = false;
                GlobalVariebles.connected = true;
            }


        }
        /// <summary>
        /// This function activated when the user press the position updtae button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionUpdateClicked(object sender, RoutedEventArgs e)
        {
            GlobalVariebles.PUclicked = true;
            AddPoint addPointWin = new AddPoint();
            addPointWin.Show();

        }
        private void mouseEvent(object sender, GeoViewInputEventArgs e)
        {
            MapPoint tappedPoint = e.Location;

            // Project the user-tapped map point location to a geometry
            Esri.ArcGISRuntime.Geometry.Geometry myGeometry = tappedPoint.Project(SpatialReferences.Wgs84);

            // Convert to geometry to a traditional Lat/Long map point
            MapPoint projectedLocation = (MapPoint)myGeometry;
            double[] utm = GeoToUtm.ConvertLatLongToUTM(projectedLocation.Y, projectedLocation.X);
            if (GlobalVariebles.UserPointClicked)
            {
                GlobalVariebles.UserPoint = new Point("", utm[0], utm[1]);
                AddPoint addPoint = new AddPoint();
                addPoint.Show();
                GlobalVariebles.UserPointClicked = false;
            }
            else
            {
                string mapLocationDescription = string.Format("X: {0:F3} Y: {1:F3}", utm[0], utm[1]);

                // Create a new callout definition using the formatted string
                CalloutDefinition myCalloutDefinition = new CalloutDefinition("Location:", mapLocationDescription);
                MainMapView.ShowCalloutAt(tappedPoint, myCalloutDefinition);
            }

        }
        private void AddPointFromDubaClick(object sender, RoutedEventArgs e)
        {
            if (GlobalVariebles.running)
            {
                GlobalVariebles.UserPointClicked = true;
                GlobalVariebles.UserPoint =  new Point("", GlobalVariebles.dubaPoint.X, GlobalVariebles.dubaPoint.Y);
                GlobalVariebles.UserPoint.setZ((double)GlobalVariebles.dubaPoint.Z);
                AddPoint add = new AddPoint();
                add.Show();
            }

        }
        private async void GoToPointClick(object sender, RoutedEventArgs e)
        {
            if (GlobalVariebles.running)
            {
                double[] geoCoord = UtmToGeo.convertUTMToGeo(GlobalVariebles.dubaPoint.X, GlobalVariebles.dubaPoint.Y);
                Viewpoint point = new Viewpoint(geoCoord[0], geoCoord[1], 25000);
                await MainMapView.SetViewpointAsync(point);
            }


        }
        private void SavePointClick(object sender, RoutedEventArgs e)
        {

        }
        private void DrawLineClick(object sender, RoutedEventArgs e)
        {

        }
        private void ShowLineClick(object sender, RoutedEventArgs e)
        {

        }
        private void HideLineClick(object sender, RoutedEventArgs e)
        {

        }

        private void ImportRouteClick(object sender, RoutedEventArgs e)
        {

            SimpleLineSymbol theSimpleLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 3);

            string routeFilePath = Functions.FileDiolog(".txt");
            Point[] routePoints = Functions.ReadPointsForReouteFile(routeFilePath);

            Route importedRoute = new Route(routeIds, routeFilePath, routePoints);

            routesGraphic.Add(new Graphic(CreatePolyline(importedRoute.Points), theSimpleLineSymbol));

            // Set the map views graphics overlay to the created graphics overlay.
            GraphicOverlayer.Graphics.Add(routesGraphic[importedRoute.Id]);
            routeIds += 1;
        }
        private void RemoveRouteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                GraphicOverlayer.Graphics.Remove(routesGraphic[routeIds - 1]);
                routeIds -= 1;
            }
            catch 
            {
                MessageBox.Show("No Routes to remove"); 
            }

        }

    }

}