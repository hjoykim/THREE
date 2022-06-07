using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Controls;
using THREE.Geometries;
using THREE.Helpers;
using THREE.Lights;
using THREE.Loaders;
using THREE.Materials;
using THREE.Math;
using THREE.Objects;
using THREE.Scenes;

namespace THREEExample.Three.Misc.Controls
{
    //[Example("controls_transform", ExampleCategory.Misc, "Controls")]
    public class Controls_Transform : Example
    {
        public Scene scene;

        public Camera cameraPersp,cameraOrtho,currentCamera;

        public OrbitControls orbit;

        public THREE.Controls.TransformControls control;

        Mesh mesh;
        public Controls_Transform() : base()
        {
            scene = new Scene();
           
        }
        public virtual void InitCamera()
        {
            cameraPersp = new PerspectiveCamera(50, this.glControl.AspectRatio, 0.01f, 30000);
            cameraOrtho = new OrthographicCamera(-600 * this.glControl.AspectRatio, 600 * this.glControl.AspectRatio, 600, -600, 0.01f, 30000);
            currentCamera = cameraPersp;

            currentCamera.Position.Set(1000, 500, 1000);
            currentCamera.LookAt(0, 200, 0);
        }

        private void Change(object obj)
        {
            Render();
        }

        public virtual void InitController()
        {
            orbit = new OrbitControls(this.glControl, currentCamera);
            orbit.Update();

            control = new THREE.Controls.TransformControls(this.glControl, currentCamera);
            //control._changeEvent = Change;
            control.PropertyChanged += Control_PropertyChanged;

            scene.Add(control);
        }

        private void Control_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("dragging"))
            {
                orbit.Enabled = !(sender as THREE.Controls.TransformControls).dragging;
            }
        }

        public virtual void InitLighting()
        {
            var light = new DirectionalLight(Color.Hex(0xffffff), 2);
            light.Position.Set(1, 1, 1);
            scene.Add(light);
        }
        public virtual void InitRenderer()
        {
            this.renderer.SetClearColor(Color.Hex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;
        }
        public virtual void BuildScene()
        {
            InitRenderer();
            InitCamera();
            InitLighting();
            InitController();

            scene.Add(new GridHelper(1000, 10, 0x888888, 0x444444));

            var texture = TextureLoader.Load(@"../../../assets/textures/crate.gif");

            var geometry = new BoxGeometry(200, 200, 200);
            var material = new MeshLambertMaterial() { Map= texture, Transparent= true } ;
            mesh = new Mesh(geometry, material);
            //scene.Add(mesh);

            //control.Attach(mesh);

            control.Visible = true;

        }
        public override void Load(GLControl control)
        {
            base.Load(control);

            BuildScene();
        }

        public override void Render()
        {
            this.renderer.Render(scene, currentCamera);
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);

            cameraPersp.Aspect = this.glControl.AspectRatio;
            cameraPersp.UpdateProjectionMatrix();

            cameraOrtho.Left = cameraOrtho.Bottom * this.glControl.AspectRatio;
            cameraOrtho.CameraRight = cameraOrtho.Top * this.glControl.AspectRatio;
            cameraOrtho.UpdateProjectionMatrix();
        }
    }
}
