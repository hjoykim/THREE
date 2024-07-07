using OpenTK;
using OpenTK.Windowing.Common;
using THREE;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter03
{
    [Example("02.Spot-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class SpotLightExample : Example
    {
        TrackballControls controls;

        AmbientLight ambientLight;

        SpotLight spotLight, spotLight0;

        Mesh plane, sphereLightMesh, cube, sphere;

        CameraHelper debugCamera;

        SpotLightHelper spotLightHelper;

        Camera camera;

        Scene scene;

        public SpotLightExample() : base()
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

            cube = DemoUtils.AddDefaultCube(scene);
            sphere = DemoUtils.AddDefaultSphere(scene);

            ambientLight = new AmbientLight(new Color().SetHex(0x1c1c1c));
            scene.Add(ambientLight);

            spotLight0 = new SpotLight(new Color().SetHex(0xcccccc));
            spotLight0.Position.Set(-40, 30, -10);

            scene.Add(spotLight0);

            plane = DemoUtils.AddGroundPlane(scene);
            spotLight0.LookAt(plane.Position);



            //var target = new Object3D();
            //target.Position.Set(5, 0, 0);

            spotLight = new SpotLight(new Color().SetHex(0xffffff));
            spotLight.Position.Set(-40, 60, -10);
            spotLight.CastShadow = true;
            spotLight.Shadow.Camera.Near = 1;
            spotLight.Shadow.Camera.Far = 100;
            spotLight.Target = plane;
            spotLight.Distance = 0;
            spotLight.Angle = 0.4f;
            spotLight.Shadow.Camera.Fov = 120;

            scene.Add(spotLight);

            debugCamera = new CameraHelper(spotLight.Shadow.Camera);

            //scene.Add(debugCamera);
            spotLightHelper = new SpotLightHelper(spotLight);
            scene.Add(spotLightHelper);

            var sphereLight = new SphereGeometry(0.2f);
            var sphereLightMaterial = new MeshBasicMaterial() { Color = new Color().SetHex(0xac6c25) };

            sphereLightMesh = new Mesh(sphereLight, sphereLightMaterial);
            sphereLightMesh.CastShadow = true;
            sphereLightMesh.Position.Set(3, 20, 3);
            scene.Add(sphereLightMesh);
        }
        
        float rotationSpeed = 0.03f;
        float bouncingSpeed = 0.03f;
        float step = 0.0f;
        float invert = 1;
        float phase = 0;
        bool stopLightChecked = false;

        public override void Render()
        {
            controls.Update();
            cube.Rotation.X += rotationSpeed;
            cube.Rotation.Y += rotationSpeed;
            cube.Rotation.Z += rotationSpeed;

            step += bouncingSpeed;

            sphere.Position.X = 20 + (10 * (float)(System.Math.Cos(step)));
            sphere.Position.Y = 2 + (10 * (float)System.Math.Abs(System.Math.Sin(step)));

            if (stopLightChecked == false)
            {
                if (phase > 2 * System.Math.PI)
                {
                    invert = invert * -1;
                    phase -= (float)(2 * System.Math.PI);
                }
                else
                {
                    phase += rotationSpeed;
                }
                sphereLightMesh.Position.Z = +(float)(7 * System.Math.Sin(phase));
                sphereLightMesh.Position.X = +(float)(14 * System.Math.Cos(phase));
                sphereLightMesh.Position.Y = 15;

                if (invert < 0)
                {
                    var pivot = 14;

                    sphereLightMesh.Position.X = (invert * (sphereLightMesh.Position.X - pivot)) + pivot;
                }
                spotLight.Position.Copy(sphereLightMesh.Position);
            }

            spotLightHelper.Update();

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
