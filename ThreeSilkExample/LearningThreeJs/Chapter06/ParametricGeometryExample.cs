using ImGuiNET;
using System;
using THREE;
using THREE.Silk;
namespace THREE.Silk.Example
{
    [Example("06.Parametric-Geometry", ExampleCategory.LearnThreeJS, "Chapter06")]
    public class ParametricGeometryExample : AdvancedGeometriesConvex
    {
        int renderIndex = 0;
        int slices = 50;
        int stacks = 50;
        public ParametricGeometryExample() : base()
        {

        }
        public override void Init()
        {
            base.Init();
            redrawButtonEnabled = false;
        }
        public override BufferGeometry BuildGeometry()
        {
            return new ParametricBufferGeometry(RadialWave,slices,stacks);
        }
        private Vector3 RadialWave(float u, float v, Vector3 optionalTarget)
        {
            var result = optionalTarget != null ? optionalTarget : new Vector3();
            var r = 50;

            var x = (float)Math.Sin(u) * r;
            var z = (float)Math.Sin(v / 2) * 2 * r;
            var y = (float)(Math.Sin(u * 4 * Math.PI) + (float)Math.Cos(v * 2 * Math.PI)) * 2.8f;

            return result.Set(x, y, z);
        }
        private Vector3 Klein(float u, float v, Vector3 optionalTarget)
        {
            var result = optionalTarget != null ? optionalTarget : new Vector3();

            u *= (float)System.Math.PI;
            v *= 2 * (float)System.Math.PI;

            u = u * 2;
            float x, y, z;
            if (u < (float)System.Math.PI)
            {
                x = 3 * (float)System.Math.Cos(u) * (1 + (float)System.Math.Sin(u)) + (2 * (1 - (float)System.Math.Cos(u) / 2)) * (float)System.Math.Cos(u) * (float)System.Math.Cos(v);
                z = -8 * (float)System.Math.Sin(u) - 2 * (1 - (float)System.Math.Cos(u) / 2) * (float)System.Math.Sin(u) * (float)System.Math.Cos(v);
            }
            else
            {
                x = 3 * (float)System.Math.Cos(u) * (1 + (float)System.Math.Sin(u)) + (2 * (1 - (float)System.Math.Cos(u) / 2)) * (float)System.Math.Cos(v + (float)System.Math.PI);
                z = -8 * (float)System.Math.Sin(u);
            }

            y = -2 * (1 - (float)System.Math.Cos(u) / 2) * (float)System.Math.Sin(v);

            return result.Set(x, y, z);
        }
        public override void Redraw()
        {
            scene.Remove(appliedMesh);
            materialsLib.Remove(appliedNormalMaterial.type);
            if (renderIndex == 0)
            {
                var geometry = new ParametricBufferGeometry(RadialWave, slices, stacks);
                geometry.Center();
                appliedMesh =new Mesh(geometry, appliedNormalMaterial);
            }
            else
            {
                var geometry = new ParametricBufferGeometry(Klein, slices, stacks);
                geometry.Center();
                appliedMesh = new Mesh(geometry, appliedNormalMaterial);

            }

            materialsLib.Add(appliedNormalMaterial.type, appliedNormalMaterial);
            scene.Add(appliedMesh);
        }
        public override void AddSettings()
        {
            bool redraw = false;
            if (ImGui.Combo("redenderFunction", ref renderIndex, "radialWave\0klein\0"))
            {
                Redraw();
            }
            if (ImGui.SliderInt("slices", ref slices, 10, 120)) redraw = true;
            if (ImGui.SliderInt("stacks", ref stacks, 10, 120)) redraw = true;

            if(redraw)
            {
                Redraw();
            }

            base.AddSettings();
           
          
        }
    }
}
