using ImGuiNET;
using System;
using THREE;

namespace THREEExample.Learning.Chapter05
{
    [Example("06.Basic-3D-Geometries-Sphere", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic3D_Geometries_Sphere : Basic3D_Geometries_Cube
    {
        float radius = 4;
        int widthSegments = 10;
        int heightSegments = 10;
        float phiStart = 0;
        float phiLength = (float)Math.PI*2;
        float thetaStart = 0;
        float thetaLength = (float)Math.PI;
        public Basic3D_Geometries_Sphere() : base()
        {

        }
        public override BufferGeometry BuildGeometry()
        {
            return new SphereBufferGeometry(radius, widthSegments, heightSegments,phiStart,phiLength,thetaStart,thetaLength);
        }

        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("radius", ref radius, 0, 40)) rebuildGeometry = true;           
            if (ImGui.SliderInt("widthSegments", ref widthSegments, 0, 20)) rebuildGeometry = true;
            if (ImGui.SliderInt("heightSegments", ref heightSegments, 0, 20)) rebuildGeometry = true;
            if (ImGui.SliderFloat("phiStart", ref phiStart, 0, (float)Math.PI * 2)) rebuildGeometry = true;
            if (ImGui.SliderFloat("phiLength", ref phiLength, 0, (float)Math.PI * 2)) rebuildGeometry = true;
            if (ImGui.SliderFloat("thetaStart", ref thetaStart, 0, (float)Math.PI * 2)) rebuildGeometry = true;
            if (ImGui.SliderFloat("thetaLength", ref thetaLength, 0, (float)Math.PI*2)) rebuildGeometry = true;

            return rebuildGeometry;
        }
    }
}
