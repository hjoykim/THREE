using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using OpenTK.Windowing.GraphicsLibraryFramework;
using THREEExample.ThreeImGui;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using System.Diagnostics;
using OpenTK.Windowing.Common;
using Rectangle = THREE.Rectangle;
namespace THREEExample
{
    [Serializable]
    abstract public class Example : ControlsContainer
    {
        public GLRenderer renderer;
        public Camera camera;
        public Scene scene;
        public TrackballControls controls;
        protected readonly Random random = new Random();

        public ImGuiManager imGuiManager { get; set; }

        protected readonly Stopwatch stopWatch = new Stopwatch();
#if WSL
        public IThreeWindow glControl;
#else
        public GLControl glControl;
#endif

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
#if WSL
        public virtual void Load(IThreeWindow control)
#else
        public virtual void Load(GLControl control)
#endif
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
        public override Rectangle GetClientRectangle()
        {
            return new Rectangle(0,0,renderer.Width,renderer.Height);
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
                camera.Aspect = this.glControl.AspectRatio;
                camera.UpdateProjectionMatrix();
            }
            base.OnResize(clientSize);
        }

        public virtual void Render()
        {
            if (!imGuiManager.ImWantMouse)
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

