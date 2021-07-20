using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;

namespace THREE.Objects
{
    public class Group : Object3D
    {
        public List<string> MaterialLibraries;

        public Group()
            : base()
        {
        }
    }
}
