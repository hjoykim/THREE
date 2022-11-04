using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Objects;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter10
{
    [Example("21-repeat-wrapping", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class RepeatWrappingExample : TemplateExample
    {
        Mesh cubeMesh, sphereMesh, polyhedronMesh;

        THREE.Math.Vector2 repeat = new THREE.Math.Vector2(1, 1);

        bool repeatWrapping = true;

        public RepeatWrappingExample() : base()
        {

        }
        public override void Load(GLControl control)
        {
            Debug.Assert(null != control);

            glControl = control;
            this.renderer = new THREE.Renderers.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            InitRenderer();

            InitCamera();

            InitCameraController();

            imguiManager = new ImGuiManager(this.glControl);

            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -10;

            DemoUtils.InitDefaultLighting(scene);

            scene.Add(new AmbientLight(new THREE.Math.Color(0x444444)));

            var polyhedron = new IcosahedronBufferGeometry(8, 0);
            polyhedronMesh = AddGeometry(scene, polyhedron, "polyhedron", TextureLoader.Load("../../../assets/textures/general/metal-rust.jpg"));
            polyhedronMesh.Position.X = 20;

            var sphere = new SphereBufferGeometry(5, 20, 20);
            sphereMesh = AddGeometry(scene, sphere, "sphere", TextureLoader.Load("../../../assets/textures/general/floor-wood.jpg"));

            var cube = new BoxBufferGeometry(10, 10, 10);
            cubeMesh = AddGeometry(scene, cube, "cube", TextureLoader.Load("../../../assets/textures/general/brick-wall.jpg"));
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
            if (!imguiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            this.renderer.Render(scene, camera);
            UpdateRepeat();
            cubeMesh.Rotation.Y += 0.01f;
            sphereMesh.Rotation.Y += 0.01f;
            polyhedronMesh.Rotation.Y += 0.01f;

            ShowGUIControls();
        }
        public override void ShowGUIControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            foreach (var item in materialsLib)
            {

                AddBasicMaterialSettings(item.Value, item.Key + "-THREE.Material");
                AddSpecificMaterialSettings(item.Value, item.Key + "-THREE.MeshStandardMaterial");
            }

            ImGui.SliderFloat("repeatX", ref repeat.X, -4, 4);
            ImGui.SliderFloat("repeatY", ref repeat.Y, -4, 4);
            ImGui.Checkbox("repeatWrapping", ref repeatWrapping);
            ImGui.End();
            ImGui.Render();

            imguiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }

    }
}
