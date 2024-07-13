/**
 * This WinForms project  and Example templates were created by referring to Three.cs( (https://github.com/lathoub/three.cs).  
 * */
using ImGuiNET;
using OpenTK;
using OpenTK.Windowing.Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using THREE;
using OpenTK.Windowing.GraphicsLibraryFramework;
using THREEExample.ThreeImGui;
namespace THREEExample
{
    [Serializable]
    abstract public class Example
    {
        public GLRenderer renderer;
        public Camera camera;
        public Scene scene;
        public TrackballControls controls;
        protected readonly Random random = new Random();

        public ImGuiManager imGuiManager { get; set; }

        protected readonly Stopwatch stopWatch = new Stopwatch();

        public IThreeWindow glControl;

        public Action AddGuiControlsAction;

        public Example()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
        }
        ~Example()
        {
            this.Dispose(false);  
        }
        public virtual void Load(IThreeWindow control)
        {
            Debug.Assert(null != control);

            glControl = control;
            this.renderer = new THREE.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            Init();

            stopWatch.Start();
        }
        public virtual void InitCamera()
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
        public virtual void InitRenderer()
        {
            this.renderer.SetClearColor(new THREE.Color().SetHex(0xEEEEEE), 1);
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public virtual void InitCameraController()
        {
            controls = new TrackballControls(camera,new Vector4(0,0,renderer.Width,renderer.Height));
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
        public virtual void InitLighting()
        {

        }
        public virtual void Init()
        {
            InitRenderer();

            InitCamera();

            InitCameraController();

            InitLighting();
        }
        public float GetDelta()
        {
            return stopWatch.ElapsedMilliseconds / 1000.0f;
        }
        public virtual void OnResize(ResizeEventArgs clientSize)
        {
            if (renderer != null)
            {
                renderer.Resize(clientSize.Width, clientSize.Height);
                camera.Aspect = glControl.AspectRatio;
                camera.UpdateProjectionMatrix();
            }
        }

        public virtual void MouseUp(MouseButtonEventArgs clientSize, Point point)
        {

        }

        public virtual void MouseWheel(MouseButtonEventArgs clientSize, Point point, int delta)
        {

        }

        public virtual void MouseMove(MouseButtonEventArgs clientSize, double posX,double posY)
        {

        }

        public virtual void MouseDown(MouseButtonEventArgs clientSize, double posX,double posY)
        {

        }

        public virtual void Render()
        {
            controls?.Update();
            renderer?.Render(scene, camera);
        }
        public virtual void ShowGUIControls()
        {
            if (AddGuiControlsAction != null)
            {
                ImGui.Begin("Controls");

                AddGuiControlsAction();

                ImGui.End();
               
            }
        }
        public virtual void Unload()
        {
            this.renderer.Dispose();
        }

        public virtual void KeyDown(Keys keyCode)
        {
            
        }
        public virtual void KeyUp(Keys keyCode)
        {

        }
        public virtual void Dispose()
        {
            Dispose(disposed);
        }
        public virtual void OnDispose()
        {
            Unload();
        }
        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            try
            {
                OnDispose();
                this.disposed = true;
            }
            finally
            {

            }
            this.disposed = true;
        }
    }
}
