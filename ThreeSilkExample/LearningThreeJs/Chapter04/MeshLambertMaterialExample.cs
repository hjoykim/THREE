using THREE;
using THREE.Silk;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("06.Mesh-Lambert-Material", ExampleCategory.LearnThreeJS, "Chapter04")]
    public class MeshLambertMaterialExample : BasicMeshMaterialExample
    {
        public MeshLambertMaterialExample() : base()
        {

        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.X = -30;
            camera.Position.Y = 50;
            camera.Position.Z = 40;
            camera.LookAt(new Vector3(10, 0, 0));

        }
        public override void BuildMeshMaterial()
        {
            meshMaterial = new MeshLambertMaterial();
            meshMaterial.Color = Color.Hex(0x7777ff);
            meshMaterial.Name = "MeshLambertMaterial";
        }

    }
}
