using System;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    [Serializable]
    public class Line2 : LineSegments2
    {
        public Line2() : base()
        {            
        }
        public Line2(Geometry geometry,Material material) : base(geometry,material)
        {

        }       
    }
}
