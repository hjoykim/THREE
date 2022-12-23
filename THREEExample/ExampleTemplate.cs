using System;
using THREE;
using OpenTK;
using ImGuiNET;
using THREEExample.ThreeImGui;

namespace THREEExample
{
    public class ExampleTemplate : Example
    {
        public Camera camera;
        public Scene scene;
        public TrackballControls controls;
        public ImGuiManager imGuiManager;

        public ExampleTemplate() : base()
        {
            camera = new PerspectiveCamera();
            scene = new Scene();
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
        public  virtual void InitCameraController()
        {
            controls = new TrackballControls(this.glControl, camera);
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
        public override void Load(GLControl control)
        {
            base.Load(control);

            Init();

            imGuiManager = new ImGuiManager(this.glControl);
        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;
            controls.Update();
            renderer.Render(scene, camera);
            ShowGUIControls();
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
        public virtual void ShowGUIControls()
        {
            if (AddGuiControlsAction != null)
            {
                ImGui.NewFrame();
                ImGui.Begin("Controls");

                AddGuiControlsAction();

                ImGui.End();
                ImGui.Render();
                imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
                this.renderer.state.currentProgram = -1;
                this.renderer.bindingStates.currentState = this.renderer.bindingStates.defaultState;
            }
        }

        public Action AddGuiControlsAction;
    }
}
