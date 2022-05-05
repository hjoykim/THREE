using ImGuiNET;
using OpenTK;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter10
{
    [Example("10-displacement-map", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class DisplacementMapExample : TemplateExample
    {       

        MeshStandardMaterial sphereMaterial;

        public DisplacementMapExample() : base()
        {          
        }       

        public override void Load(GLControl control)
        {

            glControl = control;
            this.renderer = new THREE.Renderers.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            InitRenderer();

            InitCamera();

            InitCameraController();

            imguiManager = new ImGuiManager(this.glControl);

            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -10;

            DemoUtils.InitDefaultLighting(scene);

            scene.Add(new AmbientLight(new Color(0x444444)));

            var sphere = new SphereBufferGeometry(8, 180, 180);
            sphereMaterial = new MeshStandardMaterial()
            {
                Map = TextureLoader.Load("../../../assets/textures/w_c.jpg"),
                DisplacementMap = TextureLoader.Load("../../../assets/textures/w_d.png"),
                Metalness=0.02f,
                Roughness=0.07f,
                Color = new THREE.Math.Color(0xffffff)
            };

            groundPlane.ReceiveShadow = true;

            sphereMesh = new Mesh(sphere, sphereMaterial);

            scene.Add(sphereMesh);
        }
        public override void Render()
        {
            controls.Update();
            this.renderer.Render(scene, camera);

            ShowGUIControls();

            sphereMesh.Rotation.Y += 0.01f;
        }
        
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }

       

        public override void ShowGUIControls()
        {
            ImGui.NewFrame();

            ImGui.SliderFloat("displacementScale", ref sphereMaterial.DisplacementScale, -5.0f, 5.0f);
            ImGui.SliderFloat("displacementBias", ref sphereMaterial.DisplacementBias, -5.0f, 5.0f);


            ImGui.Render();

            imguiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
       
        
    }
}
