using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using Color = THREE.Color;

namespace THREEExample.ThreeJs.postprocessing
{
    [Example("OutlinePass", ExampleCategory.ThreeJs, "PostPorcessing")]
    public class OutlinePassExample : Example
    {
        EffectComposer composer;

        OutlinePass outlinePass;

        ShaderPass effectFXAA;

        Object3D obj3d = new Object3D();

        Group group = new Group();

        List<Object3D> selectedObjects;

        Raycaster raycaster = new Raycaster();

        Vector2 mouse = new Vector2(1, 1);

        public OutlinePassExample() :base()
        {
            scene.Background = Color.Hex(0x000000);
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Near = 0.1f;
            camera.Far = 100;
            camera.Position.Set(0, 0, 8);
            camera.UpdateProjectionMatrix();
        }
        public override void InitLighting()
        {
            base.InitLighting();
            scene.Add(new THREE.AmbientLight(0xaaaaaa, 0.2f));

            var light = new THREE.DirectionalLight(0xddffdd, 0.6f);
            light.Position.Set(1, 1, 1);
            light.CastShadow = true;
            light.Shadow.MapSize.Width = 1024;
            light.Shadow.MapSize.Height = 1024;

            var d = 10;

            light.Shadow.Camera.Left = -d;
            light.Shadow.Camera.CameraRight = d;
            light.Shadow.Camera.Top = d;
            light.Shadow.Camera.Bottom = -d;
            light.Shadow.Camera.Far = 1000;

            scene.Add(light);
        }
        public override void Init()
        {
            base.Init();
            InitGeometry();
            this.MouseMove += OnMouseMove;

        }
        private void InitGeometry()
        {
            OBJLoader loader = new OBJLoader();
            var object3d = loader.Parse("../../../../assets/models/obj/tree.obj");
            var scale = 1.0f;
            foreach (var child in object3d.Children)
            {
                if(child is THREE.Mesh)
                {
                    child.Geometry.Center();
                    child.Geometry.ComputeBoundingSphere();
                    scale = 0.2f * child.Geometry.BoundingSphere.Radius;
                    var phongMaterial = new MeshPhongMaterial() { Color = Color.Hex(0xffffff), Specular = Color.Hex(0x111111), Shininess = 5 };
                    child.Material = phongMaterial;
                    child.ReceiveShadow = true;
                    child.CastShadow = true;
                }
            }
            object3d.Position.Y = -2.5f;
            object3d.Scale.DivideScalar(scale);
            obj3d.Add(object3d);
            group.Add(obj3d);
            scene.Add(group);

            var geometry = new THREE.SphereGeometry(3, 48, 24);

            for (var i = 0; i < 20; i++)
            {

                var material = new THREE.MeshLambertMaterial();
                material.Color = new THREE.Color().SetHSL(MathUtils.NextFloat(), 1.0f, 0.3f);

                var mesh = new THREE.Mesh(geometry, material);
                mesh.Position.X = MathUtils.NextFloat() * 4 - 2;
                mesh.Position.Y = MathUtils.NextFloat() * 4 - 2;
                mesh.Position.Z = MathUtils.NextFloat() * 4 - 2;
                mesh.ReceiveShadow = true;
                mesh.CastShadow = true;
                mesh.Scale.MultiplyScalar(MathUtils.NextFloat() * 0.3f + 0.1f);
                group.Add(mesh);

            }

            var floorMaterial = new THREE.MeshLambertMaterial() { Color = Color.Hex(0xcccccc),Side = Constants.DoubleSide };

			var floorGeometry = new THREE.PlaneGeometry(12, 12);
            var floorMesh = new THREE.Mesh(floorGeometry, floorMaterial);
            floorMesh.Rotation.X -= (float)Math.PI* 0.5f;
			floorMesh.Position.Y -= 1.5f;
			group.Add(floorMesh );
			floorMesh.ReceiveShadow = true;

			var torusGeometry = new THREE.TorusGeometry(1, 0.3f, 16, 100);
            var torusMaterial = new THREE.MeshPhongMaterial() { Color = Color.Hex(0xffaaff) };
			var torus = new THREE.Mesh(torusGeometry, torusMaterial);
            torus.Position.Z = - 4;
			group.Add(torus );
			torus.ReceiveShadow = true;
			torus.CastShadow = true;

            composer = new EffectComposer(renderer);

            var renderPass = new RenderPass(scene, camera);
            composer.AddPass(renderPass);

            outlinePass = new OutlinePass(new THREE.Vector2(glControl.Width, glControl.Height), scene, camera);
            composer.AddPass(outlinePass);

            var texture = TextureLoader.Load("../../../../assets/textures/tri_pattern.jpg");

            outlinePass.patternTexture = texture;
            texture.WrapS = Constants.RepeatWrapping;
            texture.WrapT = Constants.RepeatWrapping;
            texture.NeedsUpdate = true;


            effectFXAA = new ShaderPass(new FXAAShader());
            (((effectFXAA.uniforms as GLUniforms)["resolution"] as GLUniform)["value"] as Vector2).Set(1 / glControl.Width, 1 / glControl.Height);
            composer.AddPass(effectFXAA);
        }

        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
            composer.SetSize(clientSize.Width, clientSize.Height);
            (((effectFXAA.uniforms as GLUniforms)["resolution"] as GLUniform)["value"] as Vector2).Set(1 / clientSize.Width, 1 / clientSize.Height);
        }

        public override void Render()
        {
            if (!imGuiManager.ImWantMouse) controls.Enabled = true;
            else controls.Enabled = false;
            controls?.Update();

            group.Rotation.Y += 0.0001f;
            composer.Render();
        }
        private void addSelectedObject(Object3D object3D)
        {
            selectedObjects = new List<Object3D>();
            selectedObjects.Add(object3D);
        }

        private void checkIntersection()
        {
            raycaster.SetFromCamera(mouse, camera);
            var intersects = raycaster.IntersectObject(scene, true);
            if (intersects.Count > 0)
            {
                var selectedObject = intersects[0].object3D;
                addSelectedObject(selectedObject);
                outlinePass.selectedObjects = selectedObjects;
            }
            else
            {
                outlinePass.selectedObjects.Clear();
            }
        }
        private void OnMouseMove(object sender, THREE.MouseEventArgs e)
        {
            mouse.X = e.X * 1.0f / ClientRectangle.Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / ClientRectangle.Height * 2 + 1.0f;
            checkIntersection();
        }
    }
}
