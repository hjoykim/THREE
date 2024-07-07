using OpenTK;
using OpenTK.Windowing.Common;
using THREE;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter03
{
    [Example("01.Ambient-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class AmbientLightExample : Example
    {
        Scene scene;

        Camera camera;

        TrackballControls controls;

        AmbientLight ambientLight;

        SpotLight spotLight;

        public AmbientLightExample() : base()
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
            camera.Aspect = this.renderer.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.X = -30;
            camera.Position.Y = 40;
            camera.Position.Z = 30;
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
        public override void Load(GLControl control)
        {
            base.Load(control);

            InitRenderer();

            InitCamera();

            InitCameraController();

            ambientLight = new AmbientLight(new Color().SetHex(0x606008), 1);
            scene.Add(ambientLight);

            spotLight = new SpotLight(new Color().SetHex(0xffffff), 1, 180, (float)System.Math.PI / 4);
            spotLight.Shadow.MapSize.Set(2048, 2048);
            spotLight.Position.Set(-30, 40, -10);
            spotLight.CastShadow = true;
            scene.Add(spotLight);

            DemoUtils.AddHouseAndTree(scene);
        }
        public override void Render()
        {
            controls.Update();
            this.renderer.Render(scene, camera);
        }

        public override void Resize(ResizeEventArgs clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
}
