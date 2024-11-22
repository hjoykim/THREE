using System;
using THREE;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.Loader
{
    [Example("loader_ply", ExampleCategory.ThreeJs, "loader")]
    public class LoaderPlyExample : Example
    {
        Vector3 cameraTarget;
        public LoaderPlyExample() : base () { }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 35;
            camera.Near = 1;
            camera.Far = 15;
            camera.Position.Set(3, 0.15f, 3);
            cameraTarget = new Vector3(0,0.1f,0);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.outputEncoding = Constants.sRGBEncoding;
            renderer.ShadowMap.Enabled = true;
        }
        private void AddShadowedLight(float x, float y, float z, Color color, float intensity)
        {

            var directionalLight = new DirectionalLight(color, intensity);
            directionalLight.Position.Set(x, y, z);
            scene.Add(directionalLight);

            directionalLight.CastShadow = true;

            var d = 1;
            directionalLight.Shadow.Camera.Left = -d;
            directionalLight.Shadow.Camera.CameraRight = d;
            directionalLight.Shadow.Camera.Top = d;
            directionalLight.Shadow.Camera.Bottom = -d;

            directionalLight.Shadow.Camera.Near = 1;
            directionalLight.Shadow.Camera.Far = 4;

            directionalLight.Shadow.MapSize.Width = 1024;
            directionalLight.Shadow.MapSize.Height = 1024;

            directionalLight.Shadow.Bias = -0.001f;

        }
        public override void InitLighting()
        {
            base.InitLighting();
            scene.Add(new HemisphereLight(Color.Hex(0x443333), Color.Hex(0x111122)));

            AddShadowedLight(1, 1, 1, Color.Hex(0xffffff), 1.35f);
            AddShadowedLight(0.5f, 1, -1, Color.Hex(0xffaa00), 1);
        }
        public virtual void BuildScene()
        {
            scene.Background = Color.Hex(0x72645b);
            scene.Fog = new Fog(0x72645b, 2, 15);

            var plane = new Mesh(
                    new PlaneGeometry(40, 40),
                    new MeshPhongMaterial() { Color = Color.Hex(0x999999), Specular = Color.Hex(0x101010) }
				);
			plane.Rotation.X = (float)- Math.PI / 2;
			plane.Position.Y = - 0.5f;
			scene.Add(plane );

			plane.ReceiveShadow = true;

            var loader = new AssimpLoader();
            var mesh = loader.Load("../../../../assets/models/ply/ascii/dolphins.ply");
            var material = new MeshStandardMaterial() { Color = Color.Hex(0x0055ff), FlatShading = true } ;
            mesh.Traverse((m) =>
            {
                if (m is Mesh)
                {
                    m.Material = material;
                    m.CastShadow = true;
                    m.ReceiveShadow = true;
                }
            });

            mesh.Position.Y = - 0.2f;
			mesh.Position.Z= 0.3f;
			mesh.Rotation.X = (float)- Math.PI / 2;
			mesh.Scale.MultiplyScalar( 0.001f);

			
            scene.Add(mesh );

            var mesh1 = loader.Load("../../../../assets/models/ply/binary/Lucy100k.ply");
            mesh1.Traverse((m) =>
            {
                if (m is Mesh)
                {
                    m.Material = material;
                    m.CastShadow = true;
                    m.ReceiveShadow = true;
                }
            });

            mesh1.Position.X = -0.2f;
            mesh1.Position.Y = -0.02f;
            mesh1.Position.Z = -0.2f;
            mesh1.Scale.MultiplyScalar(0.0006f);

           

            scene.Add(mesh1);        
        }
        public override void Init()
        {
            base.Init();
            BuildScene();
        }
        public override void Render()
        {
            var timer = this.GetDelta();
            camera.Position.X = (float)Math.Sin(timer) * 2.5f;
            camera.Position.Z = (float)Math.Cos(timer) * 2.5f;
            camera.LookAt(cameraTarget);
            base.Render();
        }
    }
}
