using OpenTK;
using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Geometries;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Postprocessing;
using THREEExample.ThreeImGui;

namespace THREEExample.Learning.Chapter11
{
    [Example("06-ambient-occlusion", ExampleCategory.LearnThreeJS, "Chapter11")]
    public class AmbientOcclusionExample : EffectComposerTemplate
    {
        Group totalGroup;
        public AmbientOcclusionExample() : base()
        {

        }
        public override void Load(GLControl control)
        {
            Debug.Assert(null != control);

            glControl = control;
            this.renderer = new THREE.Renderers.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            stopWatch.Start();

            InitRenderer();

            InitCamera();

            InitCameraController();

            imGuiManager = new ImGuiManager(this.glControl);

            var amount = 50;
            var xRange = 20;
            var yRange = 20;
            var zRange = 20;

            totalGroup = new Group();

            for (var i = 0; i < amount; i++)
            {
                var boxGeometry = new BoxBufferGeometry(5, 5, 5);
                var material = new MeshBasicMaterial(){ Color= new Color().Random()};
                var boxMesh = new Mesh(boxGeometry, material);

                var rX = (float)MathUtils.random.NextDouble() * xRange - xRange / 2;
                var rY = (float)MathUtils.random.NextDouble() * yRange - yRange / 2;
                var rZ = (float)MathUtils.random.NextDouble() * zRange - zRange / 2;

                var totalRotation = 2 * (float)Math.PI;

                boxMesh.Position.Set(rX, rY, rZ);
                boxMesh.Rotation.X = (float)MathUtils.random.NextDouble() * totalRotation;
                boxMesh.Rotation.Y = (float)MathUtils.random.NextDouble() * totalRotation;
                boxMesh.Rotation.Z= (float)MathUtils.random.NextDouble() * totalRotation;
                totalGroup.Add(boxMesh);
            }

            scene.Add(totalGroup);

            renderPass = new RenderPass(scene, camera);           
            SSAOPass aoPass = new SSAOPass(scene, camera);
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
        public override void Resize(System.Drawing.Size clientSize)
        {            
            base.Resize(clientSize);
            this.renderer.Clear(true,true,true);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
}
