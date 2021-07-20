using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Cameras.Controlls;
using THREE.Loaders;
using THREE.Math;
using THREE.Scenes;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter09
{
    [Example("04-Fly-controls-camera", ExampleCategory.LearnThreeJS, "Chapter09")]
    public class FlyControlsCameraExample : Example
    {
        Scene scene;

        Camera camera;

        FlyControls controls;        

        public FlyControlsCameraExample() : base()
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
            controls = new FlyControls(this.glControl, camera);
            controls.MovementSpeed = 25.0f;
            controls.RollSpeed = (float)System.Math.PI / 24;
            controls.AutoForward = true;
            controls.DragToLook = false;
        }

        public override void Load(GLControl control)
        {
            base.Load(control);

            InitRenderer();

            InitCamera();

            InitCameraController();

            DemoUtils.InitDefaultLighting(scene);

            OBJLoader loader = new OBJLoader();

            var city = loader.Load(@"../../../assets/models/city/city.obj");
            DemoUtils.SetRandomColors(city);
            scene.Add(city);
        }
        public override void Render()
        {
            var delta = stopWatch.ElapsedMilliseconds/ 1000.0f;           
            stopWatch.Reset();
            stopWatch.Start();
            controls.Update(delta);

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
