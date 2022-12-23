using OpenTK;
using System;
using THREE;


namespace THREEExample.Learning.Chapter03
{
    [Example("07.Lensflares-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class LensflaresExample : Example
    {
        TrackballControls controls;
        AmbientLight ambientLight;
        DirectionalLight spotLight;
        CameraHelper shadowCamera;
        Mesh sphereLightMesh, plane, cube, sphere;
        Texture textureGrass;
        HemisphereLight hemiLight;
        Camera camera;
        Scene scene;

        float step = 0;
        float invert = 1;
        float phase = 0;
        float rotationSpeed = 0.03f;
        float bouncingSpeed = 0.03f;
        float intensity = 1;
        float distance;

        public LensflaresExample() : base()
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
            camera.Position.X = -20;
            camera.Position.Y = 10;
            camera.Position.Z = 45;
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

            textureGrass = TextureLoader.Load("../../../assets/textures/ground/grasslight-big.jpg");
            textureGrass.WrapS = Constants.RepeatWrapping;
            textureGrass.WrapT = Constants.RepeatWrapping;
            textureGrass.Repeat.Set(10, 10);


            var planeGeometry = new PlaneGeometry(1000, 1000, 20, 20);
            var planeMaterial = new MeshLambertMaterial() { Map = textureGrass };
            plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            plane.Rotation.X = (float)(-0.5 * System.Math.PI);
            plane.Position.X = 15;
            plane.Position.Y = 0;
            plane.Position.Z = 0;

            scene.Add(plane);


            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshLambertMaterial() { Color = new Color().SetHex(0xff3333) };

            cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;
            cube.Position.Set(-4, 3, 0);

            scene.Add(cube);

            var sphereGeometry = new SphereGeometry(4, 25, 25);
            var sphereMaterial = new MeshPhongMaterial() { Color = new Color().SetHex(0x7777ff) };

            sphere = new Mesh(sphereGeometry, sphereMaterial);

            sphere.Position.Set(10, 5, 10);
            sphere.CastShadow = true;

            scene.Add(sphere);

            ambientLight = new AmbientLight(Color.Hex(0x1c1c1c));
            scene.Add(ambientLight);


            var spotLight0 = new SpotLight(new Color().SetHex(0xcccccc));
            spotLight0.Position.Set(-40, 60, -10);
            //spotLight0.LookAt(plane.Position);
            scene.Add(spotLight0);




            spotLight = new DirectionalLight(new Color().SetHex(0xffffff));

            spotLight.Position.Set(30, 10, -50);
            spotLight.CastShadow = true;
            spotLight.Shadow.Camera.Near = 2;
            spotLight.Shadow.Camera.Far = 200;
            spotLight.Shadow.Camera.Fov = 50;
            spotLight.Target = plane;
            spotLight.Distance = 0;
            (spotLight.Shadow.Camera as OrthographicCamera).Left = -100;
            (spotLight.Shadow.Camera as OrthographicCamera).CameraRight = 100;
            (spotLight.Shadow.Camera as OrthographicCamera).Top = 100;
            (spotLight.Shadow.Camera as OrthographicCamera).Bottom = -100;
            spotLight.Shadow.MapSize.Set(2048, 2048);

            var textureFlare0 = TextureLoader.Load("../../../assets/textures/flares/lensflare0.png");
            var textureFlare3 = TextureLoader.Load("../../../assets/textures/flares/lensflare3.png");


            var flareColor = Color.Hex(0xffaacc);

            Lensflare lensFlare = new Lensflare();

            lensFlare.AddElement(new LensflareElement(textureFlare0, 350, 0.0f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 60, 0.6f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 70, 0.7f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 120, 0.9f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 70, 1.0f, flareColor));

            spotLight.Add(lensFlare);

            scene.Add(spotLight);
        }
        public override void Render()
        {
            controls.Update();

            cube.Rotation.X += rotationSpeed;
            cube.Rotation.Y += rotationSpeed;
            cube.Rotation.Z += rotationSpeed;

            step += bouncingSpeed;

            sphere.Position.X = (float)(20 + (10 * (Math.Cos(step))));
            sphere.Position.Y = (float)(2 + (10 * Math.Abs(Math.Sin(step))));

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
