using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Materials;
using THREE.Math;

namespace THREEExample.Learning.Chapter02
{
    [Example("03-ForcedMaterials", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class ForcedMaterialsExample : BasicSceneExample
    {
        public ForcedMaterialsExample() : base()
        {
            scene.OverrideMaterial = new MeshLambertMaterial() { Color = new Color().SetHex(0xffffff) };
        }
    }
}
