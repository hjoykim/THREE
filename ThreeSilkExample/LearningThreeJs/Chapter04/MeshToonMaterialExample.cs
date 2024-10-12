using THREE;
using THREE.Silk;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("08.Mesh-Toon-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class MeshToonMaterialExample : MeshPhongMaterialExample
    {
        public MeshToonMaterialExample() : base()
        {

        }
        public override void BuildMeshMaterial()
        {
            meshMaterial = new MeshToonMaterial();
            meshMaterial.Color = Color.Hex(0x7777ff);
            meshMaterial.Name = "MeshToonMaterial";

        }
        public override void AddSpotLight()
        {
            base.AddSpotLight();
            spotLight.Position.Set(0, 30, 60);
        }

    }
}
