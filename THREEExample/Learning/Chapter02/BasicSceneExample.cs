using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Controls;
using THREE.Geometries;
using THREE.Lights;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter02
{
    [Example("01-Basic Scene", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class BasicSceneExample : Example
    {
        public TrackballControls controls;
        public Mesh plane;
        public PlaneGeometry planeGeometry;
        public Camera camera;
        public Scene scene;

        public ImGuiManager imguiManager;
        private float rotationSpeed = 0.005f;
        private int numOfObjects = 0;

        public BasicSceneExample() : base()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
        }

        private void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.X = -30;
            camera.Position.Y = 40;
            camera.Position.Z = 30;
            camera.LookAt(THREE.Math.Vector3.Zero());
        }
        private void InitRenderer()
        {
            this.renderer.SetClearColor(new THREE.Math.Color().SetHex(0xEEEEEE), 1);
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        private void InitCameraController()
        {
            controls = new TrackballControls(this.glControl, camera);
            controls.StaticMoving = false;
            controls.RotateSpeed = 4.0f;
            controls.ZoomSpeed = 3;
            controls.PanSpeed = 3;
            controls.NoZoom = false;
            controls.NoPan = false;
            controls.NoRotate = false;
            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.3f;
        }

        public override void Load(GLControl glControl)
        {
            base.Load(glControl);

            InitRenderer();

            InitCamera();

            InitCameraController();

            imguiManager = new ImGuiManager(this.glControl);

            planeGeometry = new PlaneGeometry(60, 40, 1, 1);
            MeshPhongMaterial planeMaterial = new MeshPhongMaterial() { Color = new THREE.Math.Color().SetHex(0xffffff) };

            plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            plane.Rotation.X = (float)(-0.5 * System.Math.PI);
            plane.Position.X = 0;
            plane.Position.Y = 0;
            plane.Position.Z = 0;

            scene.Add(plane);

            var ambientLight = new AmbientLight(new THREE.Math.Color().SetHex(0x0c0c0c));
            scene.Add(ambientLight);

            var spotLight = new SpotLight(new THREE.Math.Color().SetHex(0xffffff));
            spotLight.Position.Set(-40, 60, -10);
            spotLight.CastShadow = true;

            scene.Add(spotLight);


        }
        public override void Render()
        {
            controls.Update();

            scene.Traverse(o =>
            {
                if (o is Mesh && !plane.Equals(o))
                {
                    o.Rotation.X += rotationSpeed;
                    o.Rotation.Y += rotationSpeed;
                    o.Rotation.Z += rotationSpeed;
                }
            });

            this.renderer.Render(scene, camera);

            ShowGUIControls();
        }

        public void Add()
        {
            var cubeSize = (float)MathUtils.random.NextDouble() * 3;
            cubeSize = (int)System.Math.Ceiling((decimal)cubeSize);
            var cubeGeometry = new BoxGeometry(cubeSize, cubeSize, cubeSize);
            var cubeMaterial = new MeshPhongMaterial() { Color = new THREE.Math.Color().Random() };
            var cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;
            //cube.Name = "cube-" + BasicScene.scene.Children.Count;

            cube.Position.X = -30 + (float)System.Math.Round(MathUtils.random.NextDouble() * planeGeometry.parameters.Width);
            cube.Position.Y = (float)System.Math.Round(MathUtils.random.NextDouble() * 5);
            cube.Position.Z = -20 + (float)System.Math.Round(MathUtils.random.NextDouble() * planeGeometry.parameters.Height);
            this.scene.Add(cube);
            numOfObjects++;
        }

        public void Remove()
        {
            var allChildren = scene.Children;
            var index = allChildren.Count - 1;

            if (index < 0) return;

            var lastObject = allChildren[allChildren.Count - 1];

            if (lastObject is Mesh)
            {
                scene.Remove(lastObject);
                //NumberOfObjects.Content = BasicScene.scene.Children.Count.ToString();
            }
            numOfObjects--;
        }
        private void ShowGUIControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            ImGui.Text("This is Rotation Speed Control Box");

            ImGui.SliderFloat("RotationSpeed", ref rotationSpeed, 0.0f, 0.5f);

            if(ImGui.Button("Draw"))
            {
                numOfObjects++;
            }

            if(ImGui.Button("Add Cube"))
            {
                Add();
            }

            if(ImGui.Button("Remove Cube"))
            {
                Remove();
            }

            ImGui.Text("Number Of Objects:" + numOfObjects.ToString());

            ImGui.End();
            ImGui.Render();
            imguiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
        public override void Resize(Size clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
}
