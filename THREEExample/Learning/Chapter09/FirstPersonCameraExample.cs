using OpenTK;
using THREE;
using THREEExample.Learning.Utils;
namespace THREEExample.Learning.Chapter09
{
    [Example("05-FirstPerson-camera", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class FirstPersonCameraExample : Example
    {
        Scene scene;

        Camera camera;

        FirstPersonControls controls;

        public FirstPersonCameraExample() : base()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
            stopWatch.Start();
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
            controls = new FirstPersonControls(this.glControl, camera);
            controls.LookSpeed = 0.4f;
            controls.MovementSpeed = 20;
            controls.LookVertical = true;
            controls.ConstrainVertical = true;
            controls.VerticalMin = 1.0f;
            controls.VerticalMax = 2.0f;
            controls.lon = -150;
            controls.lat = 120;
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
            var delta = stopWatch.ElapsedMilliseconds / 1000.0f;
            stopWatch.Reset();
            stopWatch.Start();
            controls.Update(delta);

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
