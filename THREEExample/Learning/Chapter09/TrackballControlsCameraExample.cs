using System;
using THREE;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter09
{
    [Example("03-Trackball-controls-camera", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class TrackballControlsCameraExample : Example
    {
        Scene scene;

        Camera camera;

        TrackballControls controls;

        Mesh mesh;

        public TrackballControlsCameraExample() : base()
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
            camera.LookAt(Vector3.Zero());
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

        public override void Load(GLControl control)
        {
            base.Load(control);

            InitRenderer();

            InitCamera();

            InitCameraController();

            DemoUtils.InitDefaultLighting(scene);

            OBJLoader loader = new OBJLoader();

            var city = loader.Load(@"../../../../assets/models/city/city.obj");
            DemoUtils.SetRandomColors(city);
            scene.Add(city);

        }
        
        public override void Render()
        {
            controls.Update();
            this.renderer.Render(scene, camera);
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
}
