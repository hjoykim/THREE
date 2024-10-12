using OpenTK.Windowing.Common;
using OpenTK.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using Color = THREE.Color;
namespace SingleFormsDemo
{
    public class THREEAppInstance : ControlsContainer
    {
        public GLRenderer renderer;
        public Camera camera;
        public Scene scene;
        public TrackballControls controls;
        public GLControl glControl;

        public THREEAppInstance()
        {
            renderer = new GLRenderer();
            camera = new PerspectiveCamera();
            scene = new Scene();           
            this.renderer = new THREE.GLRenderer();
        }

        public override THREE.Rectangle GetClientRectangle()
        {
            return new THREE.Rectangle(0, 0, renderer.Width, renderer.Height);
        }
        public void InitCamera()
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
        public void InitRenderer()
        {
            this.renderer.SetClearColor(new THREE.Color().SetHex(0xEEEEEE), 1);
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public void InitCameraController()
        {
            controls = new TrackballControls(this, camera);
            controls.StaticMoving = false;
            controls.RotateSpeed = 4.0f;
            controls.ZoomSpeed = 3;
            controls.PanSpeed = 3;
            controls.NoZoom = false;
            controls.NoPan = false;
            controls.NoRotate = false;
            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.3f;
            controls.Update();
        }
        public void InitLighting()
        {

        }
        public void Load(GLControl control)
        {
            glControl = control;

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            Init();
            
        }
        public void Init()
        {
            InitRenderer();

            InitCamera();

            InitCameraController();

            InitLighting();

            scene.Background = Color.Hex(0x00ffff);

            var axes = new AxesHelper(20);

            scene.Add(axes);

            var planeGeometry = new PlaneGeometry(60, 20);
            var planeMaterial = new MeshBasicMaterial() { Color = Color.Hex(0xcccccc) };
            var plane = new Mesh(planeGeometry, planeMaterial);

            plane.Rotation.X = (float)(-0.5 * Math.PI);
            plane.Position.Set(15, 0, 0);

            scene.Add(plane);

            // create a cube
            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshBasicMaterial() { Color = Color.Hex(0xff0000), Wireframe = true };
            var cube = new Mesh(cubeGeometry, cubeMaterial);

            // position the cube
            cube.Position.Set(-4, 3, 0);

            // add the cube to the scene

            scene.Add(cube);

            //      // create a sphere
            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshBasicMaterial() { Color = Color.Hex(0x7777ff), Wireframe = true };
            var sphere = new Mesh(sphereGeometry, sphereMaterial);

            //      // position the sphere
            sphere.Position.Set(20, 4, 2);

            //      // add the sphere to the scene
            scene.Add(sphere);
        }
        public  void Render()
        {
            controls?.Update();
            renderer?.Render(scene, camera);
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            if (renderer != null)
            {
                renderer.Resize(clientSize.Width, clientSize.Height);
                camera.Aspect = this.glControl.AspectRatio;
                camera.UpdateProjectionMatrix();
            }
            base.OnResize(clientSize);
        }
    }
}
