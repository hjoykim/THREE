using System.Collections.Generic;

namespace THREE
{
  
    public class RenderableObject
    {
        public int id = 0;
        public Object3D object3D = null;
        public float z = 0;
        public int renderOrder=0;
    }
    public class RenderableVertex : RenderableObject
    {
        public Vector3 position = Vector3.Zero();
        public Vector3 positionWorld = Vector3.Zero();
        public Vector4 positionScreen = Vector4.Zero().Set(0,0,0,1);
        public bool visible = true;

        public RenderableVertex() : base()
        {

        }
        public void Copy(RenderableVertex vertex)
        {
            this.positionWorld.Copy(vertex.positionWorld);
            this.positionScreen.Copy(vertex.positionScreen);
        }
    }
    public class RenderableFace : RenderableObject
    {
        public RenderableVertex v1 = new RenderableVertex();
        public RenderableVertex v2 = new RenderableVertex();
        public RenderableVertex v3 = new RenderableVertex();
        public Vector3 normalModel = new Vector3() ;
        public List<Vector3> vertexNormalsModel =  new List<Vector3> { Vector3.Zero(), Vector3.Zero(), Vector3.Zero() };
        public int vertexNormalsLength = 0;
        public Color color = new Color();
        public Material material = null;
        public List<Vector2> uvs = new List<Vector2> { Vector2.Zero(), Vector2.Zero(), Vector2.Zero() };
        
        public RenderableFace() : base()
        {

        }
    }

    public class RenderableLine : RenderableObject
    {
        public RenderableVertex v1 = new RenderableVertex();
        public RenderableVertex v2 = new RenderableVertex();
        public List<Color> vertexColors = new List<Color> { Color.Hex(0x000000), Color.Hex(0x000000) };
        public Material material = null;

        public RenderableLine() : base()
        {

        }
    }
    public class RenderableSprite : RenderableObject
    {
        public float x = 0;
        public float y = 0;
        public float rotation = 0;
        public Vector2 scale = Vector2.Zero();
        public Material material = null;

        public RenderableSprite() : base()
        {

        }
    }
    public class RenderList
    {
        public List<float> normals = new List<float>();
        public List<float> colors = new List<float>();
        public List<float> uvs = new List<float>();

        public Object3D object3D = null;

        public Matrix3 normalMatrix = new Matrix3();

        private Projector projector;
        public RenderList(Projector projector)
        {
            this.projector = projector;
        }
        public void SetObject(Object3D value)
        {
            object3D = value;

            normalMatrix.GetNormalMatrix(object3D.MatrixWorld);
        }

        public void ProjectVertex(RenderableVertex vertex)
        {

            var position = vertex.position;
            var positionWorld = vertex.positionWorld;
            var positionScreen = vertex.positionScreen;

            positionWorld.Copy(position).ApplyMatrix4(projector._modelMatrix);
            positionScreen.Set(positionWorld.X,positionWorld.Y,positionWorld.Z,1).ApplyMatrix4(projector._viewProjectionMatrix);

            var invW = 1 / positionScreen.W;

            positionScreen.X *= invW;
            positionScreen.Y *= invW;
            positionScreen.Z *= invW;

            vertex.visible = positionScreen.X >= -1 && positionScreen.X <= 1 &&
                     positionScreen.Y >= -1 && positionScreen.Y <= 1 &&
                     positionScreen.Z >= -1 && positionScreen.Z <= 1;

        }

        public void PushVertex(float x, float y, float z )
        {

            projector._vertex = projector.GetNextVertexInPool();
            projector._vertex.position.Set(x, y, z);

            ProjectVertex(projector._vertex);

        }

        public void PushNormal(float x, float y, float z )
        {

            normals.Add(x, y, z);

        }

        public void PushColor(float r, float g, float b )
        {

            colors.Add(r, g, b);

        }

        public void PushUv(float x, float y )
        {

            uvs.Add(x, y);

        }

        public bool  CheckTriangleVisibility(RenderableVertex v1, RenderableVertex v2, RenderableVertex v3 )
        {

            if (v1.visible == true || v2.visible == true || v3.visible == true) return true;

            projector._points3[0] = v1.positionScreen;
            projector._points3[1] = v2.positionScreen;
            projector._points3[2] = v3.positionScreen;

            return projector._clipBox.IntersectsBox(projector._boundingBox.SetFromPoints(projector._points3));

        }

        public bool CheckBackfaceCulling(RenderableVertex v1, RenderableVertex v2, RenderableVertex v3)
        {

            return ((v3.positionScreen.X - v1.positionScreen.X) *
                    (v2.positionScreen.Y - v1.positionScreen.Y) -
                    (v3.positionScreen.Y - v1.positionScreen.Y) *
                    (v2.positionScreen.X - v1.positionScreen.X)) < 0;

        }

        public void PushLine(int a, int b )
        {

            var v1 = projector._vertexPool[a];
            var v2 = projector._vertexPool[b];

            // Clip

            v1.positionScreen.Set(v1.position.X,v1.position.Y,v1.position.Z,1).ApplyMatrix4(projector._modelViewProjectionMatrix);
            v2.positionScreen.Set(v2.position.X,v2.position.Y,v2.position.Z,1).ApplyMatrix4(projector._modelViewProjectionMatrix);

            if (projector.ClipLine(v1.positionScreen, v2.positionScreen) == true)
            {

                // Perform the perspective divide
                v1.positionScreen.MultiplyScalar(1 / v1.positionScreen.W);
                v2.positionScreen.MultiplyScalar(1 / v2.positionScreen.W);

                projector._line = projector.GetNextLineInPool();
                projector._line.id = object3D.Id;
                projector._line.v1.Copy(v1);
                projector._line.v2.Copy(v2);
                projector._line.z = System.Math.Max(v1.positionScreen.Z, v2.positionScreen.Z);
                projector._line.renderOrder = object3D.RenderOrder;

                projector._line.material = object3D.Material;

                if (object3D.Material.VertexColors)
                {

                    projector._line.vertexColors[0].FromArray(colors.ToArray(), a * 3);
                    projector._line.vertexColors[1].FromArray(colors.ToArray(), b * 3);

                }

                projector._renderData.elements.Add(projector._line);

            }

        }

        public void PushTriangle(int a, int b, int c, Material material, List<int> arguments )
        {

            var v1 = projector._vertexPool[a];
            var v2 = projector._vertexPool[b];
            var v3 = projector._vertexPool[c];

            if (CheckTriangleVisibility(v1, v2, v3) == false) return;

            if (material.Side == Constants.DoubleSide || CheckBackfaceCulling(v1, v2, v3) == true)
            {

                projector._face = projector.GetNextFaceInPool();

                projector._face.id = object3D.Id;
                projector._face.v1.Copy(v1);
                projector._face.v2.Copy(v2);
                projector._face.v3.Copy(v3);
                projector._face.z = (v1.positionScreen.Z + v2.positionScreen.Z + v3.positionScreen.Z) / 3;
                projector._face.renderOrder = object3D.RenderOrder;

                // face normal
                projector._vector3.SubVectors(v3.position, v2.position);
                Vector3 v = new Vector3();
                v.SubVectors(v1.position, v2.position);
                projector._vector4.Set(v.X,v.Y,v.Z,1);
                projector._vector3.Cross(v);

                projector._face.normalModel.Copy(projector._vector3);
                projector._face.normalModel.ApplyMatrix3(normalMatrix).Normalize();

                for (var i = 0; i < 3; i++)
                {

                    var normal = projector._face.vertexNormalsModel[i];
                    normal.FromArray(normals.ToArray(), arguments[i] * 3);

                    normal.ApplyMatrix3(normalMatrix).Normalize();

                    var uv = projector._face.uvs[i];
                    uv.FromArray(uvs.ToArray(), arguments[i] * 2);

                }

                projector._face.vertexNormalsLength = 3;

                projector._face.material = material;

                if (material.VertexColors)
                {

                    projector._face.color.FromArray(colors.ToArray(), a * 3);

                }

                projector._renderData.elements.Add(projector._face);

            }

        }
    }
    public class Projector
    {
        public RenderableObject _object;
        public int _objectCount;
        public List<RenderableObject> _objectPool = new List<RenderableObject>();
        public int _objectPoolLength = 0;
        public RenderableVertex _vertex;
        public int _vertexCount;
        public List<RenderableVertex> _vertexPool;
        public int _vertexPoolLength = 0;
        public RenderableFace _face;
        public int _faceCount;
        public List<RenderableFace> _facePool;
        public int _facePoolLength = 0;
        public RenderableLine _line;
        public int _lineCount;
        public List<RenderableLine> _linePool;
        public int _linePoolLength = 0;
        public RenderableSprite _sprite;
        public int _spriteCount;
        public List<RenderableSprite> _spritePool;
        public int _spritePoolLength = 0;
        public Vector3 _vector3 = new Vector3();
        public Vector4 _vector4 = new Vector4();

        public Box3 _clipBox = new Box3(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
        public Box3 _boundingBox = new Box3();
        public List<Vector4> _points3 = new List<Vector4>();

        public Matrix4 _viewMatrix = new Matrix4();
        public Matrix4 _viewProjectionMatrix = new Matrix4();

        public Matrix4 _modelMatrix = new Matrix4();
        public Matrix4 _modelViewProjectionMatrix = new Matrix4();

        public Matrix3 _normalMatrix = new Matrix3();

        public Frustum _frustum = new Frustum();

        public Vector4 _clippedVertex1PositionScreen = new Vector4();
        public Vector4 _clippedVertex2PositionScreen = new Vector4();

        

        public struct RenderData 
        {
            public List<RenderableObject> objects;
            public List<Light> lights;
            public List<RenderableObject> elements;

        }

        public RenderData _renderData;

        public RenderList renderList;
        public Projector()
        {
            renderList = new RenderList(this);
        }

        public void ProjectVector(Vector3 vector,Camera camera)
        {
            vector.Project(camera);
        }

        public void UnprojectVector(Vector3 vector, Camera camera)
        {
            vector.UnProject(camera);
        }

        public void ProjectObject(Object3D object3D )
        {

            if (object3D.Visible == false) return;

            if (object3D is Light ) {

                _renderData.lights.Add(object3D as Light);

            } else if (object3D is Mesh || object3D is Line || object3D is Points ) {

                if (object3D.Material.Visible == false) return;
                if (object3D.FrustumCulled == true && _frustum.IntersectsObject(object3D) == false) return;
                
                AddObject(object3D);

            } else if (object3D is Sprite ) {

                if (object3D.Material.Visible == false) return;
                if (object3D.FrustumCulled == true && _frustum.IntersectsSprite(object3D as Sprite) == false) return;

                AddObject(object3D);

            }

            var children = object3D.Children;

            for (var i = 0;i< children.Count; i++)
            {
                ProjectObject(children[i]);
            }
        }

        public void AddObject(Object3D object3D )
        {

            _object = GetNextObjectInPool();
            _object.id = object3D.Id;
            _object.object3D = object3D;

            _vector3.SetFromMatrixPosition(object3D.MatrixWorld);
            _vector3.ApplyMatrix4(_viewProjectionMatrix);
            _object.z = _vector3.Z;
            _object.renderOrder = object3D.RenderOrder;

            _renderData.objects.Add(_object);

        }
        public RenderableObject GetNextObjectInPool()
        {

            if (_objectCount == _objectPoolLength)
            {

                var object3D = new RenderableObject();
                _objectPool.Add(object3D);
                _objectPoolLength++;
                _objectCount++;
                return object3D;

            }

            return _objectPool[_objectCount++];

        }
        public RenderableVertex GetNextVertexInPool()
        {

            if (_vertexCount == _vertexPoolLength)
            {

                var vertex = new RenderableVertex();
                _vertexPool.Add(vertex);
                _vertexPoolLength++;
                _vertexCount++;
                return vertex;

            }

            return _vertexPool[_vertexCount++];

        }
        public RenderableFace GetNextFaceInPool()
        {

            if (_faceCount == _facePoolLength)
            {

                var face = new RenderableFace();
                _facePool.Add(face);
                _facePoolLength++;
                _faceCount++;
                return face;

            }

            return _facePool[_faceCount++];

        }
        public RenderableLine GetNextLineInPool()
        {

            if (_lineCount == _linePoolLength)
            {

                var line = new RenderableLine();
                _linePool.Add(line);
                _linePoolLength++;
                _lineCount++;
                return line;

            }

            return _linePool[_lineCount++];

        }
        public RenderableSprite GetNextSpriteInPool()
        {

            if (_spriteCount == _spritePoolLength)
            {

                var sprite = new RenderableSprite();
                _spritePool.Add(sprite);
                _spritePoolLength++;
                _spriteCount++;
                return sprite;
            }

            return _spritePool[_spriteCount++];

        }
        public bool ClipLine(Vector4 s1, Vector4 s2 )
        {

            float alpha1 = 0, alpha2 = 1;

                // Calculate the boundary coordinate of each vertex for the near and far clip planes,
                // Z = -1 and Z = +1, respectively.

            float bc1near = s1.Z + s1.W,
                bc2near = s2.Z + s2.W,
                bc1far = -s1.Z + s1.W,
                bc2far = -s2.Z + s2.W;

            if (bc1near >= 0 && bc2near >= 0 && bc1far >= 0 && bc2far >= 0)
            {

                // Both vertices lie entirely within all clip planes.
                return true;

            }
            else if ((bc1near < 0 && bc2near < 0) || (bc1far < 0 && bc2far < 0))
            {

                // Both vertices lie entirely outside one of the clip planes.
                return false;

            }
            else
            {

                // The line segment spans at least one clip plane.

                if (bc1near < 0)
                {

                    // v1 lies outside the near plane, v2 inside
                    alpha1 = System.Math.Max(alpha1, bc1near / (bc1near - bc2near));

                }
                else if (bc2near < 0)
                {

                    // v2 lies outside the near plane, v1 inside
                    alpha2 = System.Math.Min(alpha2, bc1near / (bc1near - bc2near));

                }

                if (bc1far < 0)
                {

                    // v1 lies outside the far plane, v2 inside
                    alpha1 = System.Math.Max(alpha1, bc1far / (bc1far - bc2far));

                }
                else if (bc2far < 0)
                {

                    // v2 lies outside the far plane, v2 inside
                    alpha2 = System.Math.Min(alpha2, bc1far / (bc1far - bc2far));

                }

                if (alpha2 < alpha1)
                {

                    // The line segment spans two boundaries, but is outside both of them.
                    // (This can't happen when we're only clipping against just near/far but good
                    //  to leave the check here for future usage if other clip planes are added.)
                    return false;

                }
                else
                {

                    // Update the s1 and s2 vertices to match the clipped line segment.
                    s1.Lerp(s2, alpha1);
                    s2.Lerp(s1, 1 - alpha2);

                    return true;

                }

            }

        }

    }
}
