using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Materials;

namespace THREE.Core
{
    public class GLObject
    {
        public long Id;

        public BaseGeometry buffer;

        public Object3D object3D;

        public Material material;

        public float z;

        public bool render;
    }
}
