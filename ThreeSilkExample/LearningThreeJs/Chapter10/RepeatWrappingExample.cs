using ImGuiNET;
using System.Diagnostics;
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;

namespace THREE.Silk.Example
{
    [Example("21-repeat-wrapping", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class RepeatWrappingExample : TemplateExample
    {
        Mesh cubeMesh, sphereMesh, polyhedronMesh;

        THREE.Vector2 repeat = new THREE.Vector2(1, 1);

        bool repeatWrapping = true;

        public RepeatWrappingExample() : base()
        {

        }
        public override void Init()
        {
            base.Init();

            AddGuiControlsAction = () =>
            {
                foreach (var item in materialsLib)
                {

                    AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                    AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
                }

                ImGui.SliderFloat("repeatX", ref repeat.X, -4, 4);
                ImGui.SliderFloat("repeatY", ref repeat.Y, -4, 4);
                ImGui.Checkbox("repeatWrapping", ref repeatWrapping);
            };
        }
        public override void SetGeometryWithTexture()
        {
            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -10;


            scene.Add(new AmbientLight(new THREE.Color(0x444444)));

            var polyhedron = new IcosahedronBufferGeometry(8, 0);
            polyhedronMesh = AddGeometry(scene, polyhedron, "polyhedron", TextureLoader.Load("../../../../assets/textures/general/metal-rust.jpg"));
            polyhedronMesh.Position.X = 20;

            var sphere = new SphereBufferGeometry(5, 20, 20);
            sphereMesh = AddGeometry(scene, sphere, "sphere", TextureLoader.Load("../../../../assets/textures/general/floor-wood.jpg"));

            var cube = new BoxBufferGeometry(10, 10, 10);
            cubeMesh = AddGeometry(scene, cube, "cube", TextureLoader.Load("../../../../assets/textures/general/brick-wall.jpg"));
            cubeMesh.Position.X = -20;
        }

        private void UpdateRepeat()
        {

            cubeMesh.Material.Map.Repeat.Set(repeat.X, repeat.Y);
            sphereMesh.Material.Map.Repeat.Set(repeat.X, repeat.Y);
            polyhedronMesh.Material.Map.Repeat.Set(repeat.X, repeat.Y);


            if (repeatWrapping)
            {
                cubeMesh.Material.Map.WrapS = Constants.RepeatWrapping;
                cubeMesh.Material.Map.WrapT = Constants.RepeatWrapping;
                sphereMesh.Material.Map.WrapS = Constants.RepeatWrapping;
                sphereMesh.Material.Map.WrapT = Constants.RepeatWrapping;
                polyhedronMesh.Material.Map.WrapS = Constants.RepeatWrapping;
                polyhedronMesh.Material.Map.WrapT = Constants.RepeatWrapping;
            }
            else
            {
                cubeMesh.Material.Map.WrapS = Constants.ClampToEdgeWrapping;
                cubeMesh.Material.Map.WrapT = Constants.ClampToEdgeWrapping;
                sphereMesh.Material.Map.WrapS = Constants.ClampToEdgeWrapping;
                sphereMesh.Material.Map.WrapT = Constants.ClampToEdgeWrapping;
                polyhedronMesh.Material.Map.WrapS = Constants.ClampToEdgeWrapping;
                polyhedronMesh.Material.Map.WrapT = Constants.ClampToEdgeWrapping;
            }

            cubeMesh.Material.Map.NeedsUpdate = true;
            sphereMesh.Material.Map.NeedsUpdate = true;
            polyhedronMesh.Material.Map.NeedsUpdate = true;
        }
        public override void Render()
        {
            if (!this.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            this.renderer.Render(scene, camera);
            UpdateRepeat();
            cubeMesh.Rotation.Y += 0.01f;
            sphereMesh.Rotation.Y += 0.01f;
            polyhedronMesh.Rotation.Y += 0.01f;


        }
        

    }
}
