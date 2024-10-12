using ImGuiNET;

using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;

namespace THREE.Silk.Example
{
    [Example("01.Ambient-Light", ExampleCategory.LearnThreeJS, "Chapter03")]
    public class AmbientLightExample : Example
    {


        AmbientLight ambientLight;
        SpotLight spotLight;

        public AmbientLightExample() : base()
        {
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0x000000));
        }

        public override void InitLighting()
        {
            base.InitLighting();
            ambientLight = new AmbientLight(THREE.Color.Hex(0x606008), 1);
            scene.Add(ambientLight);

            spotLight = new SpotLight(THREE.Color.Hex(0xffffff), 1, 180, (float)System.Math.PI / 4);
            spotLight.Shadow.MapSize.Set(2048, 2048);
            spotLight.Position.Set(-30, 40, -10);
            spotLight.CastShadow = true;
            scene.Add(spotLight);
        }
        public override void Init()
        {
            base.Init();
            DemoUtils.AddHouseAndTree(scene);

            AddGuiControlsAction = () =>
            {
                ImGui.SliderFloat("intensity", ref ambientLight.Intensity, 0, 3);
                System.Numerics.Vector3 color = new System.Numerics.Vector3(ambientLight.Color.R, ambientLight.Color.G, ambientLight.Color.B);
                if (ImGui.TreeNode("Light Colors"))
                {
                    if (ImGui.ColorPicker3("ambientColor", ref color))
                    {
                        ambientLight.Color = new THREE.Color(color.X, color.Y, color.Z);
                    }
                }
                ImGui.Checkbox("disableSpotlight", ref spotLight.Visible);
            };
        }
    }
}
