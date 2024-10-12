using System;
using System.Collections.Generic;
using THREE;
using THREE.Silk;
using Color = THREE.Color;
namespace THREE.Silk.Example.Learning.Chapter06
{
    [Example("02.Advanced-3D-Geometries-Lathe", ExampleCategory.LearnThreeJS, "Chapter06")]
    public class AdvancedGeometriesLathe : AdvancedGeometriesConvex
    {
        public AdvancedGeometriesLathe() : base()
        {

        }
        public override BufferGeometry BuildGeometry()
        {
            if (spGroup != null) scene.Remove(spGroup);

            float height = 5;
            int count = 30;

            var points = new List<Vector3>();

            for (int i = 0; i < count; i++)
            {

                float X = (float)System.Math.Sin(i * 0.2f) + (float)System.Math.Cos(i * 0.3f) * height + 12;
                float Y = (i - count) + count / 2.0f;
                float Z = 0;

                points.Add(new Vector3(X, Y, Z));
            }

            var latheGeometry = new LatheBufferGeometry(points.ToArray(), 12, 0, 2 * (float)Math.PI);
            latheGeometry.ComputeVertexNormals();
            latheGeometry.ComputeFaceNormals();
            latheGeometry.NormalsNeedUpdate = true;

            spGroup = new Object3D();

            var material = new MeshBasicMaterial() { Color = Color.Hex(0xff0000), Transparent = false };

            points.ForEach(delegate (Vector3 point)
            {
                var spGeom = new SphereBufferGeometry(0.2f);
                var spMesh = new Mesh(spGeom, material);
                spMesh.Position.Copy(point);
                spGroup.Add(spMesh);
            }
            );
            scene.Add(spGroup);
            return latheGeometry;
        }
    }
}
