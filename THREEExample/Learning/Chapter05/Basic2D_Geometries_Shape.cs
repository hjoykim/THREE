using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Extras.Core;
using THREE.Geometries;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter05
{
    [Example("04.Basic-2D-Geometries-Shape", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic2D_Geometries_Shape : Basic2D_Geometries_Plane
    {
        int curveSegments = 12;
        public Basic2D_Geometries_Shape() : base()
        {

        }
        public override BufferGeometry BuildGeometry()
        {
            return new ShapeBufferGeometry(new List<Shape>() { DemoUtils.DrawShape() }, curveSegments);
        }
        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderInt("curveSegments", ref curveSegments, 1, 100)) rebuildGeometry = true;

            return rebuildGeometry;
        }
    }
}
