
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("05-FirstPerson-camera", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class FirstPersonCameraExample : Example
    {
        new FirstPersonControls controls;

        public FirstPersonCameraExample() : base()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
            stopWatch.Start();
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(new Color().SetHex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }

        public override void InitCameraController()
        {
            controls = new FirstPersonControls(this, camera);
            controls.LookSpeed = 0.4f;
            controls.MovementSpeed = 20;
            controls.LookVertical = true;
            controls.ConstrainVertical = true;
            controls.VerticalMin = 1.0f;
            controls.VerticalMax = 2.0f;
            controls.lon = -150;
            controls.lat = 120;
        }
        public override void InitLighting()
        {
            base.InitLighting();
            DemoUtils.InitDefaultLighting(scene);
        }
        public override void Init()
        {
            base.Init();

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

    }
}
