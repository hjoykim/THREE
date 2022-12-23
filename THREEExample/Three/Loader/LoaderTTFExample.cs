using System;
using System.Collections;
using System.Windows.Forms;
using THREE;
using Color = THREE.Color;

namespace THREEExample.Three.Loaders
{
    [Example("loader ttf", ExampleCategory.ThreeJs, "loader")]
    public class LoaderTTFExample : ExampleTemplate
    {
        Vector3 cameraTarget = new Vector3(0, 150, 0);
        Group group;
        Material material;

        Hashtable options;

        Mesh textMesh1, textMesh2;
        TextBufferGeometry textGeo;

        string text = "three.js";

        bool firstLetter = true;

        float targetRotation = 0;
        float targetRotationOnPointerDown = 0;

        int pointerX = 0;
        int pointerXOnPointerDown = 0;
        int windowHalfX;
        public LoaderTTFExample() : base()
        {
            scene.Background = Color.Hex(0x000000);
            scene.Fog = new Fog(0x000000, 250, 1400);
        }
        public override void InitRenderer()
        {
            base.InitRenderer();
        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Fov = 30;
            camera.Near = 0.1f;
            camera.Far = 1500;
            camera.Position.Set(0, 400, 700);
        }

        public override void InitLighting()
        {
            var dirLight = new DirectionalLight(0xffffff, 0.125f);
            dirLight.Position.Set(0, 0, 1).Normalize();
            scene.Add(dirLight);

            var pointLight = new PointLight(0xffffff, 1.5f);
            pointLight.Position.Set(0, 100, 90);
            pointLight.Color.SetHSL(MathUtils.NextFloat(), 1, 0.5f);
            scene.Add(pointLight);
        }
      
        public override void Init()
        {
            base.Init();

            windowHalfX = glControl.Width / 2;

            group = new Group();
            group.Position.Y = 100;

           

            material = new MeshPhongMaterial() { Color = Color.Hex(0xffffff), FlatShading = true };

           

            CreateText();

            scene.Add(group);

            var plane = new Mesh(
                   new PlaneBufferGeometry(10000, 10000),
                   new MeshBasicMaterial() { Color = Color.Hex(0xffffff), Opacity = 0.5f, Transparent = true }
           );
            plane.Position.Y = 100;
            plane.Rotation.X = -(float)Math.PI / 2;
            scene.Add(plane);

            glControl.KeyDown += OnKeyDown;
            glControl.KeyPress += OnKeyPress;
            glControl.MouseDown += OnMouseDown;

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (firstLetter)
            {
                firstLetter = false;
                text = "";
            }
            var keyCode = e.KeyCode;

            if (keyCode == Keys.Back)
            {
                text = text.Substring(0, text.Length - 1);

                RefreshText();
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Back)
            {
                text = text.Substring(0, text.Length - 1);

                RefreshText();
            }
            else
            {
                text = text + e.KeyChar;

                RefreshText();
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            pointerXOnPointerDown = e.X - windowHalfX;
            targetRotationOnPointerDown = targetRotation;

            glControl.MouseMove += OnMouseMove;
            glControl.MouseUp += OnMouseUp;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            glControl.MouseMove -= OnMouseMove;
            glControl.MouseUp -= OnMouseUp;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            pointerX = e.X - windowHalfX;

            targetRotation = targetRotationOnPointerDown + (pointerX - pointerXOnPointerDown) * 0.02f;
        }

        int size = 70;
        int height = 20;
        int hover = 30;
        int bevelThickness = 2;
        float bevelSize = 1.5f;
        int bevelSegments = 3;
        int curveSegments = 4;
        bool bevelEnabled = true;
        int steps = 1;
        bool mirror = true;


        private void CreateText()
        {
            TTFFont ttfFont = new TTFFont(@"../../../assets/fonts/ttf/kenpixel.ttf");
            //var geometry = CreateTextGeometry();
            options = new Hashtable()
            {
                {"size",size },
                {"height",height },
                {"bevelThickness", bevelThickness },
                {"bevelSize", bevelSize },
                {"bevelSegments", bevelSegments },
                {"bevelEnabled", bevelEnabled },
                {"curveSegments", curveSegments },
                {"steps", steps }
            };
            textGeo = ttfFont.CreateTextGeometry(text, options);
            //textGeo.ApplyMatrix4(new Matrix4().MakeScale(0.05f, 0.05f, 0.05f));

            textGeo.ComputeBoundingBox();
            textGeo.ComputeVertexNormals();

            var centerOffset = -0.5f * (textGeo.BoundingBox.Max.X - textGeo.BoundingBox.Min.X);                      

            textMesh1 = new Mesh(textGeo, material);

            textMesh1.Position.X = centerOffset;
            textMesh1.Position.Y = hover;
            textMesh1.Position.Z = 0;
            

            textMesh1.Rotation.X = 0;
            textMesh1.Rotation.Y = (float)Math.PI * 2;

           

            group.Add(textMesh1);

            if (mirror)
            {

                textMesh2 = new Mesh(textGeo, material);

                textMesh2.Position.X = centerOffset;
                textMesh2.Position.Y = -hover;
                textMesh2.Position.Z = height;

                textMesh2.Rotation.X = (float)Math.PI;
                textMesh2.Rotation.Y = (float)Math.PI * 2;

                group.Add(textMesh2);

            }
        }

        public override void Render()
        {
            group.Rotation.Y += (targetRotation - group.Rotation.Y) * 0.05f;

            camera.LookAt(cameraTarget);

            renderer.Render(scene, camera);
        }
        private void RefreshText()
        {
            group.Remove(textMesh1);
            if (mirror) group.Remove(textMesh2);
            if (text == null || text.Length == 0) return;

            CreateText();
        }

        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);

            windowHalfX = glControl.Width / 2;
        }

    }
    
}
