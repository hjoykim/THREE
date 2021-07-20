using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;

namespace THREE.Objects
{
    public class Bone : Object3D
    {
        public Bone() : base()
        {
            this.type = "Bone";
        }
    }
}
