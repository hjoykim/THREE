using ImGuiNET;
using System;
using THREE;

namespace THREEExample.Learning.Chapter05
{
    [Example("07.Basic-3D-Geometries-Cylinder", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic3D_Geometries_Cylinder : Basic3D_Geometries_Cube
    {
        float radiusTop = 20;
        float radiusBottom = 20;
        float height = 20;
        int radialSegments = 8;
        int heightSegments = 8;
        bool openEnded = false;
        float thetaStart = 0;
        float thetaLength = (float)Math.PI*2;
        public Basic3D_Geometries_Cylinder() : base()
        {

        }
        public override BufferGeometry BuildGeometry()
        {
            return new CylinderBufferGeometry(radiusTop, radiusBottom,height,radialSegments, heightSegments,openEnded, thetaStart,thetaLength);

        }
        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("radiusTop", ref radiusTop, -40, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("radiusBottom", ref radiusBottom, -40, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("height", ref height, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderInt("radialSegments", ref radialSegments, 1, 20)) rebuildGeometry = true;
            if (ImGui.SliderInt("heightSegments", ref heightSegments, 1, 20)) rebuildGeometry = true;
            if (ImGui.Checkbox("openEnded", ref openEnded)) rebuildGeometry = true;           
            if (ImGui.SliderFloat("thetaStart", ref thetaStart, 0, (float)Math.PI * 2)) rebuildGeometry = true;
            if (ImGui.SliderFloat("thetaLength", ref thetaLength, 0, (float)Math.PI * 2)) rebuildGeometry = true;

            return rebuildGeometry;
        }
    }
}
