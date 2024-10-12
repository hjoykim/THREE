using ImGuiNET;
using System;
using THREE;
using THREE.Silk;
namespace THREE.Silk.Example
{
    [Example("02.Basic-2D-Geometries-Circle", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic2D_Geometries_Circle : Basic2D_Geometries_Plane
    {
        float radius = 4;
        float thetaStart = (float)(0.3f * Math.PI * 2);
        float thetaLength = (float)(0.3f * Math.PI * 2);
        int segments = 10;
        public Basic2D_Geometries_Circle() : base()
        {

        }
        public override BufferGeometry BuildGeometry()
        {
            return new CircleBufferGeometry(radius, segments,thetaStart,thetaLength);

        }
        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("radius", ref radius, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderInt("segments", ref segments, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("thetaStart", ref thetaStart, 0, (float)(2*Math.PI))) rebuildGeometry = true;
            if (ImGui.SliderFloat("thetaLength", ref thetaLength, 0, (float)(2*Math.PI))) rebuildGeometry = true;

            return rebuildGeometry;
        }

    }
}
