using THREE;
using THREE.Silk;
namespace THREE.Silk.Example
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
