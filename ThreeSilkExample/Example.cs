using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using System.Diagnostics;
using Rectangle = THREE.Rectangle;
using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.ImGui;
using Silk.NET.Windowing;
using THREE.Silk;
namespace THREE.Silk.Example
{
    [Serializable]
    abstract public class Example : ControlsContainer
    {
        public GLRenderer renderer;
        public Camera camera;
        public Scene scene;
        public TrackballControls controls;
        protected readonly Random random = new Random();

        public ImGuiController imGuiManager { get; set; }

        protected readonly Stopwatch stopWatch = new Stopwatch();
        public bool ImWantMouse => ImGui.GetIO().WantCaptureMouse;
        public bool ImWantKeyboard =>ImGui.GetIO().WantCaptureKeyboard;
        public IWindow glControl;
        public int Width => glControl.Size.X;
        public int Height => glControl.Size.Y;
        public float AspectRatio => (float)glControl.Size.X / (float)glControl.Size.Y;



        public Action AddGuiControlsAction;

        public Example()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
            renderer = new GLRenderer();
        }
        ~Example()
        {
            this.Dispose(false);
        }

        public virtual void Load(IWindow control)
        {
            Debug.Assert(null != control);
            glControl = control;

            this.renderer.Width = control.Size.X;
            this.renderer.Height = control.Size.Y;
            this.renderer.Context = control;
            this.renderer.gl = GL.GetApi(control);
            this.renderer.Init();

            Init();

            stopWatch.Start();
        }
        public override Rectangle GetClientRectangle()
        {
            return new Rectangle(0,0,renderer.Width,renderer.Height);
        }
        public virtual void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.AspectRatio;
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
                camera.Aspect = this.AspectRatio;
                camera.UpdateProjectionMatrix();
            }
            base.OnResize(clientSize);
        }

        public virtual void Render()
        {
            if (!this.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

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

        public override void Dispose()
        {
            OnDispose();
        }
        public virtual void OnDispose()
        {
            Unload();
        }

    }
}

