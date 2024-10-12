using THREE;
using THREE.Silk;
using Color = THREE.Color;
namespace THREE.Silk.Example
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
