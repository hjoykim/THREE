using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using THREE.Silk;
namespace THREE.Silk.Example.Chapter02
{
    [Example("08.Cameras-LookAt", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class CameraLookAtExample : BothCameraExample
    {
        float step = 0.0f;
        Mesh lookAtMesh;
        public CameraLookAtExample() :base(){ }
        public override void BuildScene()
        {
            base.BuildScene();
            var lookAtGeom = new SphereGeometry(2, 20, 20);
            lookAtMesh = new Mesh(lookAtGeom, new MeshLambertMaterial() { Color = THREE.Color.Hex(0x00ff00) });
            scene.Add(lookAtMesh);
        }
        public override void Render()
        {
            step += 0.02f;

            var x = 10 + (100 * (float)System.Math.Sin(step));
            camera.LookAt(new Vector3(x, 10, 0));
            lookAtMesh.Position.Set(x, 10, 0);
            base.Render();
            //renderer.Render(scene, camera);
        }
    }
}
