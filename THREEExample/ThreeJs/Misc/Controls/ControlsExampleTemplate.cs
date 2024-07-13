using ImGuiNET;
using OpenTK;
using OpenTK.Windowing.Common;
using THREE;
using THREEExample.ThreeImGui;
using Color = THREE.Color;
namespace THREEExample.Three.Misc.Controls
{
    public class ControlsExampleTemplate : Example
    {
        public ControlsExampleTemplate() : base() { }
       
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;

            renderer.Resize(glControl.Width, glControl.Height);
        }

        public override void InitCamera()
        {
            //base.InitCamera();
            camera = new PerspectiveCamera(50, glControl.AspectRatio, 0.01f, 30000);
            camera.Position.Set(1000, 500, 1000);
            camera.LookAt(0, 200, 0);
        }

        public override void InitLighting()
        {
            base.InitLighting();
            var light = new DirectionalLight(Color.Hex(0xffffff), 2);
            light.Position.Set(1, 1, 1);
            scene.Add(light);
        }

    }
}
