using System;
using System.Collections.Generic;
using System.Diagnostics;
using THREE;
using THREE.Silk;
using THREE.Silk.Example.Learning.Utils;

namespace THREE.Silk.Example
{
    [Example("18-env-map-dynamic", ExampleCategory.LearnThreeJS, "Chapter10")]
    public class EnvMapDynamicExample : TemplateExample
    {
        Mesh cube1, sphere1;
        CubeCamera cubeCamera;
        public EnvMapDynamicExample() : base()
        {

        }
        public override void SetGeometryWithTexture()
        {
            var urls = new List<string> {
                "../../../../assets/textures/cubemap/colloseum/right.png",
                "../../../../assets/textures/cubemap/colloseum/left.png",
                "../../../../assets/textures/cubemap/colloseum/top.png",
                "../../../../assets/textures/cubemap/colloseum/bottom.png",
                "../../../../assets/textures/cubemap/colloseum/front.png",
                "../../../../assets/textures/cubemap/colloseum/back.png"
            };

            var cubeTexture = CubeTextureLoader.Load(urls);
            scene.Background = cubeTexture;

            var cubeMaterial = new MeshStandardMaterial
            {
                EnvMap = scene.Background as CubeTexture,
                Color = new THREE.Color(0xffffff),
                Metalness = 1.0f,
                Roughness = 0.0f,
            };

            var sphereMaterial = cubeMaterial.Clone() as MeshStandardMaterial;
            sphereMaterial.NormalMap = TextureLoader.Load("../../../../assets/textures/engraved/Engraved_Metal_003_NORM.jpg");
            sphereMaterial.AoMap = TextureLoader.Load("../../../../assets/textures/engraved/Engraved_Metal_003_OCC.jpg");
            var cubeRenderTarget = new GLCubeRenderTarget(512);
            cubeCamera = new CubeCamera(0.1f, 100, cubeRenderTarget);
            scene.Add(cubeCamera);


            var cube = new BoxBufferGeometry(26, 22, 12);
            cube1 = AddGeometryWithMaterial(scene, cube, "cube", cubeMaterial);
            cube1.Position.X = -15;
            cube1.Rotation.Y = -1 / 3 * (float)Math.PI;
            cubeCamera.Position.Copy(cube1.Position);
            cubeMaterial.EnvMap = cubeCamera.RenderTarget;

            var sphere = new SphereBufferGeometry(5, 50, 50);
            sphere1 = AddGeometryWithMaterial(scene, sphere, "sphere", sphereMaterial);
            sphere1.Position.X = 15;
        }

 
        public override void Render()
        {
            if (!this.ImWantMouse)
                controls.Enabled = true;
            else
                controls.Enabled = false;

            controls.Update();
            cube1.Visible = false;
            cubeCamera.Update(this.renderer, scene);
            cube1.Visible = true;

            this.renderer.Render(scene, camera);
            cube1.Rotation.Y += 0.01f;
            sphere1.Rotation.Y -= 0.01f;
        }
    }

}
