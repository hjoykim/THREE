using System;
using THREE;
using Color = THREE.Color;

namespace THREEExample.Three.Loader
{
    [Example("loader_stl",ExampleCategory.ThreeJs,"loader")]
    public class STLLoaderSample : Example
    {
        Vector3 cameraTarget;

        public STLLoaderSample() : base()
        {
            scene.Background = Color.Hex(0x72645b);
            scene.Fog = new Fog(0x72645b, 2, 15);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.outputEncoding = Constants.sRGBEncoding;
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 35;
            camera.Near = 1;
            camera.Far = 15;
            camera.Position.Set(3, 0.15f, 3);

            cameraTarget = new Vector3(0, -0.25f, 0);
        }

        public override void InitLighting()
        {
            scene.Add(new HemisphereLight(Color.Hex(0x443333), Color.Hex(0x111122)));

            AddShadowedLight(1, 1, 1, 0xffffff, 1.35f);
            AddShadowedLight(0.5f, 1, -1, 0xffaa00, 1);

        }
        public override void Init()
        {
            base.Init();

            CreateObject();
        }

        private void AddShadowedLight(float x,float y,float z,int color,float intensity)
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

            directionalLight.Shadow.Bias = -0.002f;
        }
        private void CreateObject()
        {
			var plane = new Mesh( new  PlaneBufferGeometry(40, 40), new MeshPhongMaterial() { Color= Color.Hex(0x999999), Specular= Color.Hex(0x101010) });
				plane.Rotation.X = - (float)Math.PI / 2;
				plane.Position.Y = - 0.5f;
				scene.Add(plane );

				plane.ReceiveShadow = true;


            // ASCII file
            var geometry1 = STLLoader.Load("../../../../assets/models/stl/ascii/slotted_disk.stl");
            var material1 = new MeshPhongMaterial() { Color = Color.Hex(0xff5533), Specular = Color.Hex(0x111111), Shininess = 200 };
            var mesh = new Mesh(geometry1, material1);

            mesh.Position.Set(0, -0.25f, 0.6f);
            mesh.Rotation.Set(0, -(float)Math.PI / 2, 0);
            mesh.Scale.Set(0.5f, 0.5f, 0.5f);

            mesh.CastShadow = true;
            mesh.ReceiveShadow = true;

            scene.Add(mesh);



            // Binary files


            var geometry2 = STLLoader.Load("../../../../assets/models/stl/binary/pr2_head_pan.stl");
            var material2 = new MeshPhongMaterial() { Color = Color.Hex(0xAAAAAA), Specular = Color.Hex(0x111111), Shininess = 200 };

            var mesh2 = new Mesh(geometry2, material2);

	        mesh2.Position.Set(0, -0.37f, -0.6f);
	        mesh2.Rotation.Set((float)-Math.PI / 2, 0, 0);
	        mesh2.Scale.Set(2, 2, 2);

	        mesh2.CastShadow = true;
	        mesh2.ReceiveShadow = true;

	        scene.Add(mesh2);


            var geometry3 = STLLoader.Load("../../../../assets/models/stl/binary/pr2_head_tilt.stl");

	        var mesh3 = new Mesh(geometry3, material2);

        	mesh3.Position.Set(0.136f, -0.37f, -0.6f);
	        mesh3.Rotation.Set((float)-Math.PI / 2, 0.3f, 0);
	        mesh3.Scale.Set(2, 2, 2);

	        mesh3.CastShadow = true;
	        mesh3.ReceiveShadow = true;

	        scene.Add(mesh3);


            // Colored binary STL
            var geometry4 = STLLoader.Load("../../../../assets/models/stl/binary/colored.stl");

	        var meshMaterial = material2;
	        if (geometry4.UserData.ContainsKey("hasColors") && (bool)geometry4.UserData["hasColors"])
	        {

		        meshMaterial = new MeshPhongMaterial() { Opacity = (float)geometry4.UserData["alpha"], VertexColors= true };

	        }

	        var mesh4 = new Mesh(geometry4, meshMaterial);

	        mesh4.Position.Set(0.5f, 0.2f, 0);
	        mesh4.Rotation.Set((float)-Math.PI / 2, (float)Math.PI / 2, 0);
	        mesh4.Scale.Set(0.3f, 0.3f, 0.3f);

        	mesh4.CastShadow = true;
	        mesh4.ReceiveShadow = true;

	        scene.Add(mesh4);

        }
        public override void Render()
        {
            var timer = stopWatch.ElapsedMilliseconds * 0.0005f;
            camera.Position.X = (float)Math.Cos(timer) * 3;
            camera.Position.Z = (float)Math.Sin(timer) * 3;

            camera.LookAt(cameraTarget);
            base.Render();
        }
    }
}
