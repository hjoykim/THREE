using ImGuiNET;
using THREE;
using THREE.Silk;
namespace THREE.Silk.Example
{
    [Example("05.Basic-3D-Geometries-Cube", ExampleCategory.LearnThreeJS, "Chapter05")]
    public class Basic3D_Geometries_Cube : Basic2D_Geometries_Plane
    {
        float width = 4;
        float height = 10;
        float depth = 10;
        int widthSegments = 4;
        int heightSegments = 4;
        int depthSegments = 4;
        public Basic3D_Geometries_Cube() : base()
        {

        }
        public override BufferGeometry BuildGeometry()
        {
            return new BoxBufferGeometry(width, height, depth,widthSegments, heightSegments,depthSegments);

        }
        public override bool AddGeometryParameter()
        {
            bool rebuildGeometry = false;
            if (ImGui.SliderFloat("width", ref width, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("height", ref height, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderFloat("depth", ref depth, 0, 40)) rebuildGeometry = true;
            if (ImGui.SliderInt("widthSegments", ref widthSegments, 0, 10)) rebuildGeometry = true;
            if (ImGui.SliderInt("heightSegments", ref heightSegments, 0, 10)) rebuildGeometry = true;
            if (ImGui.SliderInt("depthSegments", ref depthSegments, 0, 10)) rebuildGeometry = true;

            return rebuildGeometry;
        }
       
    }
}
