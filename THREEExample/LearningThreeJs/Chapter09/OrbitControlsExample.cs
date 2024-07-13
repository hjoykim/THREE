
using OpenTK;
using OpenTK.Windowing.Common;
using THREE;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter09
{
    [Example("06-Controls-orbit", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class OrbitControlsExample : Example
    {
        OrbitControls orbitControls;

        public OrbitControlsExample() : base()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
            stopWatch.Start();
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public override void InitCameraController()
        {
            orbitControls = new OrbitControls(this, camera);
            orbitControls.AutoRotate = true;
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var ambientLight = new AmbientLight(new Color().SetHex(0x222222));
            scene.Add(ambientLight);

            var dirLight = new DirectionalLight(new Color().SetHex(0xffffff));
            dirLight.Position.Set(50, 10, 0);
            scene.Add(dirLight);
        }
        public override void Init()
        {
            base.Init();
            var planetTexture = TextureLoader.Load("../../../../assets/textures/mars/mars_1k_color.jpg");
            var normalTexture = TextureLoader.Load("../../../../assets/textures/mars/mars_1k_normal.jpg");
            var planetMaterial = new MeshLambertMaterial() { Map = planetTexture, NormalMap = normalTexture };

            scene.Add(new Mesh(new SphereGeometry(20, 40, 40), planetMaterial));
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                orbitControls.Enabled = true;
            else
                orbitControls.Enabled = false;

            orbitControls.Update();
            renderer.Render(scene, camera);
        }

    }
}
