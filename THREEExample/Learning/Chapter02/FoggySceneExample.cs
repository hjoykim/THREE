using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Scenes;

namespace THREEExample.Learning.Chapter02
{
    [Example("02-Foggy Scene", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class FoggySceneExample : BasicSceneExample
    {

        public FoggySceneExample() : base()
        {
            scene.Fog = new Fog(System.Drawing.Color.FromArgb(0xffffff), 0.015f, 100f);
        }
    }
}
