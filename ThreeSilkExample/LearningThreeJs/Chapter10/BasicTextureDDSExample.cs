using System.Diagnostics;
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;

namespace THREE.Silk.Example.Learning.Chapter10
{
    [Example("02-Basic-texture-dds",ExampleCategory.LearnThreeJS,"Chapter10")]
    public class BasicTextureDDSExample : TemplateExample
    {
        public BasicTextureDDSExample() : base() { }
        public override void SetGeometryWithTexture()
        {
            var groundPlane = DemoUtils.AddLargeGroundPlane(scene);
            groundPlane.Position.Y = -10;

            scene.Add(new AmbientLight(0x444444));

            var texture = TextureLoader.LoadDDS("../../../../assets/textures/dds/test-dxt1.dds");

            texture.flipY = true;
            var polyhedron = new IcosahedronBufferGeometry(8, 0);
            polyhedronMesh = AddGeometry(scene, polyhedron, "polyhedron", texture);
            polyhedronMesh.Position.X = 20;

            var sphere = new SphereBufferGeometry(5, 20, 20);
            sphereMesh = AddGeometry(scene, sphere, "sphere", texture);

            var cube = new BoxBufferGeometry(10, 10, 10);
            cubeMesh = AddGeometry(scene, cube, "cube", texture);
            cubeMesh.Position.X = -20;
        }
    }
}
