using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using THREEExample.Three.Geometries;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.Geometries
{
    [Example("WireframeGeometry", ExampleCategory.ThreeJs, "geometry")]
    public class WireframeGeometryExample : Example
    {
        float radius = 100;
        float detail = 4;
        int currentModel = 0;
        Material lineMaterial, meshMaterial;
        Texture texture;
        Light[] lights = new Light[3];
        Mesh mesh;
        LineSegments lineSegments;
        public WireframeGeometryExample() :base()
        { }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 50;
            camera.Near = 0.01f;
            camera.Far = 10000000;
            camera.Position.Set(2 * radius, 2 * radius, 2 * radius);
        }
        public override void InitLighting()
        {
            base.InitLighting();

            lights[0] = new PointLight(0xffffff, 1, 0);
            lights[1] = new PointLight(0xffffff, 1, 0);
            lights[2] = new PointLight(0xffffff, 1, 0);

            lights[0].Position.Set(0, 2 * radius, 0);
            lights[1].Position.Set(2 * radius, -2 * radius, 2 * radius);
            lights[2].Position.Set(-2 * radius, -2 * radius, -2 * radius);

            scene.Add(lights[0]);
            scene.Add(lights[1]);
            scene.Add(lights[2]);

            //

            scene.Add(new AxesHelper((int)radius * 5));
        }
        private void CreateGeometry()
        {
            lineMaterial = new LineBasicMaterial { Color=Color.Hex(0xaaaaaa),Transparent=true,Opacity=0.8f};
            meshMaterial = new MeshPhongMaterial { Color=Color.Hex(0xffffff), Emissive = Color.Hex(0x111111) };
            texture = TextureLoader.Load("../../../../assets/textures/uv_grid_opengl.jpg");
            texture.WrapS = Constants.RepeatWrapping;
            texture.WrapT = Constants.RepeatWrapping;

            var geom = NewGeometry("Icosahedron");

            mesh = new Mesh(geom, meshMaterial);
            lineSegments = new LineSegments(new THREE.WireframeGeometry(geom), lineMaterial);
            lineSegments.Visible = false;

            scene.Add(mesh);
            scene.Add(lineSegments);
        }
        private BufferGeometry NewGeometry(string model)
        {
            var defaultGeometry = new IcosahedronBufferGeometry(radius, detail);
            switch (model)
            {

                case "Icosahedron":
                    return defaultGeometry;
                case "Cylinder":
                    return new CylinderBufferGeometry(radius, radius, radius * 2, (int)detail * 6);
                case "Teapot":
                    return new TeapotBufferGeometry((int)radius, (int)detail * 3, true, true, true, true, true);
                case "TorusKnot":
                    return new TorusKnotBufferGeometry(radius, 10, detail * 20, detail * 6, 3, 4);
                default:
                    return defaultGeometry;
            }
        }
        private void UpdateGeometry(string model)
        {
            var geom = NewGeometry(model);
            lineSegments.Geometry.Dispose();
            mesh.Geometry.Dispose();

            lineSegments.Geometry = new WireframeGeometry(geom);
            mesh.Geometry = geom;
        }
        public override void Init()
        {
            base.Init();

            CreateGeometry();

            string[] models = new string[] { "Icosahedron", "Cylinder", "Teapot", "TorusKnot" };
            AddGuiControlsAction = () =>
            {
                if(ImGui.Combo("model",ref currentModel,models,4))
                {
                    UpdateGeometry(models[currentModel]);
                }
                ImGui.Checkbox("wireframe", ref lineSegments.Visible);
            };
        }

    }
}
