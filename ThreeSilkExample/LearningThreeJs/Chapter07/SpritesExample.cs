using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Silk;
using Color = THREE.Color;

namespace THREE.Silk.Example
{
    [Example("01.Sprites-Example", ExampleCategory.LearnThreeJS, "Chapter07")]
    public class SpritesExample : Example
    {
        public SpritesExample()
        {

        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(0, 0, 150);
        }
        public override void Init()
        {
            base.Init();

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
