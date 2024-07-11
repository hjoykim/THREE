using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREEExample;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter07
{
    [Example("02.Points-Example", ExampleCategory.LearnThreeJS, "Chapter07")]
    public class PointsExample : Example
    {
        public PointsExample() :base()
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

            CreatePoints();

        }

        private void CreatePoints()
        {
            Random random = new Random();

            var geom = new Geometry();

            var material = new PointsMaterial() { Size=2.0f,VertexColors=true,Color=Color.Hex(0xffffff) };
            
            for (int x = -15; x < 15; x++)
            {
                for (int y = -10; y < 10; y++)
                {
                    var particle = new Vector3(x * 4, y * 4, 0);

                    geom.Vertices.Add(particle);
                    geom.Colors.Add(new Color().Random());
                }
            }

            var cloud = new Points(geom, material);
            scene.Add(cloud);
        }
    }
}
