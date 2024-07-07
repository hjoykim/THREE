using OpenTK;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using THREE;
using Vector3 = THREE.Vector3;

namespace THREEExample.Learning.Chapter02
{
    [Example("04-Geometries", ExampleCategory.LearnThreeJS, "Chapter02")]
    public class GeometriesExample :Example
    {
        public GeometriesExample() : base()
        {

        }
        public override void InitCamera()
        {
            base.InitCamera();
            camera.Position.Set(-50.0f, 30.0f, 20.0f);
            camera.LookAt(new Vector3(-10, 0, 0));
        }

        public override void InitRenderer()
        {
            base.InitRenderer();
            this.renderer.SetClearColor(THREE.Color.Hex(0xEEEEEE), 1);
            this.renderer.ShadowMap.Enabled = true;
        }

        public override void Load(IThreeWindow glControl)
        {
            base.Load(glControl);

            var planeGeometry = new PlaneGeometry(60, 40, 1, 1);
            MeshLambertMaterial planeMaterial = new MeshLambertMaterial() { Color = new Color().SetHex(0xffffff) };

            var plane = new Mesh(planeGeometry, planeMaterial);
            plane.ReceiveShadow = true;

            plane.Rotation.X = (float)(-0.5 * System.Math.PI);
            plane.Position.X = 0;
            plane.Position.Y = 0;
            plane.Position.Z = 0;

            scene.Add(plane);

            var ambientLight = new AmbientLight(new Color().SetHex(0x555555));
            scene.Add(ambientLight);

            var spotLight = new SpotLight(new Color().SetHex(0xffffff), intensity: 1.2f, distance: 150, angle: (float)System.Math.PI / 4, penumbra: 0, decay: 2);
            spotLight.Shadow.MapSize.Height = 1024;
            spotLight.Shadow.MapSize.Width = 1024;
            spotLight.Position.Set(-40, 30, 30);
            spotLight.CastShadow = true;

            scene.Add(spotLight);

            AddGeometries();
        }

        private void AddGeometries()
        {
            List<Geometry> geoms = new List<Geometry>();

            geoms.Add(new CylinderGeometry(0, 4, 4));

            geoms.Add(new BoxGeometry(2, 2, 2));

            geoms.Add(new SphereGeometry(2, 20, 20));

            geoms.Add(new IcosahedronGeometry(4));

            var points = new List<Vector3>()
            {
                new Vector3(2, 2, 2),
                new Vector3(2, 2, -2),
                new Vector3(-2, 2, -2),
                new Vector3(-2, 2, 2),
                new Vector3(2, -2, 2),
                new Vector3(2, -2, -2),
                new Vector3(-2, -2, -2),
                new Vector3(-2, -2, 2)
            };

            geoms.Add(new ConvexGeometry(points.ToArray()));

            List<Vector3> pts = new List<Vector3>();
            float detail = 0.1f;
            float radius = 3;

            for (float angle = 0.0f; angle < System.Math.PI; angle += detail)
            {
                pts.Add(new Vector3((float)Math.Cos(angle) * radius, 0, (float)Math.Sin(angle) * radius));
            }

            geoms.Add(new LatheGeometry(pts.ToArray(), 12));

            geoms.Add(new OctahedronGeometry(3));

            geoms.Add(new ParametricGeometry(Mobius3d, 20, 10));

            geoms.Add(new TetrahedronGeometry(3));

            geoms.Add(new TorusGeometry(3, 1, 10, 10));

            geoms.Add(new TorusKnotGeometry(3, 0.5f, 50, 20));

            var j = 0;

            for (int i = 0; i < geoms.Count; i++)
            {
                var materials = new List<Material>(){
                    new MeshLambertMaterial() { Color = new Color().Random()},
                    new MeshBasicMaterial() { Color = new Color().SetHex(0x000000),Wireframe=true}
                };
                var mesh = SceneUtils.CreateMultiMaterialObject(geoms[i], materials);

                mesh.Traverse(o =>
                {
                    o.CastShadow = true;
                });
                mesh.Position.X = -24 + ((i % 4) * 12);
                mesh.Position.Y = 4;
                mesh.Position.Z = -8 + (j * 12);

                if ((i + 1) % 4 == 0) j++;

                scene.Add(mesh);

            }
        }
        public Vector3 Klein(float u, float v, Vector3 optionalTarget)
        {

            var result = optionalTarget != null ? optionalTarget : new Vector3();

            u *= (float)Math.PI;
            v *= 2 * (float)Math.PI;

            u = u * 2;

            float x, y, z;

            if (u < Math.PI)
            {

                x = (float)(3 * Math.Cos(u) * (1 + Math.Sin(u)) + (2 * (1 - Math.Cos(u) / 2)) * Math.Cos(u) * Math.Cos(v));
                z = (float)(-8 * Math.Sin(u) - 2 * (1 - Math.Cos(u) / 2) * Math.Sin(u) * Math.Cos(v));

            }
            else
            {

                x = (float)(3 * Math.Cos(u) * (1 + Math.Sin(u)) + (2 * (1 - Math.Cos(u) / 2)) * Math.Cos(v + Math.PI));
                z = -8 * (float)Math.Sin(u);

            }

            y = (float)(-2 * (1 - Math.Cos(u) / 2) * Math.Sin(v));

            return result.Set(x, y, z);

        }


        private Vector3 Mobius(float u, float t, Vector3 optionalTarget)
        {

            var result = optionalTarget != null ? optionalTarget : new Vector3();

            // flat mobius strip
            // http://www.wolframalpha.com/input/?i=M%C3%B6bius+strip+parametric+equations&lk=1&a=ClashPrefs_*Surface.MoebiusStrip.SurfaceProperty.ParametricEquations-
            u = u - 0.5f;
            var v = 2 * (float)Math.PI * t;

            float x, y, z;

            var a = 2;

            x = (float)(Math.Cos(v) * (a + u * Math.Cos(v / 2)));
            y = (float)(Math.Sin(v) * (a + u * Math.Cos(v / 2)));
            z = (float)(u * Math.Sin(v / 2));

            return result.Set(x, y, z);

        }

        private Vector3 Mobius3d(float u, float t, Vector3 optionalTarget)
        {

            var result = optionalTarget != null ? optionalTarget : new Vector3();

            // volumetric mobius strip

            u *= (float)Math.PI;
            t *= 2 * (float)Math.PI;

            u = u * 2;
            var phi = u / 2;
            float major = 2.25f, a = 0.125f, b = 0.65f;

            float x, y, z;

            x = (float)(a * Math.Cos(t) * Math.Cos(phi) - b * Math.Sin(t) * Math.Sin(phi));
            z = (float)(a * Math.Cos(t) * Math.Sin(phi) + b * Math.Sin(t) * Math.Cos(phi));
            y = (major + x) * (float)Math.Sin(u);
            x = (major + x) * (float)Math.Cos(u);

            return result.Set(x, y, z);

        }

    }
}
