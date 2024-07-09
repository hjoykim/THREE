using THREE;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter04
{
    [Example("10.Mesh-Physical-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class MeshPhysicalMaterialExample : MeshToonMaterialExample
    {
        public MeshPhysicalMaterialExample() : base()
        {

        }
        public override void BuildMeshMaterial()
        {
            meshMaterial = new MeshPhysicalMaterial();
            meshMaterial.Color = Color.Hex(0x7777ff);
            meshMaterial.Name = "MeshToonMaterial";
        }
    }
}
