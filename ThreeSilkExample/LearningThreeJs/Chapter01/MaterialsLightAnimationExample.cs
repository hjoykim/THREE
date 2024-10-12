using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Silk;

namespace THREE.Silk.Example
{
    [Example("04-Materials-Light-Animation", ExampleCategory.LearnThreeJS, "Chapter01")]
    public class MaterialsLightAnimationExample : MaterialsLightExample
    {
        public float step = 0.0f;
        public float rotationSpeed = 0.2f;
        public float bouncingSpeed = 0.04f;
        public MaterialsLightAnimationExample() : base() { }
        public override void InitLighting()
        {
            // add subtle ambient lighting
            var ambienLight = new THREE.AmbientLight(0x353535);
            scene.Add(ambienLight);

            // add spotlight for the shadows
            var spotLight = new THREE.SpotLight(0xffffff);
            spotLight.Position.Set(-10, 20, -5);
            spotLight.CastShadow = true;
            scene.Add(spotLight);
        }
        public override void Render()
        {
            // rotate the cube around its axes
            cube.Rotation.X += rotationSpeed;
            cube.Rotation.Y += rotationSpeed;
            cube.Rotation.Z += rotationSpeed;

            // bounce the sphere up and down
            step += bouncingSpeed;
            sphere.Position.X = 20 + (10 * (float)(Math.Cos(step)));
            sphere.Position.Y = 2 + (10 * (float)Math.Abs(Math.Sin(step)));

            base.Render();
        }
    }
}
