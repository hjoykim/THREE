using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Geometries;

namespace THREEExample.Learning.Chapter05
{
    [Example("09.Basic-3D-Geometries-Torus", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic3D_Geometries_Torus : Basic3D_Geometries_Cube
    {
        float radius = 10;
        float tube = 10;
        int radialSegments = 8;
        int tubularSegments = 6;
        float arc = (float)Math.PI * 2;
        public Basic3D_Geometries_Torus() : base()
        {

        }

        public override BufferGeometry BuildGeometry()
        {
            return new TorusBufferGeometry(radius, tube, radialSegments, tubularSegments, arc);
        }

        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("radius", ref radius, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("tube", ref tube, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderInt("radialSegments", ref radialSegments, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderInt("tubularSegments", ref tubularSegments, 1, 20)) rebuildGeometry = true;
            if (ImGui.SliderFloat("arc", ref arc, 0, (float)Math.PI * 2)) rebuildGeometry = true;

            return rebuildGeometry;
        }
    }
}
