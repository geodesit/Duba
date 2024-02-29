using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Timers;
using Point = DubaProject.Objects.Point;
using Timer = System.Timers.Timer;
using System.Threading;
using UnityEngine;
using Aspose.Gis.Rendering;
using System.Windows.Media;
using System.Windows.Threading;
using DubaProject.ViewModel;
using DubaProject.Files;
namespace DubaProject
{
    class DubeConnection
    {
        private MapViewModel _viewmodel;
        public string IpAddress { get; set; }
        public int IpPortUdp { get; set; } 
        public int IpPortTcp { get; set; }
        public Point? PositionUpdate { get; set; }
        public Socket? SocketTcp { get; set; }
        public Socket? SocketUdp { get; set; }

        public byte[] KeepAlive = [0xaa, 0x82, 01, 06, 0xfe, 0xcd];
        private Timer sendKAtimer;
        private Timer UdpSocket;

        public DataBlock? RecviedData { get; set; }
        public bool IsConnected { get; set; } = false;
        public bool IsEntered { get; set; }
        public string? LogFilePath { get; set; }

        public DubeConnection()
        {
            IpAddress = "192.168.1.133";
            IpPortTcp = 6450;
            IpPortUdp = 6451;



        }
        public DubeConnection(string ipAddress, int ipPortUdp, int ipPortTcp)
        {
            IpAddress = ipAddress;
            IpPortUdp = ipPortUdp;
            IpPortTcp = ipPortTcp;

        }
        public void connectToDuba()
        {
            SocketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketTcp.Connect(IPAddress.Parse(IpAddress), IpPortTcp);
            setLogFilePath();
            sendKAtimer = new Timer();
            sendKAtimer.Interval = 1000; // in miliseconds
            sendKAtimer.AutoReset = true;
            sendKAtimer.Elapsed += sendKeepAlive;
            sendKAtimer.Start();

        }
        public void disconnectDube()
        {
            try
            {
                sendKAtimer.Stop();
                sendKAtimer.Dispose();
                SocketTcp.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public void BindviaUdp()
        {

            IPEndPoint ipUdp = new IPEndPoint(IPAddress.Any, IpPortUdp);
            SocketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketUdp.Bind(ipUdp);
            reciveData();
            SocketUdp.Close();
        }
        public void reciveData()
        {
            try
            {

                byte[] data = new byte[1024];
                int recv = SocketUdp.Receive(data);
                int blockNumber = DataBlock.checkWhichBlock(recv);
                if (blockNumber == 0)
                {
                    BlockZero dataBlock = new BlockZero( data, recv);
                    Point dubaPoint =  dataBlock.getPointFromData();
                    double time = dataBlock.getTimeFromData();
                    GlobalVariebles.DubaPoints.Add(new Point(String.Format("{0:0.00}", time),dubaPoint.X, dubaPoint.Y));
                    GlobalVariebles.dubaPoint = new Point(String.Format("{0:0.00}", time), dubaPoint.X, dubaPoint.Y);
                    GlobalVariebles.dubaPoint.setZ((double)dubaPoint.Z);
                    double[] LogData = [0, dubaPoint.Y+time, dubaPoint.X, (double)dubaPoint.Z, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    Functions.UpdateLogFile(LogFilePath, LogData);


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void sendKeepAlive(object sender, ElapsedEventArgs e)
        {
            try
            {
                int echo;
                byte[] echoKeepAlive = new byte[1024];
                SocketTcp.Send(KeepAlive);
                echo = SocketTcp.Receive(echoKeepAlive);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void setPositionUpdate(Point positionUpdate)
        {
            try
            {
                PositionUpdate = positionUpdate;
                byte[] PUBuffer = positionUpdateBuffer(positionUpdate.X, positionUpdate.Y, (double)positionUpdate.Z);
                SocketTcp.Send(PUBuffer);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
                
        }
        public void setLogFilePath()
        {
            LogFilePath = Functions.createLogFile();
        }
       

        /// <summary>
        /// This function gets a coordinate and then return the buffer for the position update to send to the duba.
        /// </summary>
        /// <param name="xUser"></param>
        /// <param name="yUser"></param>
        /// <returns>uffer for the position update</returns>
        public static byte[] positionUpdateBuffer(double xUser, double yUser, double altUser)
        {
            UInt32 yCoordinate = Convert.ToUInt32(yUser);
            UInt32 xCoordinate = Convert.ToUInt32(xUser);
            UInt32 altCoordinate = Convert.ToUInt32(altUser);

            //UInt32 alt = 22;
            UInt16 HorizentalPosError = 10;
            UInt16 AltError = 10;


            //string bytesize = Convert.ToString(19, 2);
            // create a Position Update Buffer 
            byte[] x = BitConverter.GetBytes(xCoordinate);
            byte[] y = BitConverter.GetBytes(yCoordinate);
            byte[] altt = BitConverter.GetBytes(altCoordinate);
            byte[] horizentalPosError = BitConverter.GetBytes(HorizentalPosError);
            byte[] AlttiError = BitConverter.GetBytes(AltError);
            Array.Reverse(x);
            Array.Reverse(y);
            Array.Reverse(altt);
            Array.Reverse(horizentalPosError);
            Array.Reverse(AlttiError);
            byte[] startBuffer = { 0xaa, 0x82, 19, 26 };
            IEnumerable<byte> full = startBuffer.Concat(x).Concat(y).Concat(altt);
            byte[] zoneDatum = { 36, 47 };
            full = full.Concat(zoneDatum).Concat(horizentalPosError).Concat(AlttiError);
            byte[] PuValidity = { 0x90, 0 };
            full = full.Concat(PuValidity);
            byte[] fullArray = full.ToArray();
            ushort checkSum = CalcChecksum(fullArray, fullArray.Length);
            byte[] checkSumB = BitConverter.GetBytes(checkSum);
            Array.Reverse(checkSumB);
            full = full.Concat(checkSumB);
            fullArray = full.ToArray();
            return fullArray;
        }

        /// <summary>
        /// This function gets byte array and then reverse the array 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>reversed array</returns>
        private static byte[] SwapEndianness(byte[] data)
        {
            // Clone the array to avoid modifying the original data
            byte[] result = (byte[])data.Clone();

            // Reverse the order of the bytes
            Array.Reverse(result);

            return result;
        }

        /// <summary>
        /// This function gets a data buffer and his length and return the checksum
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns>checksum</returns>
        private static ushort CalcChecksum(byte[] data, int length)
        {
            ushort sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += data[i];
            }

            ushort sumCalc = (ushort)(~sum + 1);
            return sumCalc;
        }
    }
}
