using ImGuiNET;
using OpenTK;
using System.Diagnostics;
using THREE;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter11
{
    [Example("09-shader-pass-custom", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class ShaderPassCustomExample : EffectComposerTemplate
    {
        ShaderPass customGrayScale;
        ShaderPass customBit;
        public ShaderPassCustomExample():base()
        {
        }

        public override void Load(GLControl control)
        {
            Debug.Assert(null != control);

            glControl = control;
            this.renderer = new THREE.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            stopWatch.Start();

            InitRenderer();

            InitCamera();

            InitCameraController();

            imGuiManager = new ImGuiManager(this.glControl);

            (earth, pivot) = Util11.AddEarth(scene);

            renderPass = new RenderPass(scene, camera);
            customGrayScale = new ShaderPass(new CustomGrayScaleShader());
            customGrayScale.Enabled = false;
            customBit = new ShaderPass(new CustomBitShader());
            customBit.Enabled = false;
            var effectCopy = new ShaderPass(new CopyShader());
            effectCopy.RenderToScreen = true;


            composer = new EffectComposer(renderer);
            composer.AddPass(renderPass);
            composer.AddPass(customGrayScale);
            composer.AddPass(customBit);
            composer.AddPass(effectCopy);

        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls.Update();

            composer.Render();

            ShowControls();
        }
        public override void ShowControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");

            AddCustomGray(customGrayScale);
            AddCustomBit(customBit);
            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
        float rPower = 0.2126f;
        float gPower =0.7152f;
        float bPower = 0.0722f;
        int bitSize = 4;
        private void AddCustomGray(ShaderPass pass)
        {
            if (ImGui.TreeNode("CustomGray"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("rPower", ref rPower, 0.0f, 1.0f))
                {
                    (pass.uniforms["rPower"] as Uniform)["value"] = rPower;
                }
                if (ImGui.SliderFloat("gPower", ref gPower, 0.0f, 1.0f))
                {
                    (pass.uniforms["gPower"] as Uniform)["value"] = gPower;
                }
                if (ImGui.SliderFloat("rPower", ref bPower, 0.0f, 1.0f))
                {
                    (pass.uniforms["bPower"] as Uniform)["value"] = bPower;
                }


                ImGui.TreePop();
            }
        }
        private void AddCustomBit(ShaderPass pass)
        {
            if (ImGui.TreeNode("CustomBit"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderInt("bitSize", ref bitSize, 1, 10))
                {
                    (pass.uniforms["bitSize"] as Uniform)["value"] = bitSize;
                }

                ImGui.TreePop();
            }
        }
    }
}
