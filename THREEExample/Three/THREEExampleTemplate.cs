using System;
using THREEExample.ThreeImGui;
using THREE;
using OpenTK;
using ImGuiNET;
namespace THREEExample.Three
{
    public class THREEExampleTemplate : Example
    {
        public Scene scene;

        public Camera camera;

        public TrackballControls controls;

        public ImGuiManager imGuiManager;

        public THREEExampleTemplate() : base()
        {
            scene = new Scene();
            camera = new PerspectiveCamera(); 
        }
        public virtual void InitRenderer()
        {
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;

            renderer.Resize(glControl.Width, glControl.Height);
        }

        public virtual void InitCamera()
        {
            camera.Fov = 45.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 0.1f;
            camera.Far = 1000.0f;
            camera.Position.Set(0, 20, 40);
            camera.LookAt(new THREE.Vector3(10, 0, 0));
        }
        public virtual void InitCameraController()
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
        public virtual void InitLighting()
        {
            var light = new DirectionalLight(Color.Hex(0xffffff), 2);
            light.Position.Set(1, 1, 1);
            scene.Add(light);
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
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
            base.Resize(clientSize);
          
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
