using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DubaProject
{
    internal class DataBlock
    {
        public int BlockNumber { get; set; }
        public byte[] Data { get; set; }
        public int BlockLength { get; set; }    
        public DataBlock()
        {
            BlockNumber = 0;
            Data = new byte[0];
            BlockLength = 0;
        }
        public DataBlock( byte[] data, int blockLength)
        {
            BlockNumber = checkWhichBlock(blockLength);
            Data = data;
            BlockLength = blockLength;
        }
        public static int checkWhichBlock(int len)
        {
            if (len == 48)
                return 0;
            else if (len == 168)
                return 1;
            else if (len == 108)
                return 2;
            else
                return 3;
        }
    }
}
