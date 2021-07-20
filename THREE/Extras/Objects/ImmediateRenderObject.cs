using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;

namespace THREE.Extras.Objects
{
    public class ImmediateRenderObject : Object3D
    {
        public ImmediateRenderObject(Material material) : base()
        {
            this.Material = material;
        }

        protected ImmediateRenderObject(ImmediateRenderObject other) : base(other)
        {
            this.Material = (Material)other.Material.Clone();
        }

        public void Render(Action renderCAllback)
        {
        }
    }
}
