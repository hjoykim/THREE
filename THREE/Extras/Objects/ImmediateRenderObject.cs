using System;

namespace THREE
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
