using ImGuiNET;
using THREE;
using static System.Formats.Asn1.AsnWriter;
namespace THREEExample.Chapter02
{
    [Example("07.Both-Cameras", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class BothCameraExample : Example
    {
        Mesh cube;
        string perspective = "Perspective";

        public BothCameraExample() : base() { }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0xEEEEEE), 1);
            this.renderer.ShadowMapEnabled = true;
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.X = 120;
            camera.Position.Y = 60;
            camera.Position.Z = 180;
            camera.LookAt(scene.Position);
        }
        public override void InitLighting()
        {
            base.InitLighting();
            var directionalLight = new DirectionalLight(THREE.Color.Hex(0xffffff), 0.7f);
            directionalLight.Position.Set(-20, 40, 60);
            scene.Add(directionalLight);

            var ambientLight = new AmbientLight(THREE.Color.Hex(0x292929));
            scene.Add(ambientLight);
        }
        public virtual void BuildScene()
        {
            var planeGeometry = new PlaneBufferGeometry(180, 180);
            MeshLambertMaterial planeMaterial = new MeshLambertMaterial() { Color = THREE.Color.Hex(0xffffff) };

            var plane = new Mesh(planeGeometry, planeMaterial);


            plane.Rotation.X = (float)(-0.5 * System.Math.PI);
            plane.Position.X = 0;
            plane.Position.Y = 0;
            plane.Position.Z = 0;

            scene.Add(plane);


            var cubeGeometry = new BoxBufferGeometry(4, 4, 4);

            for (int j = 0; j < (planeGeometry.parameters.Height / 5); j++)
            {
                for (int i = 0; i < (planeGeometry.parameters.Width / 5); i++)
                {
                    var rnd = random.NextDouble() * 0.75f + 0.25f;

                    var cubeMaterial = new MeshLambertMaterial();

                    var c = new THREE.Color((float)rnd, 0, 0);
                    cubeMaterial.Color = c;
                    var cube = new Mesh(cubeGeometry, cubeMaterial);

                    cube.Position.Z = -(planeGeometry.parameters.Height / 2) + 2 + (j * 5);
                    cube.Position.X = -(planeGeometry.parameters.Width / 2) + 2 + (i * 5);
                    cube.Position.Y = 2;
                    scene.Add(cube);
                }
            }
        }
        public override void Init()
        {

            base.Init();


            BuildScene();

            AddGuiControlsAction = () =>
            {
                if (ImGui.Button("switchCamera"))
                {
                    SwitchCamera();
                }

                ImGui.Text("Perspective : " + perspective);
            };
        }
        private void SwitchCamera()
        {

            if (camera is PerspectiveCamera)
            {
                camera = new OrthographicCamera(this.glControl.Width / -16, this.glControl.Width / 16, this.glControl.Height / 16, this.glControl.Height / -16, -200, 500);
                camera.Position.X = 120;
                camera.Position.Y = 60;
                camera.Position.Z = 180;
                camera.LookAt(scene.Position);
                controls.camera = camera;
                this.perspective = "Orthographic";
            }
            else
            {
                camera = new PerspectiveCamera();
                camera.Fov = 45.0f;
                camera.Aspect = this.glControl.AspectRatio;
                camera.Near = 0.1f;
                camera.Far = 1000.0f;
                camera.Position.X = 120;
                camera.Position.Y = 60;
                camera.Position.Z = 180;
                controls.camera = camera;
                camera.LookAt(scene.Position);
                this.perspective = "Perspective";
            }

        }
    }
}
