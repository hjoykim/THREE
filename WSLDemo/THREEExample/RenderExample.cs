using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;

namespace THREEExample
{
    public class RenderExample : IDisposable
    {
        public GLRenderer renderer;
        public Scene scene;
        public Camera camera;
        //public TrackballControls controls;
        protected readonly Stopwatch stopWatch = new Stopwatch();
        IGraphicsContext context;
        public int Width;
        public int Height;
        public RenderExample()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
        }
        public virtual void Load(IGraphicsContext context,int width, int height)
        {
            Debug.Assert(null != context);

            this.context = context;
            this.renderer = new THREE.GLRenderer();
            this.Width = width;
            this.Height = height;
            this.renderer.Context = context;
            this.renderer.Width = width;
            this.renderer.Height = height;
            this.renderer.Init();

            Init();

            CreateScene();
            stopWatch.Start();
        }
        public virtual void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.renderer.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.X = -30;
            camera.Position.Y = 40;
            camera.Position.Z = 30;
            camera.LookAt(THREE.Vector3.Zero());
        }
        public virtual void InitRenderer()
        {
            this.renderer.SetClearColor(new THREE.Color().SetHex(0xEEEEEE), 1);
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        //public virtual void InitCameraController()
        //{
        //    controls = new TrackballControls(this.glControl, camera);
        //    controls.StaticMoving = false;
        //    controls.RotateSpeed = 4.0f;
        //    controls.ZoomSpeed = 3;
        //    controls.PanSpeed = 3;
        //    controls.NoZoom = false;
        //    controls.NoPan = false;
        //    controls.NoRotate = false;
        //    controls.StaticMoving = true;
        //    controls.DynamicDampingFactor = 0.3f;
        //    controls.Update();
        //}
        public virtual void InitLighting()
        {

        }
        public virtual void CreateScene()
        {
            scene.Background = THREE.Color.Hex(0xffffff);

            var axes = new AxesHelper(20);

            scene.Add(axes);

            var planeGeometry = new PlaneGeometry(60, 20);
            var planeMaterial = new MeshBasicMaterial() { Color = THREE.Color.Hex(0xcccccc) };
            var plane = new Mesh(planeGeometry, planeMaterial);

            plane.Rotation.X = (float)(-0.5 * Math.PI);
            plane.Position.Set(15, 0, 0);

            scene.Add(plane);

            // create a cube
            var cubeGeometry = new BoxGeometry(4, 4, 4);
            var cubeMaterial = new MeshBasicMaterial() { Color = THREE.Color.Hex(0xff0000), Wireframe = true };
            var cube = new Mesh(cubeGeometry, cubeMaterial);

            // position the cube
            cube.Position.Set(-4, 3, 0);

            // add the cube to the scene

            scene.Add(cube);

            //      // create a sphere
            var sphereGeometry = new SphereGeometry(4, 20, 20);
            var sphereMaterial = new MeshBasicMaterial() { Color = THREE.Color.Hex(0x7777ff), Wireframe = true };
            var sphere = new Mesh(sphereGeometry, sphereMaterial);

            //      // position the sphere
            sphere.Position.Set(20, 4, 2);

            //      // add the sphere to the scene
            scene.Add(sphere);
        }
        public virtual void Init()
        {
            InitRenderer();

            InitCamera();

            //InitCameraController();

            InitLighting();

            CreateScene();

            Resize += RenderExample_Resize;
        }

        private void RenderExample_Resize(ResizeEventArgs e)
        {
            if (renderer != null)
            {
                renderer.Resize(e.Width, e.Height);
                camera.Aspect = renderer.AspectRatio;
                camera.UpdateProjectionMatrix();
            }
        }

        public virtual void Render()
        {
            renderer.Render(scene, camera);
        }

        public void Dispose()
        {
            renderer.Dispose();
        }
        
        public event Action<MouseButtonEventArgs> MouseUp;
        public event Action<MouseButtonEventArgs> MouseDown;
        public event Action<MouseMoveEventArgs> MouseMove;
        public event Action<MouseWheelEventArgs> MouseWheel;
        public event Action<ResizeEventArgs> Resize;
        public virtual void OnMouseDown(MouseButtonEventArgs args)
        {
            this.MouseDown?.Invoke(args);
        }
        public virtual void OnMouseUp(MouseButtonEventArgs args)
        {
            this.MouseUp?.Invoke(args);
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs args)
        {
            this.MouseWheel?.Invoke(args);
        }

        public virtual void OnMouseMove(MouseMoveEventArgs args)
        {
            this.MouseMove?.Invoke(args);
        }
        public virtual void OnResize(ResizeEventArgs clientSize)
        {
            Resize?.Invoke(clientSize);
        }
    }
}
