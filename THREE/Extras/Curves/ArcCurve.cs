using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Extras.Curves
{
    public class ArcCurve : EllipseCurve
    {
        public ArcCurve(float? aX = null, float? aY = null, float? aRadius = null, float? aStartAngle = null, float? aEndAngle = null, bool? clockwise = null) : base(aX, aY, aRadius, aRadius, aStartAngle, aEndAngle, clockwise,null)
        {

        }
    }
}
