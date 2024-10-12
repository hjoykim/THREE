using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Silk;

namespace THREE.Silk.Example
{
    [Example("05-Control-GUI", ExampleCategory.LearnThreeJS, "Chapter01")]
    public class ControlGUIExample : MaterialsLightAnimationExample
    {
        public ControlGUIExample() : base() { }

        public override void Init()
        {
            base.Init();

            AddGuiControlsAction = () =>
            {
                ImGui.SliderFloat("RotationSpeed", ref rotationSpeed, 0.0f, 0.5f);
                ImGui.SliderFloat("BouncingSpeed", ref bouncingSpeed, 0.0f, 0.5f);
            };
        }

    }
}
