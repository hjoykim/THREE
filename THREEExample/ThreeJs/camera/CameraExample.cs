using OpenTK;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
namespace THREEExample.Three.camera
{
    [Example("camera", ExampleCategory.ThreeJs, "camera")]
    public class CameraExample : Example
    {

        int SCREEN_WIDTH;
        int SCREEN_HEIGHT;
        Camera cameraPerspective;
        Camera cameraOrtho;
        int frustumSize = 600;
        Camera activeCamera;
        CameraHelper activeHelper, cameraPerspectiveHelper, cameraOrthoHelper;
        Group cameraRig;
        Mesh mesh, mesh2, mesh3;

        public CameraExample() : base() { }
        public override void InitCamera()
        {
            camera.Fov = 50.0f;
            camera.Aspect = this.glControl.AspectRatio*0.5f;
            camera.Near = 1f;
            camera.Far = 10000.0f;
            camera.Position.Set(0, 0, 2500);

            cameraPerspective = new PerspectiveCamera(50, 0.5f * glControl.AspectRatio, 150, 1000);

            cameraPerspectiveHelper = new CameraHelper(cameraPerspective);
            scene.Add(cameraPerspectiveHelper);

            cameraOrtho = new OrthographicCamera(0.5f * frustumSize * glControl.AspectRatio / -2, 0.5f * frustumSize * glControl.AspectRatio / 2, frustumSize / 2, frustumSize / -2, 150, 1000);

            cameraOrthoHelper = new CameraHelper(cameraOrtho);
            scene.Add(cameraOrthoHelper);

            activeCamera = cameraPerspective;
            activeHelper = cameraPerspectiveHelper;

            cameraOrtho.Rotation.Y = (float)Math.PI;
            cameraPerspective.Rotation.Y = (float)Math.PI;
        }
        public override void InitCameraController()
        {
        }
        public override void InitLighting()
        {
        }
        private void CreateObject()
        {
            cameraRig = new Group();

            cameraRig.Add(cameraPerspective);
            cameraRig.Add(cameraOrtho);

            scene.Add(cameraRig);

            mesh = new Mesh(
                    new SphereBufferGeometry(100, 16, 8),
                    new MeshBasicMaterial() { Color = Color.Hex(0xffffff), Wireframe = true }
                );
            scene.Add(mesh);

            mesh2 = new Mesh(
                new SphereBufferGeometry(50, 16, 8),
                new MeshBasicMaterial() { Color = Color.Hex(0x00ff00), Wireframe = true }
                );
            mesh2.Position.Y = 150;
            mesh.Add(mesh2);

            mesh3 = new Mesh(
                    new SphereBufferGeometry(5, 16, 8),
                    new MeshBasicMaterial() { Color = Color.Hex(0x0000ff), Wireframe = true }
                );
            mesh3.Position.Z = 150;
            cameraRig.Add(mesh3);

            var geometry = new BufferGeometry();
            var vertices = new List<float>();

            for (var i = 0; i < 10000; i++)
            {

                vertices.Add(MathUtils.RandFloatSpread(2000)); // x
                vertices.Add(MathUtils.RandFloatSpread(2000)); // y
                vertices.Add(MathUtils.RandFloatSpread(2000)); // z

            }

            geometry.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));

            var particles = new Points(geometry, new PointsMaterial() { Color = Color.Hex(0x888888) });
            scene.Add(particles);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.AutoClear = false;
        }
        public override void Init()
        {
            base.Init();

            CreateObject();
            SCREEN_WIDTH = glControl.Width;
            SCREEN_HEIGHT = glControl.Height;
            renderer.SetSize(SCREEN_WIDTH, SCREEN_HEIGHT);
            this.KeyDown += OnKeyDown;
        }

        float r = 0;
        public override void Render()
        {
            r += 0.0005f;

            mesh.Position.X = 700 * (float)Math.Cos(r);
            mesh.Position.Z = 700 * (float)Math.Sin(r);
            mesh.Position.Y = 700 * (float)Math.Sin(r);

            mesh.Children[0].Position.X = 70 * (float)Math.Cos(2 * r);
            mesh.Children[0].Position.Z = 70 * (float)Math.Sin(r);

            if (activeCamera == cameraPerspective)
            {

                cameraPerspective.Fov = 35 + 30 * (float)Math.Sin(0.5 * r);
                cameraPerspective.Far = mesh.Position.Length();
                cameraPerspective.UpdateProjectionMatrix();

                cameraPerspectiveHelper.Update();
                cameraPerspectiveHelper.Visible = true;

                cameraOrthoHelper.Visible = false;

            }
            else
            {

                cameraOrtho.Far = mesh.Position.Length();
                cameraOrtho.UpdateProjectionMatrix();

                cameraOrthoHelper.Update();
                cameraOrthoHelper.Visible = true;

                cameraPerspectiveHelper.Visible = false;

            }

            cameraRig.LookAt(mesh.Position);

            renderer.Clear();

            activeHelper.Visible = false;

            renderer.SetViewport(0, 0, SCREEN_WIDTH / 2, SCREEN_HEIGHT);
            renderer.Render(scene, activeCamera);

            activeHelper.Visible = true;

            renderer.SetViewport(SCREEN_WIDTH / 2, 0, SCREEN_WIDTH / 2, SCREEN_HEIGHT);
            renderer.Render(scene, camera);
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            SCREEN_WIDTH = glControl.Width;
            SCREEN_HEIGHT = glControl.Height;

            float aspect = glControl.AspectRatio;
            
            renderer.SetSize(SCREEN_WIDTH, SCREEN_HEIGHT);

            camera.Aspect = 0.5f * aspect;
            camera.UpdateProjectionMatrix();

            cameraPerspective.Aspect = 0.5f * aspect;
            cameraPerspective.UpdateProjectionMatrix();

            cameraOrtho.Left = -0.5f * frustumSize * aspect / 2;
            cameraOrtho.CameraRight = 0.5f * frustumSize * aspect / 2;
            cameraOrtho.Top = frustumSize / 2;
            cameraOrtho.Bottom = -frustumSize / 2;
            cameraOrtho.UpdateProjectionMatrix();
        }

        public void OnKeyDown(object sender,KeyboardKeyEventArgs key)
        {
            switch (key.Key)
            {
                case Keys.O:
                    activeCamera = cameraOrtho;
                    activeHelper = cameraOrthoHelper;
                    break;
                case Keys.P:
                    activeCamera = cameraPerspective;
                    activeHelper = cameraPerspectiveHelper;
                    break;
            }
        }
    }
}
