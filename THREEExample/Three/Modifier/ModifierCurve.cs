using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Controls;
using THREE.Controls;
using THREE.Core;
using THREE.Extras.Curves;
using THREE.Geometries;
using THREE.Lights;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;
using Vector3 = THREE.Math.Vector3;

namespace THREEExample.Three.Modifier
{
    //[Example("modifier_curve", ExampleCategory.ThreeJs, "Modifier")]
    public class ModifierCurve : Example
    {
        public Scene scene;

        public Camera camera;

        public Raycaster raycaster;

        public TransformControls control;

        public TrackballControls controls;

        public List<Mesh> curveHandles = new List<Mesh>();

        const int ACTION_SELECT = 1;
        const int ACTION_NONE = 0;
        int action = ACTION_NONE;
        
        public ModifierCurve() : base()
        {
            camera = new THREE.Cameras.PerspectiveCamera();
            scene = new Scene();
            stopWatch.Start();
        }
        public virtual void InitCamera()
        {
            camera.Fov = 40.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 1.0f;
            camera.Far = 1000.0f;
            camera.Position.Set(2,2,4);
            camera.LookAt(scene.Position);
        }
        public virtual void InitRenderer()
        {
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public override void Load(GLControl control)
        {
            base.Load(control);

            InitRenderer();
            InitCamera();
        }
        public override void Render()
        {
            renderer.Render(scene, camera);
        }
        public virtual void InitLighting()
        {
            var light = new DirectionalLight(Color.Hex(0xffaa33));
            light.Position.Set(-10, 10, 10);
            light.Intensity = 1.0f;
            scene.Add(light);

            var light2 = new AmbientLight(Color.Hex(0x003973));
            light2.Intensity = 1.0f;
            scene.Add(light2);
        }
        public virtual void BuildScene()
        {

            InitLighting();

            List<Vector3> initialPoints = new List<Vector3>() {
                new Vector3(1,0,-1),
                new Vector3(1,0,1),
                new Vector3(-1,0,1),
                new Vector3(-1,0,-11)               
				};

            var boxGeometry = new BoxGeometry(0.1f, 0.1f, 0.1f);
            var boxMaterial = new MeshBasicMaterial();

            foreach (var handlePos in initialPoints ) {

                var handle = new Mesh(boxGeometry, boxMaterial);
                handle.Position.Copy(handlePos);
                curveHandles.Add(handle);
                scene.Add(handle);
            }

    //        var curve = new CatmullRomCurve3(curveHandles);
    //        );
    //        curve.curveType = 'centripetal';
    //        curve.closed = true;

    //        const points = curve.getPoints(50);
    //        const line = new THREE.LineLoop(
    //            new THREE.BufferGeometry().setFromPoints(points),
    //            new THREE.LineBasicMaterial( { color: 0x00ff00 } )
				//);

				//scene.add(line );
        }
    }
}
