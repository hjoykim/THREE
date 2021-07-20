using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Geometries;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;
using THREEExample.Learning.Utils;

namespace THREEExample.Learning.Chapter11
{
    public class Util11
    {
        public static (Mesh mesh,Scene light) AddEarth(Scene scene)
        {
            var planetMaterial = new MeshPhongMaterial
            {
                Map = TextureLoader.Load("../../../assets/textures/earth/Earth.png"),
                NormalMap = TextureLoader.Load("../../../assets/textures/earth/EarthNormal.png"),
                SpecularMap = TextureLoader.Load("../../../assets/textures/earth/EarthSpec.png"),
                Specular = new Color(0x4444aa),
                NormalScale = new Vector2(6, 6),
                Shininess = 0.5f
            };

            var earth = new Mesh(new SphereBufferGeometry(15, 40, 40), planetMaterial);
            scene.Add(earth);
            var pivot = new Scene();
            DemoUtils.InitDefaultLighting(pivot);
            scene.Add(pivot);

            return (earth, pivot);
        }

        public static (Mesh mesh,Scene light) AddMars(Scene scene)
        {
            var planetMaterial = new MeshPhongMaterial
            {
                Map = TextureLoader.Load("../../../assets/textures/mars/mars_1k_color.jpg"),
                NormalMap = TextureLoader.Load("../../../assets/textures/mars/mars_1k_normal.jpg"),
                NormalScale = new Vector2(6, 6),
                Shininess = 0.5f
            };

            var mars = new Mesh(new SphereBufferGeometry(15, 40, 40), planetMaterial);
            scene.Add(mars);

            var pivot = new Scene();
            DemoUtils.InitDefaultLighting(pivot);
            scene.Add(pivot);

            return (mars, pivot);
        }

    }
}
