﻿using ImGuiNET;
using THREE;
using THREE.Silk;
namespace THREE.Silk.Example
{
    [Example("10.Basic-3D-Geometries-Torus-knot", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic3D_Geometries_Torus_knot : Basic3D_Geometries_Cube
    {
        float radius = 1;
        float tube = 0.3f;
        int radialSegments = 8;
        int tubularSegments = 64;
        int p = 2;
        int q = 3;

        public Basic3D_Geometries_Torus_knot() : base() { }

        public override BufferGeometry BuildGeometry()
        {
            return new TorusKnotBufferGeometry(radius,tube,tubularSegments,radialSegments,p,q);
        }
        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("radius", ref radius, 0, 10)) rebuildGeometry = true;
            if (ImGui.SliderFloat("tube", ref tube, 0, 10)) rebuildGeometry = true;
            if (ImGui.SliderInt("radialSegments", ref radialSegments, 0, 400)) rebuildGeometry = true;
            if (ImGui.SliderInt("tubularSegments", ref tubularSegments, 1, 200)) rebuildGeometry = true;
            if (ImGui.SliderInt("p", ref p, 1, 10)) rebuildGeometry = true;
            if (ImGui.SliderInt("q", ref q, 1, 15)) rebuildGeometry = true;

            return rebuildGeometry;
        }
    }
}
