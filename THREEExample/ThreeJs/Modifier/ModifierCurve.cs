using OpenTK;
using System.Collections.Generic;
using THREE;
using Vector3 = THREE.Vector3;
using Color = THREE.Color;
namespace THREEExample.Three.Modifier
{
    //[Example("modifier_curve", ExampleCategory.ThreeJs, "Modifier")]
    public class ModifierCurve : Example
    {
        public Raycaster raycaster;

        public TransformControls transformControl;

        public List<Mesh> curveHandles = new List<Mesh>();

        const int ACTION_SELECT = 1;
        const int ACTION_NONE = 0;
        int action = ACTION_NONE;
        
        public ModifierCurve() : base()
        {
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 40.0f;
            camera.Aspect = this.glControl.AspectRatio;
            camera.Near = 1.0f;
            camera.Far = 1000.0f;
            camera.Position.Set(2,2,4);
            camera.LookAt(scene.Position);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }

        public override void Init()
        {
            base.Init();
            BuildScene();
        }
        public override void InitLighting()
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
