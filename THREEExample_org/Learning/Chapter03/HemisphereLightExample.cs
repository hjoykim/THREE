using OpenTK;
using OpenTK.Windowing.Common;
using System;
using THREE;

namespace THREEExample.Learning.Chapter03
{
    [Example("05.Hemisphere-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class HemisphereLightExample : Example
    {
        TrackballControls controls;
        DirectionalLight directionalLight;
        Mesh plane, cube, sphere;
        Texture textureGrass;
        HemisphereLight hemiLight;
        Camera camera;
        Scene scene;

        float step = 0;
        float rotationSpeed = 0.01f;
        float bouncingSpeed = 0.03f;

        public HemisphereLightExample() : base()
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

            textureGrass = TextureLoader.Load("../../../..//assets/textures/ground/grasslight-big.jpg");
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




            var spotLight0 = new SpotLight(new Color().SetHex(0xcccccc));
            spotLight0.Position.Set(-40, 60, -10);
            //spotLight0.LookAt(plane.Position);
            scene.Add(spotLight0);


            hemiLight = new HemisphereLight(new Color().SetHex(0x0000ff), new Color().SetHex(0x00ff00), 0.6f);
            hemiLight.Position.Set(0, 500, 0);
            scene.Add(hemiLight);


            directionalLight = new DirectionalLight(new Color().SetHex(0xffffff));
            directionalLight.Target = plane;
            directionalLight.Position.Set(30, 10, -50);
            directionalLight.CastShadow = true;

            directionalLight.Shadow.Camera.Near = 0.1f;
            directionalLight.Shadow.Camera.Far = 200;
            (directionalLight.Shadow.Camera as OrthographicCamera).Left = -50;
            (directionalLight.Shadow.Camera as OrthographicCamera).CameraRight = 50;
            (directionalLight.Shadow.Camera as OrthographicCamera).Top = 50;
            (directionalLight.Shadow.Camera as OrthographicCamera).Bottom = -50;
            directionalLight.Shadow.MapSize.Set(2048, 2048);

            scene.Add(directionalLight);
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
        public override void Resize(ResizeEventArgs clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
}
