using ImGuiNET;
using OpenTK;
using OpenTK.Windowing.Common;
using THREE;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter10
{
    [Example("10-displacement-map", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class DisplacementMapExample : TemplateExample
    {       

        MeshStandardMaterial sphereMaterial;

        public DisplacementMapExample() : base()
        {          
        }
        public override void SetGeometryWithTexture()
        {
            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -10;


            scene.Add(new AmbientLight(new Color(0x444444)));

            var sphere = new SphereBufferGeometry(8, 180, 180);
            sphereMaterial = new MeshStandardMaterial()
            {
                Map = TextureLoader.Load("../../../../assets/textures/w_c.jpg"),
                DisplacementMap = TextureLoader.Load("../../../../assets/textures/w_d.png"),
                Metalness = 0.02f,
                Roughness = 0.07f,
                Color = new THREE.Color(0xffffff)
            };

            groundPlane.ReceiveShadow = true;

            sphereMesh = new Mesh(sphere, sphereMaterial);

            scene.Add(sphereMesh);
        }

        public override void Init()
        {
            base.Init();

            AddGuiControlsAction = () =>
            {
                ImGui.SliderFloat("displacementScale", ref sphereMaterial.DisplacementScale, -5.0f, 5.0f);
                ImGui.SliderFloat("displacementBias", ref sphereMaterial.DisplacementBias, -5.0f, 5.0f);
            };
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            this.renderer.Render(scene, camera);

            ShowGUIControls();

            sphereMesh.Rotation.Y += 0.01f;
        }
    }
}
