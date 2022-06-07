
using OpenTK;
using THREE;
using THREE.Cameras;
using THREE.Cameras.Controlls;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;
namespace THREEExample.Learning.Chapter09
{
    [Example("06-Controls-orbit", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class OrbitControlsExample : Example
    {
        Scene scene;

        Camera camera;

        OrbitControls controls;

        public OrbitControlsExample() : base()
        {
            camera = new THREE.Cameras.PerspectiveCamera();
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
            camera.LookAt(THREE.Math.Vector3.Zero());
        }
        private void InitCameraController()
        {
            controls = new OrbitControls(this.glControl, camera);
            controls.AutoRotate = true;
        }

        public override void Load(GLControl control)
        {
            base.Load(control);

            InitRenderer();

            InitCamera();

            InitCameraController();
            var ambientLight = new AmbientLight(new Color().SetHex(0x222222)) ;
            scene.Add(ambientLight);

            var dirLight = new DirectionalLight(new Color().SetHex(0xffffff));
            dirLight.Position.Set(50, 10, 0);
            scene.Add(dirLight);

            var planetTexture = TextureLoader.Load("../../../assets/textures/mars/mars_1k_color.jpg");
            var normalTexture = TextureLoader.Load("../../../assets/textures/mars/mars_1k_normal.jpg");
            var planetMaterial = new MeshLambertMaterial() { Map = planetTexture, NormalMap = normalTexture };

            scene.Add(new Mesh(new SphereGeometry(20, 40, 40), planetMaterial));
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
