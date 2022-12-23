using System;
using System.Collections.Generic;
using THREE;
using OpenTK;
using THREEExample.Learning.Utils;
using ImGuiNET;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter09
{
    [Example("02-SelectingObject", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class SelectingObjectExample : Example
    {
        float step = 0.0f;
        float scalingStep = 0.0f;
        public Mesh cube;
        public Mesh sphere;
        public Mesh plane;
        public Mesh cylinder;

        float rotationSpeed = 0.02f;
        float bouncingSpeed = 0.03f;
        float scalingSpeed = 0.03f;

        Scene scene;

        Camera camera;

        TrackballControls controls;

        ImGuiManager imGuiManager;
        public SelectingObjectExample() : base()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
        }
        private void InitRenderer()
        {
            this.renderer.SetClearColor(new Color().SetHex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        private void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.X = -30;
            camera.Position.Y = 50;
            camera.Position.Z = 40;
            camera.LookAt(THREE.Vector3.Zero());
        }
        private void InitCameraController()
        {
            controls = new TrackballControls(this.glControl, camera);
            controls.StaticMoving = false;
            controls.RotateSpeed = 3.0f;
            controls.ZoomSpeed = 2;
            controls.PanSpeed = 2;
            controls.NoZoom = false;
            controls.NoPan = false;
            controls.NoRotate = false;
            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.2f;
        }
        public override void Load(GLControl glControl)
        {
            base.Load(glControl);

            this.glControl.MouseMove += MouseMove;
            this.glControl.MouseDown += MouseDown;

            InitRenderer();

            InitCamera();

            InitCameraController();

            imGuiManager = new ImGuiManager(this.glControl);

            DemoUtils.InitDefaultLighting(scene);

            var groundPlane = DemoUtils.AddGroundPlane(scene);

            groundPlane.Position.Y = 0;

            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshStandardMaterial() { Color = Color.Hex(0xff0000) };
            cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;
            cube.Position.Set(-10, 4, 0);
            scene.Add(cube);

            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshStandardMaterial() { Color = Color.Hex(0x7777ff) };
            sphere = new Mesh(sphereGeometry, sphereMaterial);
            sphere.Position.Set(20, 0, 2);
            sphere.CastShadow = true;
            scene.Add(sphere);

            var cylinderGeometry = new CylinderGeometry(2, 2, 20);
            var cylinderMaterial = new MeshStandardMaterial() { Color = Color.Hex(0x77ff77) };
            cylinder = new Mesh(cylinderGeometry, cylinderMaterial);
            cylinder.CastShadow = true;
            cylinder.Position.Set(0, 0, 1);

            scene.Add(cylinder);

            // add subtle ambient lighting
            var ambienLight = new AmbientLight(Color.Hex(0x353535));
            scene.Add(ambienLight);

            
        }

        public override void Render()
        {
            
            cube.Rotation.X += rotationSpeed;
            cube.Rotation.Y += rotationSpeed;
            cube.Rotation.Z += rotationSpeed;

            step += bouncingSpeed;

            sphere.Position.X = 20 + (10 * (float)(System.Math.Cos(step)));
            sphere.Position.Y = 2 + (10 * (float)System.Math.Abs(Math.Sin(step)));

            // scale the cylinder
            scalingStep += scalingSpeed;
            var scaleX = (float)Math.Abs(Math.Sin(scalingStep / 4));
            var scaleY = (float)Math.Abs(Math.Cos(scalingStep / 5));
            var scaleZ = (float)Math.Abs(Math.Sin(scalingStep / 7));
            cylinder.Scale.Set(scaleX, scaleY, scaleZ);

            if (!imGuiManager.ImWantMouse) controls.Update();

            this.renderer.Render(scene, camera);

            showControls();

        }

        public void MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {


            var vector = new THREE.Vector3(((float)e.X / this.glControl.Width) * 2 - 1, -((float)e.Y / this.glControl.Height) * 2 + 1, 0.5f);
            vector = vector.UnProject(camera);

            var raycaster = new Raycaster(camera.Position, vector.Sub(camera.Position).Normalize());
            var intersects = raycaster.IntersectObjects(new List<Object3D>() { sphere, cylinder, cube });

            if (intersects.Count > 0)
            {
                intersects[0].object3D.Material.Transparent = true;
                intersects[0].object3D.Material.Opacity = 0.1f;
            }
        }
        public void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
        private void showControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            ImGui.Text("This is Rotaion Speed Control box");
            ImGui.SliderFloat("RotationSpeed", ref rotationSpeed, 0.0f, 0.5f);
            ImGui.SliderFloat("bouncingSpeed", ref bouncingSpeed, 0.0f, 0.5f);
            ImGui.SliderFloat("scalingSpeed", ref scalingSpeed, 0.0f, 0.5f);
            ImGui.End();

            ImGui.Render();

            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
    }
}
