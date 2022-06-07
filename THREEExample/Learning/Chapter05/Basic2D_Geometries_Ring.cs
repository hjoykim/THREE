using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Geometries;
using THREE.Objects;

namespace THREEExample.Learning.Chapter05
{
    [Example("03.Basic-2D-Geometries-Ring", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic2D_Geometries_Ring : Basic2D_Geometries_Plane
    {
        float innerRadius = 3;
        float outerRadius = 10;
        int thetaSegments = 8;
        int phiSegments = 8;
        float thetaStart = 0;
        float thetaLength = (float)Math.PI * 2;

        public Basic2D_Geometries_Ring() : base()
        {

        }
        public override BufferGeometry BuildGeometry()
        {
            return new RingBufferGeometry(innerRadius, outerRadius, thetaSegments, phiSegments, thetaStart,thetaLength);
        }
       
        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("innerRadius", ref innerRadius, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("outerRadius", ref outerRadius, 0, 100)) rebuildGeometry = true;
            if (ImGui.SliderInt("thetaSegments", ref thetaSegments, 1, 40)) rebuildGeometry = true;
            if (ImGui.SliderInt("phiSegments", ref phiSegments, 1, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("thetaStart", ref thetaStart, 0, (float)Math.PI*2)) rebuildGeometry = true;
            if (ImGui.SliderFloat("thetaLength", ref thetaLength, 0, (float)Math.PI*2)) rebuildGeometry = true;

            return rebuildGeometry;
        }
    }
}
