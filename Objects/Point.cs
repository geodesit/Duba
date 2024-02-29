using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DubaProject.Objects
{
    public class Point
    {
        // Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double? Z { get; set; }
        public string Name { get; set; }

        // Constructor
        public Point(string name, double x, double y)
        {
            X = x;
            Y = y;
            Name = name;

        }
        public void setZ(double z)
        { Z = z; }

    }

}
