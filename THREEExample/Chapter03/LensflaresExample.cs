using ImGuiNET;
using OpenTK;
using OpenTK.Windowing.Common;
using System;
using THREE;


namespace THREEExample.Learning.Chapter03
{
    [Example("07.Lensflares-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class LensflaresExample : Example
    {
        AmbientLight ambientLight;
        DirectionalLight spotLight;
        CameraHelper shadowCamera;
        Mesh sphereLightMesh, plane, cube, sphere;
        Texture textureGrass;
        HemisphereLight hemiLight;

        float step = 0;
        float invert = 1;
        float phase = 0;
        float rotationSpeed = 0.03f;
        float bouncingSpeed = 0.03f;
        float intensity = 1;
        float distance;

        public LensflaresExample() : base()
        {
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0x000000));
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.X = -20;
            camera.Position.Y = 10;
            camera.Position.Z = 45;
        }
        public override void InitLighting()
        {
            base.InitLighting();
            ambientLight = new AmbientLight(THREE.Color.Hex(0x1c1c1c));
            scene.Add(ambientLight);


            var spotLight0 = new SpotLight(THREE.Color.Hex(0xcccccc));
            spotLight0.Position.Set(-40, 60, -10);
            //spotLight0.LookAt(plane.Position);
            scene.Add(spotLight0);




            spotLight = new DirectionalLight(THREE.Color.Hex(0xffffff));

            spotLight.Position.Set(30, 10, -50);
            spotLight.CastShadow = true;
            spotLight.Shadow.Camera.Near = 2;
            spotLight.Shadow.Camera.Far = 200;
            spotLight.Shadow.Camera.Fov = 50;

            spotLight.Distance = 0;
            ((OrthographicCamera)spotLight.Shadow.Camera).Left = -100;
            ((OrthographicCamera)spotLight.Shadow.Camera).CameraRight = 100;
            ((OrthographicCamera)spotLight.Shadow.Camera).Top = 100;
            ((OrthographicCamera)spotLight.Shadow.Camera).Bottom = -100;
            spotLight.Shadow.MapSize.Set(2048, 2048);
            scene.Add(spotLight);
        }
        public override void Init()
        {
            base.Init();
            textureGrass = TextureLoader.Load("../../../../assets/textures/ground/grasslight-big.jpg");
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
            spotLight.Target = plane;
            scene.Add(plane);

            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshLambertMaterial() { Color = THREE.Color.Hex(0xff3333) };

            cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;
            cube.Position.Set(-4, 3, 0);

            scene.Add(cube);

            var sphereGeometry = new SphereGeometry(4, 25, 25);
            var sphereMaterial = new MeshPhongMaterial() { Color = THREE.Color.Hex(0x7777ff) };

            sphere = new Mesh(sphereGeometry, sphereMaterial);

            sphere.Position.Set(10, 5, 10);
            sphere.CastShadow = true;

            scene.Add(sphere);            

            var textureFlare0 = TextureLoader.Load("../../../../assets/textures/flares/lensflare0.png");
            var textureFlare3 = TextureLoader.Load("../../../../assets/textures/flares/lensflare3.png");

            var flareColor = THREE.Color.Hex(0xffaacc);

            Lensflare lensFlare = new Lensflare();

            lensFlare.AddElement(new LensflareElement(textureFlare0, 350, 0.0f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 60, 0.6f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 70, 0.7f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 120, 0.9f, flareColor));
            lensFlare.AddElement(new LensflareElement(textureFlare3, 70, 1.0f, flareColor));

            spotLight.Add(lensFlare);

            AddGuiControlsAction = () =>
            {
                var ambientColor = new System.Numerics.Vector3(ambientLight.Color.R, ambientLight.Color.G, ambientLight.Color.B);
                var pointColor = new System.Numerics.Vector3(spotLight.Color.R, spotLight.Color.G, spotLight.Color.B);
                if (ImGui.TreeNode("Light Colors"))
                {
                    if (ImGui.ColorPicker3("ambientColor", ref ambientColor))
                    {
                        ambientLight.Color = new THREE.Color(ambientColor.X, ambientColor.Y, ambientColor.Z);
                    }
                    if (ImGui.ColorPicker3("pointColor", ref pointColor))
                    {
                        spotLight.Color = new THREE.Color(pointColor.X, pointColor.Y, pointColor.Z);
                    }
                    ImGui.TreePop();
                }
                ImGui.SliderFloat("intensity", ref spotLight.Intensity, 0, 5);
            };
        }

        public override void Render()
        {

            cube.Rotation.X += rotationSpeed;
            cube.Rotation.Y += rotationSpeed;
            cube.Rotation.Z += rotationSpeed;

            step += bouncingSpeed;

            sphere.Position.X = (float)(20 + (10 * (Math.Cos(step))));
            sphere.Position.Y = (float)(2 + (10 * Math.Abs(Math.Sin(step))));

            base.Render();
        } 
    }
}
