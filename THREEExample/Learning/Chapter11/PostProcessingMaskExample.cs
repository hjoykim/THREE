using ImGuiNET;
using OpenTK;
using System.Diagnostics;
using THREE;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter11
{
    [Example("04-Post-Processing", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class PostProcessingMaskExample : EffectComposerTemplate
    {
        public Scene earthLight;
        public Scene marsLight;
        public Mesh mars;

        private ShaderPass effectSepia;
        private ShaderPass effectColorify;
        public PostProcessingMaskExample() : base()
        {

        }
        public override void Load(GLControl control)
        {
            //base.Load(control);
            Debug.Assert(null != control);

            glControl = control;
            this.renderer = new THREE.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            stopWatch.Start();

            InitRenderer();

            this.renderer.SetSize(control.Width, control.Height);

            InitCamera();

            InitCameraController();

            imGuiManager = new ImGuiManager(this.glControl);

            renderer.AutoClear = false;

            // create the scenes
            var sceneEarth = new Scene();
            var sceneMars = new Scene();
            var sceneBG = new Scene();

            // create all the scenes we'll be rendering.
            sceneBG.Background = TextureLoader.Load("../../../assets/textures/bg/starry-deep-outer-space-galaxy.jpg");
            (earth,earthLight) = Util11.AddEarth(sceneEarth);
            sceneEarth.TranslateX(-16);
            sceneEarth.Scale.Set(1.2f, 1.2f, 1.2f);
            (mars,marsLight) = Util11.AddMars(sceneMars);
            sceneMars.TranslateX(12);
            sceneMars.TranslateY(6);
            sceneMars.Scale.Set(0.2f, 0.2f, 0.2f);

            // setup passes. First the main renderpasses. Note that
            // only the bgRenderpass clears the screen.
            var bgRenderPass = new RenderPass(sceneBG, camera);
            var earthRenderPass = new RenderPass(sceneEarth, camera);
            earthRenderPass.Clear = false;
            var marsRenderPass = new RenderPass(sceneMars, camera);
            marsRenderPass.Clear = false;

            // setup the mask
            var clearMask = new ClearMaskPass();
            var earthMask = new MaskPass(sceneEarth, camera);
            var marsMask = new MaskPass(sceneMars, camera);

            // setup some effects to apply
            effectSepia = new ShaderPass(new SepiaShader());
            effectSepia.uniforms["amount"] = new GLUniform { { "value", 0.8f } };
            effectColorify = new ShaderPass(new ColorifyShader());
            effectColorify.uniforms["color"] = new GLUniform { { "value", new Color(0.5f, 0.5f, 1f) } };

            var effectCopy = new ShaderPass(new CopyShader());
            effectCopy.RenderToScreen = true;

            composer = new EffectComposer(renderer);
            composer.RenderTarget1.stencilBuffer = true;
            composer.RenderTarget2.stencilBuffer = true;
            composer.AddPass(bgRenderPass);
            composer.AddPass(earthRenderPass);
            composer.AddPass(marsRenderPass);
            composer.AddPass(marsMask);
            composer.AddPass(effectColorify);
            composer.AddPass(clearMask);
            composer.AddPass(earthMask);
            composer.AddPass(effectSepia);
            composer.AddPass(clearMask);
            composer.AddPass(effectCopy);

        }
        private float amount = 1;
        private Color color = Color.Hex(0xffffff);
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls.Update();

            earth.Rotation.Y += 0.001f;
            earthLight.Rotation.Y += -0.0003f;
            mars.Rotation.Y += -0.001f;
            marsLight.Rotation.Y += +0.0003f;

            composer.Render();
            ShowControls();
        }
        public override void ShowControls()
        {
            ImGui.NewFrame();
            ImGui.Begin("Controls");
            AddSepiaShaderControls(effectSepia);
            AddColorifyShaderControls(effectColorify);
            ImGui.End();
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
        }
        private void AddSepiaShaderControls(ShaderPass pass)
        {
            if (ImGui.TreeNode("SepiaShader"))
            {
                if (ImGui.SliderFloat("amount", ref amount, 0, 10))
                {
                    (pass.uniforms["amount"] as GLUniform)["value"] = amount;
                }

                ImGui.TreePop();
            }
        }
        private void AddColorifyShaderControls(ShaderPass pass)
        {
            System.Numerics.Vector3 color1 = new System.Numerics.Vector3(color.R, color.G, color.B);
            if (ImGui.TreeNode("ColrifyShader"))
            {
                if (ImGui.ColorPicker3("color", ref color1))
                {
                    color.SetRGB(color1.X, color1.Y, color1.Z);
                    (pass.uniforms["color"] as GLUniform)["value"] = color;
                }

                ImGui.TreePop();
            }
        }
    }
}
