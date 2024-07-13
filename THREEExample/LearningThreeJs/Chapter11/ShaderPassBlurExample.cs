using ImGuiNET;
using OpenTK;
using System.Diagnostics;
using THREE;
using THREEExample.Learning.Utils;
using THREEExample.ThreeImGui;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter11
{
    [Example("08-shader-pass-blur", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class ShaderPassBlurExample : EffectComposerTemplate
    {
        Mesh groundPlane;
        ShaderPass horBlurShader;
        ShaderPass verBlurShader;
        ShaderPass horTiltShiftShader;
        ShaderPass verTiltShiftShader;
        ShaderPass triangleBlurShader;
        ShaderPass focusShader;
        public ShaderPassBlurExample() : base()
        {

        }
        public override void InitLighting()
        {
            base.InitLighting();
            DemoUtils.InitDefaultDirectionalLighting(scene);
        }
        public override void Init()
        {
            base.Init();
            groundPlane = DemoUtils.AddLargeGroundPlane(scene, true);
            groundPlane.Position.Y = 2;
            // add a whole lot of boxes
            var totalWidth = 800;
            var totalDepth = 800;
            var nBoxes = 51;
            var mBoxes = 51;
            int[] colors = new int[] { 0x66ff00, 0x6600ff, 0x0066ff, 0xff6600, 0xff0066 };

            for (var i = 0; i < nBoxes; i++)
            {
                for (var j = 0; j < mBoxes; j++)
                {
                    var box = new BoxBufferGeometry(5, 10, 5);
                    var mat = new MeshStandardMaterial
                    {
                        Color = Color.Hex(colors[MathUtils.random.Next(0, 4)]),
                        Roughness = 0.6f
                    };
                    var mesh = new Mesh(box, mat);
                    mesh.Position.Z = -(totalDepth / 2) + (totalDepth / mBoxes) * j;
                    mesh.Position.X = -(totalWidth / 2) + (totalWidth / nBoxes) * i;
                    // mesh.rotation.y = i;
                    mesh.CastShadow = true;
                    scene.Add(mesh);
                }
            }

            renderPass = new RenderPass(scene, camera);
            renderPass.RenderToScreen = false;
            var effectCopy = new ShaderPass(new CopyShader());
            effectCopy.RenderToScreen = true;
            horBlurShader = new ShaderPass(new HorizontalBlurShader());
            horBlurShader.Enabled = false;
            verBlurShader = new ShaderPass(new VerticalBlurShader());
            verBlurShader.Enabled = false;
            horTiltShiftShader = new ShaderPass(new HorizontalTiltShiftShader());
            horTiltShiftShader.Enabled = false;
            verTiltShiftShader = new ShaderPass(new VerticalTiltShiftShader());
            verTiltShiftShader.Enabled = false;
            //triangleBlurShader = new ShaderPass(new TriangleBlurShader());
            focusShader = new ShaderPass(new FocusShader());
            focusShader.Enabled = false;

            composer = new EffectComposer(renderer);
            composer.AddPass(renderPass);
            composer.AddPass(horBlurShader);
            composer.AddPass(verBlurShader);
            composer.AddPass(horTiltShiftShader);
            composer.AddPass(verTiltShiftShader);
            //composer.AddPass(triangleBlurShader);
            composer.AddPass(focusShader);
            composer.AddPass(effectCopy);

            AddGuiControlsAction = () =>
            {
                AddHorizontalBlur(horBlurShader);
                AddVerticalBlur(verBlurShader);
                AddHorizontalTiltShift(horTiltShiftShader);
                AddVerticalTiltShift(verTiltShiftShader);
                AddFocus(focusShader);
            };
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls.Update();

            composer.Render();

        }

        float h = 1.0f / 512.0f;
        float v = 1.0f / 512.0f;       
        float hh = 1.0f / 512.0f;
        float vv = 1.0f / 512.0f;
        float hr = 0.35f;
        float vr = 0.35f;
        float sampleDistance=0.94f;
        float waveFactor= 0.00125f;
        private void AddHorizontalBlur(ShaderPass pass)
        {
            if (ImGui.TreeNode("horizontalBlur"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("h", ref h, 0.0f, 0.01f))
                {
                    (pass.uniforms["h"] as GLUniform)["value"] = h;
                }
                
                ImGui.TreePop();
            }
        }
        private void AddVerticalBlur(ShaderPass pass)
        {
            if (ImGui.TreeNode("verticalBlur"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("v", ref v, 0.0f, 0.01f))
                {
                    (pass.uniforms["v"] as GLUniform)["value"] = v;
                }

                ImGui.TreePop();
            }
        }

        private void AddHorizontalTiltShift(ShaderPass pass)
        {
            if (ImGui.TreeNode("horizontalTiltShift"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("r", ref hr, 0.0f, 1.0f))
                {
                    (pass.uniforms["r"] as GLUniform)["value"] = hr;
                }
                if (ImGui.SliderFloat("h", ref hh, 0.0f, 0.01f))
                {
                    (pass.uniforms["h"] as GLUniform)["value"] = hh;
                }

                ImGui.TreePop();
            }
        }
        private void AddVerticalTiltShift(ShaderPass pass)
        {
            if (ImGui.TreeNode("vertalTiltShift"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("r", ref vr, 0.0f, 1.0f))
                {
                    (pass.uniforms["r"] as GLUniform)["value"] = vr;
                }
                if (ImGui.SliderFloat("v", ref vv, 0.0f, 0.01f))
                {
                    (pass.uniforms["v"] as GLUniform)["value"] = vv;
                }

                ImGui.TreePop();
            }
        }
        private void AddFocus(ShaderPass pass)
        {
            if (ImGui.TreeNode("focus"))
            {
                ImGui.Checkbox("enabled", ref pass.Enabled);
                if (ImGui.SliderFloat("sampleDistance", ref sampleDistance, 0.0f, 10.0f))
                {
                    (pass.uniforms["sampleDistance"] as GLUniform)["value"] = sampleDistance;
                }
                if (ImGui.SliderFloat("waveFactor", ref waveFactor, 0.0f, 0.005f))
                {
                    (pass.uniforms["waveFactor"] as GLUniform)["value"] = waveFactor;
                }

                ImGui.TreePop();
            }
        }
    }
}
