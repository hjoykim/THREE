using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;
using THREE.Math;

namespace THREE.Objects
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
        
        protected Sprite(Sprite source) : base()
        {
            if (source.Center != null) this.Center.Copy(source.Center);

        }
        public new Sprite Clone()
        {
            return new Sprite(this);
        }
        //public void Raycast(Raycaster raycaster, Intersection[] intersects)
        //{
        //}

    }
}
