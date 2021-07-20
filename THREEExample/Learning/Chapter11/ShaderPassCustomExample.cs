using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Postprocessing;
using THREE.Renderers.gl;
using THREE.Shaders;
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
            this.renderer = new THREE.Renderers.GLRenderer();

            this.renderer.glControl = control;

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
                    (pass.uniforms["rPower"] as GLUniform)["value"] = rPower;
                }
                if (ImGui.SliderFloat("gPower", ref gPower, 0.0f, 1.0f))
                {
                    (pass.uniforms["gPower"] as GLUniform)["value"] = gPower;
                }
                if (ImGui.SliderFloat("rPower", ref bPower, 0.0f, 1.0f))
                {
                    (pass.uniforms["bPower"] as GLUniform)["value"] = bPower;
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
                    (pass.uniforms["bitSize"] as GLUniform)["value"] = bitSize;
                }

                ImGui.TreePop();
            }
        }
    }
}
