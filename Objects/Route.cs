using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DubaProject.Objects
{
    internal class Route
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public Point[] Points { get; set; }

        public Route() 
        {
            Points = new Point[0];
            Id = 0;
            Name = string.Empty;
        }
        public Route(int id, string name, Point[] points)
        {
            Id = id;
            Name = name;
            Points = points;
        }
    }
}
