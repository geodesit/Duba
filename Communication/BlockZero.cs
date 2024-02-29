using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DubaProject.Objects;
using UnityEngine;

namespace DubaProject
{
    internal class BlockZero:DataBlock
    {
        public BlockZero(byte[] data, int blockLength)
        {
            BlockNumber = checkWhichBlock(blockLength);
            Data = data;
            BlockLength = blockLength;
        }
        public Point getPointFromData()
        {
            byte[] xBytes = { Data[11], Data[10], Data[9], Data[8] };
            byte[] yBytes = { Data[15], Data[14], Data[13], Data[12] };
            byte[] Alt = { Data[19], Data[18], Data[17], Data[16] };
            double altitude = BitConverter.ToInt32(Alt, 0);
            double xcoor = BitConverter.ToInt32(xBytes, 0);
            double ycoor = BitConverter.ToInt32(yBytes, 0);
            Point recivedPoint = new Point("", xcoor, ycoor);
            recivedPoint.setZ(altitude);
            return recivedPoint;
        }
        public double getTimeFromData()
        {
            byte[] utcTime = { Data[7], Data[6], Data[5], Data[4] };
            double time = BitConverter.ToInt32(utcTime, 0);
            time = (Math.Pow(2, -12) * time);
            return time;

        }

    }
}
