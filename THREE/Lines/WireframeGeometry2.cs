using System;
using System.Collections.Generic;
using System.Text;

namespace THREE
{
    public class WireframeGeometry2 : LineSegmentsGeometry
    {
        public WireframeGeometry2(BufferGeometry geometry) : base()
        {
            type = "WireframeGeometry2";
            this.FromWireframeGeometry(new WireframeGeometry(geometry));
        }
    }
}
