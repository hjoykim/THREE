using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace THREE
{
    public class LineSegments2 : Mesh
    {
        Vector3 _start = new Vector3();
        Vector3 _end = new Vector3();

        Vector4 _start4 = new Vector4();
        Vector4 _end4 = new Vector4();

        Vector4 _ssOrigin = new Vector4();
        Vector3 _ssOrigin3 = new Vector3();
        Matrix4 _mvMatrix = new Matrix4();
        Line3 _line = new Line3();
        Vector3 _closestPoint = new Vector3();

        Box3 _box = new Box3();
        Sphere _sphere = new Sphere();
        Vector4 _clipToWorldVector = new Vector4();

        public LineSegments2() : base()
        {
        }
        public LineSegments2(Geometry geometry = null, Material material = null) : base(geometry,material)
        {            
        }
       
        public LineSegments2 ComputeLineDistances()
        {

            var geometry = this.Geometry as BufferGeometry;

            var instanceStart = geometry.Attributes["instanceStart"] as InterleavedBufferAttribute<float>;
            var instanceEnd = geometry.Attributes["instanceEnd"] as InterleavedBufferAttribute<float>;
            var lineDistances = new float[2 * instanceStart.count];

            for (int i = 0, j = 0, l = instanceStart.count; i < l; i++, j += 2)
            {

                _start.FromBufferAttribute(instanceStart, i);
                _end.FromBufferAttribute(instanceEnd, i);

                lineDistances[j] = (j == 0) ? 0 : lineDistances[j - 1];
                lineDistances[j + 1] = lineDistances[j] + _start.DistanceTo(_end);

            }

            var instanceDistanceBuffer = new InstancedInterleavedBuffer<float>(lineDistances, 2, 1); // d0, d1

            geometry.SetAttribute("instanceDistanceStart", new InterleavedBufferAttribute<float>(instanceDistanceBuffer, 1, 0)); // d0
            geometry.SetAttribute("instanceDistanceEnd", new InterleavedBufferAttribute<float>(instanceDistanceBuffer, 1, 1)); // d1

            return this;
        }
        public override void Raycast(Raycaster raycaster, List<Intersection> intersects )
        {

            if (raycaster.camera == null)
            {

                Debug.WriteLine("LineSegments2: 'Raycaster.camera' needs to be set in order to raycast against LineSegments2.");
                System.Environment.Exit(-1);

            }

            float threshold = raycaster.parameters.Line2.ContainsKey("threshold") ? (float)raycaster.parameters.Line2["threshold"]  : 0;

            var ray = raycaster.ray;
            var camera = raycaster.camera;
            var projectionMatrix = camera.ProjectionMatrix;

            var matrixWorld = this.MatrixWorld;
            var geometry = this.Geometry as BufferGeometry;
            var material = this.Material;
            var resolution = (material as LineMaterial).Resolution;
            var lineWidth = (material as LineMaterial).LineWidth + threshold;

            var instanceStart = geometry.Attributes["instanceStart"] as InterleavedBufferAttribute<float>;
            var instanceEnd = geometry.Attributes["instanceEnd"] as InterleavedBufferAttribute<float>;

            // camera forward is negative
            var near = -camera.Near;

            // clip space is [ - 1, 1 ] so multiply by two to get the full
            // width in clip space
            var ssMaxWidth = 2.0f * Math.Max(lineWidth / resolution.Width, lineWidth / resolution.Height);

            //

            // check if we intersect the sphere bounds
            if (geometry.BoundingSphere == null)
            {

                geometry.ComputeBoundingSphere();

            }

            _sphere.Copy(geometry.BoundingSphere).ApplyMatrix4(matrixWorld);
            var distanceToSphere = Math.Max(camera.Near, _sphere.DistanceToPoint(ray.origin));

            // get the w component to scale the world space line width
            _clipToWorldVector.Set(0, 0, -distanceToSphere, 1.0f).ApplyMatrix4(camera.ProjectionMatrix);
            _clipToWorldVector.MultiplyScalar(1.0f / _clipToWorldVector.W);
            _clipToWorldVector.ApplyMatrix4(camera.ProjectionMatrixInverse);

            // increase the sphere bounds by the worst case line screen space width
            var sphereMargin = Math.Abs(ssMaxWidth / _clipToWorldVector.W) * 0.5f;
            _sphere.Radius += sphereMargin;

            if (raycaster.ray.IntersectsSphere(_sphere) == false)
            {
                return;
            }

            //

            // check if we intersect the box bounds
            if (geometry.BoundingBox == null)
            {

                geometry.ComputeBoundingBox();

            }

            _box.Copy(geometry.BoundingBox).ApplyMatrix4(matrixWorld);
            var distanceToBox = Math.Max(camera.Near, _box.DistanceToPoint(ray.origin));

            // get the w component to scale the world space line width
            _clipToWorldVector.Set(0, 0, -distanceToBox, 1.0f).ApplyMatrix4(camera.ProjectionMatrix);
            _clipToWorldVector.MultiplyScalar(1.0f / _clipToWorldVector.W);
            _clipToWorldVector.ApplyMatrix4(camera.ProjectionMatrixInverse);

            // increase the sphere bounds by the worst case line screen space width
            var boxMargin = Math.Abs(ssMaxWidth / _clipToWorldVector.W) * 0.5f;
            _box.Max.X += boxMargin;
            _box.Max.Y += boxMargin;
            _box.Max.Z += boxMargin;
            _box.Min.X -= boxMargin;
            _box.Min.Y -= boxMargin;
            _box.Min.Z -= boxMargin;

            if (raycaster.ray.IntersectsBox(_box) == false)
            {
                return;
            }

            //

            // pick a point 1 unit out along the ray to avoid the ray origin
            // sitting at the camera origin which will cause "w" to be 0 when
            // applying the projection matrix.
            ray.At(1, _ssOrigin);

            // ndc space [ - 1.0, 1.0 ]
            _ssOrigin.W = 1;
            _ssOrigin.ApplyMatrix4(camera.MatrixWorldInverse);
            _ssOrigin.ApplyMatrix4(projectionMatrix);
            _ssOrigin.MultiplyScalar(1 / _ssOrigin.W);

            // screen space
            _ssOrigin.X *= resolution.X / 2;
            _ssOrigin.Y *= resolution.Y / 2;
            _ssOrigin.Z = 0;

            _ssOrigin3.Set(_ssOrigin.X,_ssOrigin.Y,_ssOrigin.Z);

            _mvMatrix.MultiplyMatrices(camera.MatrixWorldInverse, matrixWorld);

            for (int i = 0, l = instanceStart.count; i < l; i++)
            {

                _start4.FromBufferAttribute(instanceStart, i);
                _end4.FromBufferAttribute(instanceEnd, i);

                _start4.W = 1;
                _end4.W = 1;

                // camera space
                _start4.ApplyMatrix4(_mvMatrix);
                _end4.ApplyMatrix4(_mvMatrix);

                // skip the segment if it's entirely behind the camera
                var isBehindCameraNear = _start4.Z > near && _end4.Z > near;
                if (isBehindCameraNear)
                {

                    continue;

                }

                // trim the segment if it extends behind camera near
                if (_start4.Z > near)
                {

                    var deltaDist = _start4.Z - _end4.Z;
                    var t = (_start4.Z - near) / deltaDist;
                    _start4.Lerp(_end4, t);

                }
                else if (_end4.Z > near)
                {

                    var deltaDist = _end4.Z - _start4.Z;
                    var t = (_end4.Z - near) / deltaDist;
                    _end4.Lerp(_start4, t);

                }

                // clip space
                _start4.ApplyMatrix4(projectionMatrix);
                _end4.ApplyMatrix4(projectionMatrix);

                // ndc space [ - 1.0, 1.0 ]
                _start4.MultiplyScalar(1 / _start4.W);
                _end4.MultiplyScalar(1 / _end4.W);

                // screen space
                _start4.X *= resolution.X / 2;
                _start4.Y *= resolution.Y / 2;

                _end4.X *= resolution.X / 2;
                _end4.Y *= resolution.Y / 2;

                // create 2d segment
                _line.Start.Set(_start4.X,_start.Y,_start.Z);
                _line.Start.Z = 0;

                _line.End.Set(_end4.X, _end4.Y, _end4.Z);
                _line.End.Z = 0;

                // get closest point on ray to segment
                var param = _line.ClosestPointToPointParameter(_ssOrigin3, true);
                _line.At(param, _closestPoint);

                // check if the intersection point is within clip space
                var zPos = MathUtils.Lerp(_start4.Z, _end4.Z, param);
                var isInClipSpace = zPos >= -1 && zPos <= 1;

                var isInside = _ssOrigin3.DistanceTo(_closestPoint) < lineWidth * 0.5;

                if (isInClipSpace && isInside)
                {

                    _line.Start.FromBufferAttribute(instanceStart, i);
                    _line.End.FromBufferAttribute(instanceEnd, i);

                    _line.Start.ApplyMatrix4(matrixWorld);
                    _line.End.ApplyMatrix4(matrixWorld);

                    var pointOnLine = new Vector3();
                    var point = new Vector3();

                    ray.DistanceSqToSegment(_line.Start, _line.End, point, pointOnLine);

                    intersects.Add(new Intersection {

                    point = point,
					pointOnLine = pointOnLine,
					distance = ray.origin.DistanceTo(point),
					object3D = this,
					face = null,
					faceIndex = i,
					uv = null,
					uv2 = null,

				} );

        }

    }

}
    }
}
