using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;
namespace THREE.Lights
{
    public class RectAreaLight : Light
    {
        public RectAreaLight(Color color, float? itensity=null, int? width=null, int? height=null) : base(color,itensity)
        {
            this.Width = (width != null) ? (int)width : 10;
            this.Height = (height != null) ? (int)height : 10;
            this.type = "RectAreaLight";
        }
        public RectAreaLight(int color, float? itensity = null, int? width = null, int? height = null) : this(Color.Hex(color), itensity)
        {

        }
        protected RectAreaLight(RectAreaLight other) : base(other)
        {
            this.Width = other.Width;
            this.Height = other.Height;
            this.type = "RectAreaLight";
        }

    }
}
