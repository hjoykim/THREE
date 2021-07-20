using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Core
{
    public class GeometryGroup : BaseGeometry
    {
        protected static int GeometryGroupIdCount;
        public GeometryGroup()
        {
            Id = GeometryGroupIdCount++;
        }
        public override void ComputeBoundingBox()
        {
            throw new NotImplementedException();
        }
        public override void ComputeBoundingSphere()
        {
            throw new NotImplementedException();
        }
        public override void ComputeVertexNormals(bool areaWeighted = false)
        {
            throw new NotImplementedException();
        }
    }
}
