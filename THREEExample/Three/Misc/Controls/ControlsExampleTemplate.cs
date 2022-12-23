using ImGuiNET;
using OpenTK;
using THREE;
using THREEExample.ThreeImGui;

namespace THREEExample.Three.Misc.Controls
{
    public class ControlsExampleTemplate : Example
    {
        public Scene scene;

        public Camera camera;

        public TrackballControls controls;

        public ImGuiManager imGuiManager;

        public ControlsExampleTemplate() : base() 
        {
            scene = new Scene();
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
            camera = new PerspectiveCamera(50, glControl.AspectRatio, 0.01f, 30000); ;
            camera.Position.Set(1000, 500, 1000);
            camera.LookAt(0, 200, 0);
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
            ImGui.NewFrame();
            ImGui.Begin("Controls");

            AddGuiControls();

            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
        public virtual void AddGuiControls()
        {

        }
    }
}
