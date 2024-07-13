using ImGuiNET;
using OpenTK;
using THREE;


namespace THREEExample.Learning.Chapter11
{
    [Example("02-SimplePass1", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class SimplePassExample1 : EffectComposerTemplate
    {

        public SimplePassExample1() : base()
        {
           
        }

        public override void Init()
        {
            base.Init();

            LoadGeometry();
            // setup effects
            renderPass = new RenderPass(scene, camera);
            effectFilm = new FilmPass(0.8f, 0.325f, 256.0f, false);
            effectFilm.RenderToScreen = true;

            bloomPass = new BloomPass();
            dotScreenPass = new DotScreenPass();
            var copyShader = new CopyShader();
            var effectCopy = new ShaderPass(copyShader);
            effectCopy.RenderToScreen = true;

            composer = new EffectComposer(renderer);
            composer.AddPass(renderPass);
            composer.AddPass(effectCopy);

            // reuse the rendered scene from the composer
            renderedScene = new TexturePass(composer.RenderTarget2.Texture);

            // define the composers
            effectFilmComposer = new EffectComposer(renderer);
            effectFilmComposer.AddPass(renderedScene);
            effectFilmComposer.AddPass(effectFilm);

            bloomComposer = new EffectComposer(renderer);
            bloomComposer.AddPass(renderedScene);
            bloomComposer.AddPass(bloomPass);
            bloomComposer.AddPass(effectCopy);

            dotScreenComposer = new EffectComposer(renderer);
            dotScreenComposer.AddPass(renderedScene);
            dotScreenComposer.AddPass(dotScreenPass);
            dotScreenComposer.AddPass(effectCopy);

            AddGuiControlsAction = () =>
            {
                AddFilmPassControl("FilmPass", effectFilm);
                AddDotScreenPassControl("DotScreenPass", dotScreenPass);
                AddBloomPassControl("BloomPass", bloomPass);
            };
        }


        public override void Render()
        {

            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;

            earth.Rotation.Y += 0.001f;
            pivot.Rotation.Y += -0.0003f;

            renderer.AutoClear = false;
            renderer.Clear();

            renderer.SetViewport(halfWidth, halfHeight, halfWidth, halfHeight);
            composer.Render();

            renderer.SetViewport(0, 0, halfWidth, halfHeight);
            effectFilmComposer.Render();

            renderer.SetViewport(0, halfHeight, halfWidth, halfHeight);
            bloomComposer.Render();

            renderer.SetViewport(halfWidth, 0, halfWidth, halfHeight);
            dotScreenComposer.Render();

            renderer.SetViewport(0, 0, renderer.Width, renderer.Height);
        }

    }
}
