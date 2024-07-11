using THREE;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter07
{
    [Example("07.Rainy-Scene", ExampleCategory.LearnThreeJS, "Chapter07")]
    public class RainySceneExample : Example
    {

        public class Particle : Vector3
        {
            public float VelocityX;

            public float VelocityY;

            public Particle(float x, float y, float z) : base(x, y, z)
            {
            }
        }

        float step = 0.0f;
        Points cloud;
        public RainySceneExample()
        {

        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(20, 0, 150);
        }
        public override void Init()
        {
            base.Init();
            renderer.SetClearColor(0x000000);
            CreatePointCloud(3,true,0.6f,true,Color.Hex(0xffffff));
        }

        public override void Render()
        {
            base.Render();
            var vertices = cloud.Geometry.Vertices;
            vertices.ForEach(delegate (Vector3 v1)
            {
                var v = v1 as Particle;
                v.Y = v.Y - (v.VelocityY);
                v.X = v.X - (v.VelocityX);

                if (v.Y <= 0) v.Y = 60;
                if (v.X <= -20 || v.X >= 20) v.VelocityX = v.VelocityX * -1;
            });
            cloud.Geometry.VerticesNeedUpdate = true;
        }
        
        private void CreatePointCloud(float size,bool transparent,float opacity,bool sizeAttenuation,Color color)
        {

            var texture = TextureLoader.Load(@"..\..\..\..\assets\textures\particles\raindrop-3.png");

            var geom = new Geometry();

            var material = new PointsMaterial() { Size=size,Opacity=opacity,Transparent=transparent,SizeAttenuation=sizeAttenuation,Color=color,Map=texture};

            int range = 40;
            for(int i = 0; i < 1500; i++)
            {
                Particle particle = new Particle((float)random.NextDouble() * range , (float)random.NextDouble() * range*1.5f, 1+(i/100.0f));
                particle.VelocityY = 0.1f + (float)random.NextDouble() / 5;
                particle.VelocityX = (float)(random.NextDouble() - 0.5f) / 3;
                geom.Vertices.Add(particle);
               
            }

            cloud = new Points(geom, material);
            cloud.Name = "Particle";
            scene.Add(cloud);
        }
    }
}
