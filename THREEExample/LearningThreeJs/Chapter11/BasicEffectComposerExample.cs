using OpenTK;
using THREE;

namespace THREEExample.Learning.Chapter11
{
    [Example("01-BasicEffectComposer", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class BasicEffectComposerExample : EffectComposerTemplate
    {       

        

        public BasicEffectComposerExample() : base()
        {          
        }
        public override void Init()
        {
            base.Init();
            LoadGeometry();
            renderPass = new RenderPass(scene, camera);
            effectFilm = new FilmPass(0.8f, 0.325f, 256.0f, false);
            effectFilm.RenderToScreen = true;

            composer = new EffectComposer(renderer);
            composer.AddPass(renderPass);
            composer.AddPass(effectFilm);

            AddGuiControlsAction = () =>
            {
                AddFilmPassControl("FilmPass", effectFilm); 
            };
        }

        public override void Render()
        {

            base.Render();

            composer.Render();


        }        
    }
}
