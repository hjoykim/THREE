using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("04-Fly-controls-camera", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class FlyControlsCameraExample : Example
    {
        new FlyControls controls;

        public FlyControlsCameraExample() : base() { }
   
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }

        public override void InitCameraController()
        {
            controls = new FlyControls(this, camera);
            controls.MovementSpeed = 25.0f;
            controls.RollSpeed = (float)System.Math.PI / 24;
            controls.AutoForward = true;
            controls.DragToLook = false;
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
            var delta = stopWatch.ElapsedMilliseconds/ 10000.0f;           
            stopWatch.Reset();
            stopWatch.Start();
            controls.Update(delta);

            this.renderer.Render(scene, camera);
        }

    }
}
