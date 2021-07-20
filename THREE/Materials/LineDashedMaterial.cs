using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Materials
{
    public class LineDashedMaterial : LineBasicMaterial
    {
        public float Scale = 1;
        
        public float DashSize = 3;
        
        public float GapSize = 1;

        public LineDashedMaterial() : base()
        {
            this.type = "LineDashedMaterial";
        }
    }
}
