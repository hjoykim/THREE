using System.Collections.Generic;


namespace THREE
{
    public class Points : Object3D
    {
        public Points(Geometry geometry=null, Material material=null)
        {
            this.type = "Points";

            this.Geometry = geometry ?? new BufferGeometry();
            this.Material = material ?? new PointsMaterial { Color = new Color().SetHex(0xFFFFFF) };
            
            this.UpdateMorphTargets();
        }

        public Points(Geometry geometry=null, List<Material> materials=null)
        {
            this.type = "Points";

            this.Geometry = geometry ?? new BufferGeometry();

            if (materials == null)
            {
                this.Material = new PointsMaterial { Color = new Color().SetHex(0xFFFFFF) };
                this.Materials.Add(Material);
            }
            else
            {
                this.Materials = materials;
                if (this.Materials.Count > 0)
                    this.Material = this.Materials[0];
            }
        }

        public void UpdateMorphTargets()
        {

        }

        private void testPoint(Vector3 point, int index, float localThresholdSq, Ray ray, Raycaster raycaster, Vector3 intersectPoint, List<Intersection> intersectionList)
        {
            float rayPointDistanceSq = ray.DistanceSqToPoint(point);
            if (rayPointDistanceSq < localThresholdSq)
            {
                ray.ClosestPointToPoint(point, intersectPoint);
                intersectPoint.ApplyMatrix4(MatrixWorld);
                float distance = raycaster.ray.origin.DistanceTo(intersectPoint);
                if (distance < raycaster.near || distance > raycaster.far)
                {
                    return;
                }
                Intersection item = new Intersection();
                item.distance = distance;
                item.distanceToRay = (float)System.Math.Sqrt(rayPointDistanceSq);
                item.point = intersectPoint.Clone();
                item.index = index;
                item.face = null;
                item.object3D = this;
                intersectionList.Add(item);
            }
        }

        public override void Raycast(Raycaster raycaster, List<Intersection> intersectionList)
        {
            Matrix4 inverseMatrix = new Matrix4().GetInverse(MatrixWorld);
            Ray ray = new Ray();
            Sphere sphere = new Sphere();
            //float threshold = raycaster.parameters.Points.Threshold;
            float threshold = 0.1f; // Not sure what the replacement for Threshold is
            // Checking boundingSphere distance to ray
            if (Geometry.BoundingSphere == null)
            {
                Geometry.ComputeBoundingSphere();
            }
            sphere.Copy(Geometry.BoundingSphere);
            sphere.ApplyMatrix4(MatrixWorld);
            sphere.Radius += threshold;
            if (!raycaster.ray.IntersectsSphere(sphere))
            {
                return;
            }
            ray.copy(raycaster.ray).ApplyMatrix4(inverseMatrix);
            
            float localThreshold = threshold / ((float)(Scale.X + Scale.Y + Scale.Z) / 3);
            Vector3 position = new Vector3();
            Vector3 intersectPoint = new Vector3();

            BufferAttribute<int> index = (Geometry as BufferGeometry).Index;
            //float[] positions = Geometry.position.arrayFloat;
            float[] positions = ((BufferAttribute<float>)(Geometry as BufferGeometry).Attributes["position"]).Array; // Not sure if this is Correct
            if (index != null)
            {
                int[] indices = index.Array;
                for (int i = 0; i < indices.Length; i++)
                {
                    int a = indices[i];
                    position.FromArray(positions, a * 3);
                    testPoint(position, a, localThreshold, ray, raycaster, intersectPoint, intersectionList);
                }
            }
            else
            {
                for (int i = 0; i < positions.Length / 3; i++)
                {
                    position.FromArray(positions, i * 3);
                    testPoint(position, i, localThreshold, ray, raycaster, intersectPoint, intersectionList);
                }
            }
        }
    }
}
