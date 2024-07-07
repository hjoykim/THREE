using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREEExample;

namespace THREEDemo.Learning.Chapter07
{
    [Example("01.Sprites-Example", ExampleCategory.LearnThreeJS, "Chapter07")]
    public class SpritesExample : ExampleTemplate
    {
        public SpritesExample()
        {

        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(0, 0, 150);
        }
        public override void Load(GLControl control)
        {
            base.Load(control);

            CreateSprites();
          
        }

        private void CreateSprites()
        {
            Random random = new Random();
            for (int x = -15; x < 15; x++)
            {
                for (int y = -10; y < 10; y++)
                {
                    var material = new SpriteMaterial() { Color = new Color().Random() };

                    var sprite = new Sprite(material);
                    sprite.Position.Set(x * 4, y * 4, 0);
                    scene.Add(sprite);
                }
            }
        }
    }
}
