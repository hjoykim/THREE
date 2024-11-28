using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;

namespace THREEExample.ThreeJs.Loader
{
    [Example("LoaderObj_mtl", ExampleCategory.ThreeJs, "loader")]
    public class LoaderObjMTLExample : GradientBackgroundShaderExample
    {

        public LoaderObjMTLExample() :base() { }

        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 45;
            camera.Near = 0.1f;
            camera.Far = 20;
            camera.Position.Set(0, 0, 2.5f);
        }
        public override void BuildScene()
        {
            renderer.SetClearColor(0x000000);

            AssimpLoader loader = new AssimpLoader();
            var obj = loader.Load("../../../../assets/models/obj//male02/male02.obj");
            //var obj = loader.Load("../../../../assets/models/obj//O{22}.obj");
            //var obj = loader.Load("../../../../assets/models/obj/dolphins.obj");
            obj.Traverse((m) =>
            {
                if (m is Mesh)
                {
                    m.Material.FlatShading = false;
                    (m.Geometry as BufferGeometry).deleteAttribute("normal");
                    var geometry = BufferGeometryUtils.MergeVertices(m.Geometry as BufferGeometry);
                    geometry.ComputeVertexNormals();
                    m.Geometry = geometry;
                }
            });
            obj.Position.Y = -0.95f;
            obj.Scale.SetScalar(0.01f);
            scene.Add(obj);

        }
        public override void Init()
        {
            base.Init();
            BuildScene();
        }
       
    }
}
