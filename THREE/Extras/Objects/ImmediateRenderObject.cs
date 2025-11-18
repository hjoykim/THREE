using System;

namespace THREE
{
    [Serializable]
    public class ImmediateRenderObject : Object3D
    {
        public ImmediateRenderObject(Material material) : base()
        {
            this.Material = material;
        }

        public void Render(Action renderCAllback)
        {
        }
    }
}
