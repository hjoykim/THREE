using System;
using System.Collections.Generic;
using System.Text;
using THREE;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Color = THREE.Color;
namespace THREEExample.ThreeJs.Interactive
{
    [Example("Interactive_Raycasting_Points", ExampleCategory.ThreeJs, "Interactive")]
    public class RaycastingPointsExample : Example
    {
        List<Object3D> pointclouds;
        Raycaster raycaster = new Raycaster();
        Vector2 mouse = new Vector2();
        Intersection intersection = null;
        List<Object3D> spheres = new List<Object3D>();
        int spheresIndex = 0;
        float threshold = 0.1f;
        float pointSize = 0.05f;
        int width = 80;
        int length = 160;
        float toggle = 0.0f;
        Matrix4 rotateY = new Matrix4().MakeRotationY(0.005f);
        public RaycastingPointsExample()
        {
        }

        private BufferGeometry GeneratePointCloudGeometry(Color color, int w, int l)
        {

            var geometry = new BufferGeometry();
            var numPoints = w * l;

            float[] positions = new float[numPoints * 3];
            float[] colors = new float[numPoints * 3];

            var k = 0;

            for (var i = 0; i < w; i++)
            {

                for (var j = 0; j < l; j++)
                {

                    var u = (float)i / w;
                    var v = (float)j / l;
                    var x = u - 0.5f;
                    var y = (Math.Cos(u * Math.PI * 4) + Math.Sin(v * Math.PI * 8)) / 20;
                    var z = v - 0.5f;

                    positions[3 * k] = x;
                    positions[3 * k + 1] = (float)y;
                    positions[3 * k + 2] = z;

                    var intensity = (float)(y + 0.1f) * 5;
                    colors[3 * k] = color.R * intensity;
                    colors[3 * k + 1] = color.G * intensity;
                    colors[3 * k + 2] = color.B * intensity;

                    k++;

                }

            }

            geometry.SetAttribute("position", new BufferAttribute<float>(positions, 3));
            geometry.SetAttribute("color", new BufferAttribute<float>(colors, 3));
            geometry.ComputeBoundingBox();
            return geometry;

        }
        private Points GeneratePointcloud(Color color, int w, int l)
        {

            var geometry = GeneratePointCloudGeometry(color, w, l);
            var material = new PointsMaterial { Size = pointSize, VertexColors = true };

            return new THREE.Points(geometry, material);

        }
        private Points GenerateIndexedPointcloud(Color color, int w, int l)
        {

            var geometry = GeneratePointCloudGeometry(color, w, l);
            var numPoints = w * l;
            int[] indices = new int[numPoints];

            var k = 0;

            for (var i = 0; i < width; i++)
            {

                for (var j = 0; j < length; j++)
                {

                    indices[k] = k;
                    k++;

                }

            }

            geometry.SetIndex(new BufferAttribute<int>(indices, 1));

            var material = new THREE.PointsMaterial { Size = pointSize, VertexColors = true };

            return new THREE.Points(geometry, material);

        }
        private Points GenerateIndexedWithOffsetPointcloud(Color color, int w, int l)
        {

            var geometry = GeneratePointCloudGeometry(color, w, l);
            var numPoints = w * l;
            int[] indices = new int[numPoints];

            var k = 0;

            for (var i = 0; i < width; i++)
            {

                for (var j = 0; j < length; j++)
                {

                    indices[k] = k;
                    k++;

                }

            }

            geometry.SetIndex(new BufferAttribute<int>(indices, 1));
            geometry.AddGroup(0, indices.Length);

            var material = new THREE.PointsMaterial { Size = pointSize, VertexColors = true };

            return new THREE.Points(geometry, material);

        }

        public override void InitCamera()
        {
            base.InitCamera();
            camera.Near = 1.0f;
            camera.Far = 10000.0f;
            camera.Position.Set(10, 10, 10);
            camera.LookAt(scene.Position);
            camera.UpdateMatrix();
        }

        public override void Init()
        {
            base.Init();

            this.renderer.SetClearColor(0x000000); // black

            var pcBuffer = GeneratePointcloud(new Color(1, 0, 0), width, length);
            pcBuffer.Scale.Set(5, 10, 10);
            pcBuffer.Position.Set(-5, 0, 0);
            scene.Add(pcBuffer);

            var pcIndexed = GenerateIndexedPointcloud(new Color(0, 1, 0), width, length);
            pcIndexed.Scale.Set(5, 10, 10);
            pcIndexed.Position.Set(0, 0, 0);
            scene.Add(pcIndexed);

            var pcIndexedOffset = GenerateIndexedWithOffsetPointcloud(new Color(0, 1, 1), width, length);
            pcIndexedOffset.Scale.Set(5, 10, 10);
            pcIndexedOffset.Position.Set(5, 0, 0);
            scene.Add(pcIndexedOffset);

            pointclouds = new List<Object3D> { pcBuffer, pcIndexed, pcIndexedOffset };

            //

            var sphereGeometry = new SphereBufferGeometry(0.1f, 32, 32);
            var sphereMaterial = new MeshBasicMaterial { Color = Color.Hex(0xff0000) };

            for (var i = 0; i < 40; i++)
            {
                var sphere = new Mesh(sphereGeometry, sphereMaterial);
                scene.Add(sphere);
                spheres.Add(sphere);
            }
            raycaster.parameters.Points["threshold"] = threshold;

            this.MouseMove += RaycastingPointsExample_MouseMove;
        }

        private void RaycastingPointsExample_MouseMove(object sender, THREE.MouseEventArgs e)
        {
            mouse.X = ( 1.0f * e.X / glControl.Width ) * 2 - 1;
            mouse.Y = - ( 1.0f * e.Y / glControl.Height ) * 2 + 1;
        }
        public override void Render()
        {
            camera.ApplyMatrix4(rotateY);
            camera.UpdateMatrixWorld();

            raycaster.SetFromCamera(mouse, camera);

            var intersections = raycaster.IntersectObjects(pointclouds);
            intersection = (intersections.Count) > 0 ? intersections[0] : null;

            if (toggle > 0.02f && intersection != null)
            {

                spheres[spheresIndex].Position.Copy(intersection.point);
                spheres[spheresIndex].Scale.Set(1, 1, 1);
                spheresIndex = (spheresIndex + 1) % spheres.Count;

                toggle = 0;

            }

            for (var i = 0; i < spheres.Count; i++)
            {

                var sphere = spheres[i];
                sphere.Scale.MultiplyScalar(0.98f);
                sphere.Scale.ClampScalar(0.01f, 1);

            }

            toggle += GetDelta();

            renderer.Render(scene, camera);
        }
    }
}
