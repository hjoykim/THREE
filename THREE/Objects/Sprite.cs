using System.Collections.Generic;

namespace THREE
{
    public class Sprite : Object3D
    {
        Geometry _geometry;

        Vector3  _intersectPoint = new Vector3();
        Vector3 _worldScale = new Vector3();
        Vector3 _mvPosition = new Vector3();

        Vector2 _alignedPosition = new Vector2();
        Vector2 _rotatedPosition = new Vector2();
        Matrix4 _viewWorldMatrix = new Matrix4();

        Vector3 _vA = new Vector3();
        Vector3 _vB = new Vector3();
        Vector3 _vC = new Vector3();

        Vector2 _uvA = new Vector2();
        Vector2 _uvB = new Vector2();
        Vector2 _uvC = new Vector2();

        public Vector2 Center;

        public Sprite(Material material)
        {
            type = "Sprite";

            if (_geometry == null)
            {
                _geometry = new BufferGeometry();

                float[] float32Array = new float[]{
                    -0.5f, -0.5f, 0, 0, 0,
                    0.5f, -0.5f, 0, 1, 0,
                    0.5f, 0.5f, 0, 1, 1,
                    -0.5f, 0.5f, 0, 0, 1
                };

                var interleavedBuffer = new InterleavedBuffer<float>(float32Array, 5);

                (_geometry as BufferGeometry).SetIndex( new List<int>(){ 0, 1, 2, 0, 2, 3} );
                (_geometry as BufferGeometry).SetAttribute("position", new InterleavedBufferAttribute<float>(interleavedBuffer, 3, 0, false));
                (_geometry as BufferGeometry).SetAttribute("uv", new InterleavedBufferAttribute<float>(interleavedBuffer, 2, 3, false));
            }

            this.Geometry = _geometry;
            this.Material = material != null ? material : new SpriteMaterial();

            this.Center = new Vector2(0.5f, 0.5f);
        }

        public override void Raycast(Raycaster raycaster, List<Intersection> intersectionList)
        {
            Vector3 intersectPoint = new Vector3();
            Vector3 worldScale = new Vector3();
            Vector3 mvPosition = new Vector3();

            Vector2 alignedPosition = new Vector2();
            Vector2 rotatedPosition = new Vector2();
            Matrix4 viewWorldMatrix = new Matrix4();

            Vector3 vA = new Vector3();
            Vector3 vB = new Vector3();
            Vector3 vC = new Vector3();

            Vector2 uvA = new Vector2();
            Vector2 uvB = new Vector2();
            Vector2 uvC = new Vector2();

            worldScale.SetFromMatrixScale(MatrixWorld);

            viewWorldMatrix.Copy(raycaster.camera.MatrixWorld);
            this.ModelViewMatrix.MultiplyMatrices(raycaster.camera.MatrixWorldInverse, MatrixWorld);

            mvPosition.SetFromMatrixPosition(ModelViewMatrix);

            if ((raycaster.camera is PerspectiveCamera) && !Materials[0].SizeAttenuation)
            {
                worldScale.MultiplyScalar(-mvPosition.Z);
            }

            float rotation = Material.Rotation;
            float sin = 0, cos = 0;
            if (rotation != 0)
            {
                cos = (float)System.Math.Cos(rotation);
                sin = (float)System.Math.Sin(rotation);

            }

            Vector2 scale = worldScale.ToVector2();
            TransformVertex(vA.Set(-0.5f, -0.5f, 0), mvPosition, Center, scale, sin, cos,
                    alignedPosition, rotatedPosition, viewWorldMatrix);
            TransformVertex(vB.Set(0.5f, -0.5f, 0), mvPosition, Center, scale, sin, cos,
                    alignedPosition, rotatedPosition, viewWorldMatrix);
            TransformVertex(vC.Set(0.5f, 0.5f, 0), mvPosition, Center, scale, sin, cos,
                    alignedPosition, rotatedPosition, viewWorldMatrix);

            uvA.Set(0, 0);
            uvB.Set(1, 0);
            uvC.Set(1, 1);

            // check first triangle
            Vector3 intersect = raycaster.ray.IntersectTriangle(vA, vB, vC, false, intersectPoint);

            if (intersect == null)
            {

                // check second triangle
                TransformVertex(vB.Set(-0.5f, 0.5f, 0), mvPosition, Center, scale, sin, cos,
                        alignedPosition, rotatedPosition, viewWorldMatrix);
                uvB.Set(0, 1);

                intersect = raycaster.ray.IntersectTriangle(vA, vC, vB, false, intersectPoint);
                if (intersect == null)
                {
                    return;
                }

            }

            float distance = raycaster.ray.origin.DistanceTo(intersectPoint);

            if (distance < raycaster.near || distance > raycaster.far) return;

            Intersection item = new Intersection();
            item.distance = distance;
            item.point = intersectPoint.Clone();
            item.uv = Triangle.GetUV(intersectPoint, vA, vB, vC, uvA, uvB, uvC, new Vector2());
            item.face = null;
            item.object3D = this;
            intersectionList.Add(item);
        }

        private void TransformVertex(Vector3 vertexPosition, Vector3 mvPosition, Vector2 center, Vector2 scale, float sin,
                                 float cos, Vector2 alignedPosition, Vector2 rotatedPosition, Matrix4 viewWorldMatrix)
        {
            // compute position in camera space
            Vector2 vPos = vertexPosition.ToVector2();
            alignedPosition.SubVectors(vPos, center).AddScalar(0.5f).Multiply(scale);

            // to check if rotation is not zero
            if (sin != 0)
            {
                rotatedPosition.X = (cos * alignedPosition.X) - (sin * alignedPosition.Y);
                rotatedPosition.Y = (sin * alignedPosition.X) + (cos * alignedPosition.Y);

            }
            else
            {
                rotatedPosition.Copy(alignedPosition);
            }


            vertexPosition.Copy(mvPosition);
            vertexPosition.X += rotatedPosition.X;
            vertexPosition.Y += rotatedPosition.Y;

            // transform to world space
            vertexPosition.ApplyMatrix4(viewWorldMatrix);
        }

        protected Sprite(Sprite source) : base()
        {
            if (source.Center != null) this.Center.Copy(source.Center);

        }

        public new Sprite Clone()
        {
            return new Sprite(this);
        }
    }
}
