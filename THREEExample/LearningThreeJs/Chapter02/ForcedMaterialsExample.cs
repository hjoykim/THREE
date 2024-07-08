using THREE;

namespace THREEExample.Learning.Chapter02
{
    [Example("03-ForcedMaterials", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class ForcedMaterialsExample : BasicSceneExample
    {
        public ForcedMaterialsExample() : base()
        {
            scene.OverrideMaterial = new MeshLambertMaterial() { Color = new THREE.Color().SetHex(0xffffff) };
        }
    }
}
