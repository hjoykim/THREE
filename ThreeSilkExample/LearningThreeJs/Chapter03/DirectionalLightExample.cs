using ImGuiNET;
using System;
using THREE;
using THREE.Silk;
namespace THREE.Silk.Example
{
    [Example("04.Directional-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class DirectionalLightExample : Example
    {

        float step = 0;
        float rotationSpeed = 0.03f;
        float bouncingSpeed = 0.03f;
        AmbientLight ambientLight;
        DirectionalLight directionalLight;
        CameraHelper shadowCamera;
        Mesh sphereLightMesh, plane, cube, sphere;
        System.Numerics.Vector3 ambientColor;
        System.Numerics.Vector3 pointColor;
        bool debug = false;
        int targetIndex = 0;
        public DirectionalLightExample() : base() { }
        public override void InitRenderer()
        {
            base.InitRenderer();
            renderer.SetClearColor(THREE.Color.Hex(0x000000));
        }
        
        public override void Init()
        {
            base.Init();
            CreateObject();

            AddGuiControlsAction = () =>
            {
                ambientColor = new System.Numerics.Vector3(ambientLight.Color.R, ambientLight.Color.G, ambientLight.Color.B);
                pointColor = new System.Numerics.Vector3(directionalLight.Color.R, directionalLight.Color.G, directionalLight.Color.B);
                if (ImGui.TreeNode("Light Colors"))
                {
                    if (ImGui.ColorPicker3("ambientColor", ref ambientColor))
                    {
                        ambientLight.Color = new THREE.Color(ambientColor.X, ambientColor.Y, ambientColor.Z);
                    }
                    if (ImGui.ColorPicker3("pointColor", ref pointColor))
                    {
                        directionalLight.Color = new THREE.Color(pointColor.X, pointColor.Y, pointColor.Z);
                    }
                    ImGui.TreePop();
                }
                ImGui.SliderFloat("intensity", ref directionalLight.Intensity, 0.0f, 5.0f);
                if (ImGui.Checkbox("debug", ref debug))
                {
                    if (debug) scene.Add(shadowCamera);
                    else scene.Remove(shadowCamera);
                }
                ImGui.Checkbox("castShadow", ref directionalLight.CastShadow);
                if (ImGui.Combo("target", ref targetIndex, "Plane\0Sphere\0Cube\0"))
                {
                    switch (targetIndex)
                    {
                        case 0: directionalLight.Target = plane; break;
                        case 1: directionalLight.Target = sphere; break;
                        case 2: directionalLight.Target = cube; break;
                    }
                }
            };
        }
       
        private void CreateObject()
        {
            var planeGeometry = new PlaneBufferGeometry(600, 200, 20, 20);
            var planeMaterial = new MeshLambertMaterial() { Color = THREE.Color.Hex(0xffffff) };
            plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            plane.Rotation.X = (float)(-0.5 * System.Math.PI);
            plane.Position.X = 15;
            plane.Position.Y = -5;
            plane.Position.Z = 0;

            scene.Add(plane);

            var cubeMaterial = new MeshLambertMaterial() { Color = THREE.Color.Hex(0xff3333) };
            var cubeGeometry = new BoxGeometry(4, 4, 4);
            cube = new Mesh(cubeGeometry, cubeMaterial);
            cube.CastShadow = true;
            cube.Position.X = -4;
            cube.Position.Y = 3;
            cube.Position.Z = 0;

            scene.Add(cube);

            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshLambertMaterial() { Color = THREE.Color.Hex(0x7777ff) };

            sphere = new Mesh(sphereGeometry, sphereMaterial);

            sphere.Position.Set(20, 0, 2);
            sphere.CastShadow = true;

            scene.Add(sphere);

            ambientLight = new AmbientLight(THREE.Color.Hex(0x1c1c1c));
            scene.Add(ambientLight);

            directionalLight = new DirectionalLight(THREE.Color.Hex(0xff5808));
            directionalLight.Target = plane;
            directionalLight.Position.Set(-40, 60, -10);
            directionalLight.CastShadow = true;

            directionalLight.Shadow.Camera.Near = 2;
            directionalLight.Shadow.Camera.Far = 80;
            ((OrthographicCamera)directionalLight.Shadow.Camera).Left = -30;
            ((OrthographicCamera)directionalLight.Shadow.Camera).CameraRight = 30;
            ((OrthographicCamera)directionalLight.Shadow.Camera).Top = 30;
            ((OrthographicCamera)directionalLight.Shadow.Camera).Bottom = -30;

            directionalLight.Intensity = 0.5f;
            directionalLight.Shadow.MapSize.Set(1024, 1024);

            scene.Add(directionalLight);

            shadowCamera = new CameraHelper(directionalLight.Shadow.Camera);

            var sphereLight = new SphereGeometry(0.2f);
            var sphereLightMaterial = new MeshBasicMaterial() { Color = THREE.Color.Hex(0xac6c25) };

            sphereLightMesh = new Mesh(sphereLight, sphereLightMaterial);

            sphereLightMesh.Position.Set(3, 20, 3);

            scene.Add(sphereLightMesh);

        }
         
        public override void Render()
        {
            cube.Rotation.X += rotationSpeed;
            cube.Rotation.Y += rotationSpeed;
            cube.Rotation.Z += rotationSpeed;

            step += bouncingSpeed;

            sphere.Position.X = (float)(20 + (10 * (Math.Cos(step))));
            sphere.Position.Y = (float)(2 + (10 * Math.Abs(Math.Sin(step))));


            sphereLightMesh.Position.X = 10 + (float)(26 * System.Math.Cos(step / 3));
            sphereLightMesh.Position.Y = (float)(27 * System.Math.Cos(step / 3));
            sphereLightMesh.Position.Z = -8;

            directionalLight.Position.Copy(sphereLightMesh.Position);
            shadowCamera.Update();

            base.Render();
        }

    }
}
