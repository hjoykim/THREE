using System;

namespace THREE
{
    public class Frustum : ICloneable
    {
        public Plane[] Planes = new Plane[6] {new Plane(),new Plane(),new Plane(),new Plane(),new Plane(),new Plane()} ;

        private Sphere _sphere = new Sphere();

        private Vector3 _vector = new Vector3();

        public Frustum()
        {
        }

        public Frustum(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4, Plane p5)
        {
            this.Planes[0] = p0 != null ? p0 : new Plane();
            this.Planes[1] = p1 != null ? p1 : new Plane();
            this.Planes[2] = p2 != null ? p2 : new Plane();
            this.Planes[3] = p3 != null ? p3 : new Plane();
            this.Planes[4] = p4 != null ? p4 : new Plane();
            this.Planes[5] = p5 != null ? p5 : new Plane();
        }

        protected Frustum(Frustum other)
        {
            var planes = this.Planes;
            for (int i = 0; i < 6; i++)
            {
                planes[i] = (Plane)other.Planes[i].Clone();
            }
        }

        public Frustum Set(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4, Plane p5)
        {
            var planes = this.Planes;

            planes[0] = (Plane)p0.Clone();
            planes[1] = (Plane)p1.Clone();
            planes[2] = (Plane)p2.Clone();
            planes[3] = (Plane)p3.Clone();
            planes[4] = (Plane)p4.Clone();
            planes[5] = (Plane)p5.Clone();

            return this;
        }

        public object Clone()
        {
            return new Frustum(this);
        }

        public Frustum SetFromProjectionMatrix(Matrix4 m)
        {
            var planes = this.Planes;
            var me = m.Elements;
            float me0 = me[0], me1 = me[1], me2 = me[2], me3 = me[3],
                  me4 = me[4], me5 = me[5], me6 = me[6], me7 = me[7],
                  me8 = me[8], me9 = me[9], me10 = me[10], me11 = me[11],
                  me12 = me[12], me13 = me[13], me14 = me[14], me15 = me[15];

            planes[0].SetComponents(me3 - me0, me7 - me4, me11 - me8, me15 - me12).Normalize();
            planes[1].SetComponents(me3 + me0, me7 + me4, me11 + me8, me15 + me12).Normalize();
            planes[2].SetComponents(me3 + me1, me7 + me5, me11 + me9, me15 + me13).Normalize();
            planes[3].SetComponents(me3 - me1, me7 - me5, me11 - me9, me15 - me13).Normalize();
            planes[4].SetComponents(me3 - me2, me7 - me6, me11 - me10, me15 - me14).Normalize();
            planes[5].SetComponents(me3 + me2, me7 + me6, me11 + me10, me15 + me14).Normalize();

            return this;
        }

        public bool IntersectsObject(Object3D object3D)
        {
            var geometry = object3D.Geometry;

            if (geometry.BoundingSphere == null) geometry.ComputeBoundingSphere();

            _sphere.Copy(geometry.BoundingSphere).ApplyMatrix4(object3D.MatrixWorld); ;
           

            return this.IntersectsSphere(_sphere);
        }

        public bool IntersectsSprite(Sprite sprite)
        {
            _sphere.Center.Set(0, 0, 0);
            _sphere.Radius = 0.7071067811865476f;
            _sphere.ApplyMatrix4(sprite.MatrixWorld);

            return this.IntersectsSphere(_sphere);
        }
        public bool IntersectsSphere(Sphere sphere)
        {
            Plane[] planes = this.Planes;
            Vector3 center = sphere.Center;
            var negRadius = -sphere.Radius;

            for (int i = 0; i < 6; i++)
            {
                float distance = planes[i].DistanceToPoint(center);

                if (distance < negRadius)
                {
                    return false;
                }

            }
            return true;
        }
        public bool IntersectsBox(Box3 box)
        {
            var planes = this.Planes;

            for (var i = 0; i < 6; i++)
            {

                var plane = planes[i];

                // corner at max distance

                _vector.X = plane.Normal.X > 0 ? box.Max.X : box.Min.X;
                _vector.Y = plane.Normal.Y > 0 ? box.Max.Y : box.Min.Y;
                _vector.Z = plane.Normal.Z > 0 ? box.Max.Z : box.Min.Z;

                if (plane.DistanceToPoint(_vector) < 0)
                {
                    return false;
                }
            }

            return true;
        }
        public bool ContainsPoint(Vector3 point)
        {
            var planes = this.Planes;

            for (var i = 0; i < 6; i++)
            {

                if (planes[i].DistanceToPoint(point) < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

}
