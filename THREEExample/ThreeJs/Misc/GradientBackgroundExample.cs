using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using THREE;
using Typography.OpenFont.Tables;

namespace THREEExample.ThreeJs.Loader
{
    [Example("GradientBackground", ExampleCategory.ThreeJs, "Misc")]
    public class GradientBackgroundExample : LightHintExample
    {
        Scene backgroundScene;
        Camera backgroundCamera;
        public GradientBackgroundExample() 
        {
           
        }
        public override void Init()
        {
            base.Init();
            var texture = TextureLoader.Load("../../../../assets/textures/background.png");
            var backgroundMesh = new THREE.Mesh(
                new PlaneBufferGeometry(2, 2, 0),
                new MeshBasicMaterial(){
                    Map = texture,
                    DepthTest = false,
                    DepthWrite = false
                });

            backgroundScene = new THREE.Scene();
            backgroundCamera = new THREE.Camera();
            backgroundScene.Add(backgroundCamera );
            backgroundScene.Add(backgroundMesh );

        }
        public override void Render()
        {
            renderer.AutoClear = false;
            renderer.Clear();
            renderer.Render(backgroundScene,backgroundCamera);
            base.Render();
        }

    }
}
