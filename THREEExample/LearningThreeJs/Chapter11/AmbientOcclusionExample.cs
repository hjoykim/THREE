using OpenTK;
using OpenTK.Windowing.Common;
using System;
using System.Diagnostics;
using THREE;
using THREEExample.ThreeImGui;
using Color = THREE.Color;
namespace THREEExample.Learning.Chapter11
{
    [Example("06-ambient-occlusion", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class AmbientOcclusionExample : EffectComposerTemplate
    {
        Group totalGroup;
        public AmbientOcclusionExample() : base()
        {

        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(0, 20, 40);
        }
        public override void Init()
        {
            base.Init();
            var amount = 50;
            var xRange = 20;
            var yRange = 20;
            var zRange = 20;

            totalGroup = new Group();

            for (var i = 0; i < amount; i++)
            {
                var boxGeometry = new BoxBufferGeometry(5, 5, 5);
                var material = new MeshBasicMaterial() { Color = new Color().Random() };
                var boxMesh = new Mesh(boxGeometry, material);

                var rX = (float)MathUtils.random.NextDouble() * xRange - xRange / 2;
                var rY = (float)MathUtils.random.NextDouble() * yRange - yRange / 2;
                var rZ = (float)MathUtils.random.NextDouble() * zRange - zRange / 2;

                var totalRotation = 2 * (float)Math.PI;

                boxMesh.Position.Set(rX, rY, rZ);
                boxMesh.Rotation.X = (float)MathUtils.random.NextDouble() * totalRotation;
                boxMesh.Rotation.Y = (float)MathUtils.random.NextDouble() * totalRotation;
                boxMesh.Rotation.Z = (float)MathUtils.random.NextDouble() * totalRotation;
                totalGroup.Add(boxMesh);
            }

            scene.Add(totalGroup);

            renderPass = new RenderPass(scene, camera);
            SSAOPass aoPass = new SSAOPass(scene, camera,glControl.Width,glControl.Height);
            aoPass.RenderToScreen = true;

            composer = new EffectComposer(renderer);
            composer.AddPass(renderPass);
            composer.AddPass(aoPass);
        }


        public override void Render()
        {
            controls.Update();
            totalGroup.Rotation.X += 0.0001f;
            totalGroup.Rotation.Y += 0.001f;
            composer.Render();
            
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
            composer.SetSize(clientSize.Width, clientSize.Height);
        }

    }
}
