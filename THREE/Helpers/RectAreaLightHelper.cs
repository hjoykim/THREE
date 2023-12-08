
using System.Collections.Generic;

namespace THREE
{
    using System;

    public class RectAreaLightHelper : Line
    {
        private Light Light;

        private Color? Color;

        public RectAreaLightHelper(Light light, Color? color=null) : base()
        {
            this.type = "RectAreaLightHelper";
            this.Light = light;
            this.Light.UpdateMatrixWorld();

            //this.Matrix = this.Light.MatrixWorld;
            //this.MatrixAutoUpdate = false;

            this.Color = color;

            var positions = new List<float>() { 1, 1, 0,  -1, 1, 0,  -1, -1, 0,  1, -1, 0,  1, 1, 0 };

            var geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(positions.ToArray(), 3));
            geometry.ComputeBoundingSphere();
            
            this.InitGeometry(geometry, new List<Material>() { new LineBasicMaterial() { Fog = false, ToneMapped = false } });

            var positions2 = new List<float>() { 1, 1, 0,  -1, 1, 0,  -1, -1, 0,  1, 1, 0,  -1, -1, 0,  1, -1, 0 };

            var geometry2 = new BufferGeometry();
            geometry2.SetAttribute("position", new BufferAttribute<float>(positions2.ToArray(), 3));
            geometry2.ComputeBoundingSphere();

            this.Add(new Mesh(geometry2, new MeshBasicMaterial() { Side = Constants.BackSide, Fog = false }));
            this.Update();
        }

        public void Update()
        {
            this.Light.UpdateMatrixWorld();
            this.Scale.Set(0.5f * this.Light.Width, 0.5f * this.Light.Height, 1f);

            if (this.Color != null)
            {
                this.Material.Color = this.Color;
                this.Children[0].Material.Color = this.Color;
            }
            else
            {
                this.Material.Color = this.Light.Color.MultiplyScalar(this.Light.Intensity);

                // prevent hue shift
                Color c = (Color)this.Material.Color;
                var max = Math.Max(c.R, Math.Max(c.G, c.B));
                if (max > 1) c.MultiplyScalar(1 / max);

                this.Children[0].Material.Color = this.Material.Color;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            this.Geometry.Dispose();
            this.Material.Dispose();
            this.Children[0].Geometry.Dispose();
            this.Children[0].Material.Dispose();
        }
    }
}
