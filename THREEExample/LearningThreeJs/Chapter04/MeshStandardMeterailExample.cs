using THREE;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter04
{
    [Example("09.Mesh-Standard-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class MeshStandardMeterailExample : MeshToonMaterialExample
    {
        public MeshStandardMeterailExample() : base()
        {

        }
        public override void BuildMeshMaterial()
        {
            meshMaterial = new MeshStandardMaterial();
            meshMaterial.Color = Color.Hex(0x7777ff);
            meshMaterial.Name = "MeshStandardMaterial";

        }
    }
}
