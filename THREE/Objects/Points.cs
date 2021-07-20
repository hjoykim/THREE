using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;
using THREE.Math;

namespace THREE.Objects
{
    public class Points : Object3D
    {
        public Points(Geometry geometry=null, Material material=null)
        {
            this.type = "Points";

            this.Geometry = geometry ?? new BufferGeometry();
            this.Material = material ?? new PointsMaterial { Color = new Color().SetHex(0xFFFFFF) };
            
            this.UpdateMorphTargets();
        }

        public Points(Geometry geometry=null, List<Material> materials=null)
        {
            this.type = "Points";

            this.Geometry = geometry ?? new BufferGeometry();

            if (materials == null)
            {
                this.Material = new PointsMaterial { Color = new Color().SetHex(0xFFFFFF) };
                this.Materials.Add(Material);
            }
            else
            {
                this.Materials = materials;
                if (this.Materials.Count > 0)
                    this.Material = this.Materials[0];
            }
        }

        public void UpdateMorphTargets()
        {

        }
    }
}
