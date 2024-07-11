using THREE;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter07
{
    [Example("10-Sprites-3D",ExampleCategory.LearnThreeJS,"Chapter07")]
    public class Sprites3DExample :Example
    {


        Group group;

        float step = 0;
        int sprite = 0;
        public Sprites3DExample()
        {

        }

        public override void Init()
        {
            base.Init();

            camera.Position.Set(20, 0, 150);

            camera.LookAt(new Vector3(20, 30, 0));

            renderer.SetClearColor(0x000000);

            var texture = TextureLoader.Load(@"../../../../assets/textures/particles/sprite-sheet.png");

            CreateSprites(texture);

           
        }

        public override void Render()
        {
           
            base.Render();
            group.Rotation.X += 0.01f;
           
        }
        public void CreateSprites(Texture texture)
        {
            group = new Group();

            var range = 200;

            for(int i = 0; i < 400; i++)
            {
                group.Add(CreateSprite(10, false, 0.6f, Color.Hex(0xffffff), i % 5, range, texture));
            }

            scene.Add(group);
        }
        private Sprite CreateSprite(float size,bool transparent,float opacity,Color color,int spriteNumber,int range,Texture texture)
        {
            var spriteMaterial = new SpriteMaterial(){
                Opacity= opacity,
                Color= color,
                Transparent= transparent,
                Map= texture
            };

            // we have 1 row, with five sprites
            spriteMaterial.Map.Offset = new Vector2(0.2f * spriteNumber, 0);
            spriteMaterial.Map.Repeat = new Vector2(1f / 5, 1);
            spriteMaterial.Blending = Constants.AdditiveBlending;
            // make sure the object is always rendered at the front
            spriteMaterial.DepthTest = false;

            var sprite = new Sprite(spriteMaterial);
            sprite.Scale.Set(size, size, size);
            sprite.Position.Set(
                (float)random.NextDouble()*range -range/2.0f, 
                (float)random.NextDouble()*range-range/2.0f, 
                (float)random.NextDouble()*range -range/2.0f);

           

            return sprite;
        }

    }
}
