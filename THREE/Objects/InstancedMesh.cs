using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;

namespace THREE.Objects
{
    public class InstancedMesh : Mesh
    {
        public BufferAttribute<float> InstanceMatrix;

        public BufferAttribute<float> InstanceColor;

        public int count;

        public InstancedMesh(Geometry geometry, List<Material> material, int count) : base(geometry,material)
        {
            this.type = "InstancedMesh";

            this.count = count;
        }
    }
}
