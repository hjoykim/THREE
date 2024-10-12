using SkiaSharp;
using System;
using System.Collections.Generic;
using THREE;
using Color = THREE.Color;
namespace THREE.Silk.Example
{
    [Example("Terrain", ExampleCategory.ThreeJs, "Geometry")]
    public class GeometryTerrainExample : Example
    {
        public int worldWidth = 256;
        public int worldDepth = 256;
        public int worldHalfWidth = 0;
        public int worldHalfDepth = 0;
        public List<float> Data;
        public Mesh mesh;

        private FirstPersonControls firstPersonControl;
        public GeometryTerrainExample() : base()
        {
            worldHalfWidth = worldWidth / 2;
            worldHalfDepth = worldDepth / 2;

            scene.Background = THREE.Color.Hex(0xbfd1e5);

            Data = GenerateHeight(worldWidth, worldDepth);

            stopWatch.Start();
        }
        public override void InitCamera()
        {
            camera = new THREE.PerspectiveCamera(60, this.AspectRatio, 10, 20000);
            camera.Position.Y = Data[worldHalfWidth + worldHalfDepth * worldWidth] * 10 + 500;
        }
        public override void InitCameraController()
        {
            firstPersonControl = new FirstPersonControls(this,camera);
            firstPersonControl.MovementSpeed = 20;
            firstPersonControl.LookSpeed = 0.1f;
            firstPersonControl.LookVertical = true;
            firstPersonControl.ConstrainVertical = true;
            firstPersonControl.VerticalMin = 1.0f;
            firstPersonControl.VerticalMax = 2.0f;
            firstPersonControl.lon = -150;
            firstPersonControl.lat = 120;
        }
        public override void Init()
        {
            base.Init();

            BuildScene();
            
        }
        public virtual void BuildScene()
        {
            var geometry = new THREE.PlaneBufferGeometry(7500, 7500, worldWidth - 1, worldDepth - 1);
            geometry.RotateX(-(float)Math.PI / 2);

            var vertices = (geometry.Attributes["position"] as BufferAttribute<float>).Array;

            int j = 0;
            for (var i = 0; i < vertices.Length; i++)
            {
                if ((j + 1) < vertices.Length)
                {
                    vertices[j + 1] = Data[i] * 10;
                }
                j += 3;

            }
            geometry.SetAttribute("position", new BufferAttribute<float>(vertices, 3));
            geometry.ComputeFaceNormals(); // needed for helper

            var texture = GenerateTexture(Data, worldWidth, worldDepth);
            texture.NeedsUpdate = true;
            texture.WrapS = Constants.ClampToEdgeWrapping;
            texture.WrapT = Constants.ClampToEdgeWrapping;
            mesh = new Mesh(geometry, new MeshBasicMaterial() { Map = texture });
            scene.Add(mesh);
        }

        private List<float> GenerateHeight(int width, int height)
        {
            var size = width * height;
            float[] data = new float[size];
            var perlin = new ImprovedNoise();
            var quality = 1.0f;
            var z = MathUtils.NextFloat() * 100;

            for (var j = 0; j < 4; j++)
            {

                for (var i = 0; i < size; i++)
                {

                    var x = i % width;
                    var y = ~~(i / width);
                    data[i] += Math.Abs(perlin.Noise(x / quality, y / quality, z) * quality * 1.75f);

                }

                quality *= 5;

            }

            return new List <float>(data);
        }
        private Texture GenerateTexture(List<float> data, int width, int height)
        {

            SKBitmap bitmap ;
            SKBitmap canvasScaled ;

            var vector3 = new Vector3(0, 0, 0);
            var sun = new Vector3(1, 1, 1);
            sun.Normalize();




            byte[] imageData = new byte[width * height*4];

            int j = 0;

            for (var i = 0; i < imageData.Length; i += 4, j++)
            {
                if ((j - 2 < 0) || (j + 2 >= data.Count))
                {
                    imageData[i + 3] = 255;
                    continue;
                }
                else if ((j - width * 2 < 0) || ((j + width * 2) >= data.Count))
                {
                    imageData[i + 3] = 255;
                    continue;
                }
                else
                {
                    vector3.X = data[j - 2] - data[j + 2];

                    vector3.Y = 2;

                    vector3.Z = j + width * 2 >= data.Count ? 0 : j - width * 2 < 0 ? 0 : data[j - width * 2] - data[j + width * 2];

                    vector3.Normalize();

                    var shade = vector3.Dot(sun);
                    float r = (96 + shade * 128) * (0.5f + data[j] * 0.007f);
                    float g = (32 + shade * 96) * (0.5f + data[j] * 0.007f);
                    float b = (shade * 96) * (0.5f + data[j] * 0.007f);

                    if (r > 255 || g > 255 || b > 255) r = g = b = 0;
                    if (r < 0 || g < 0 || b < 0) r = g = b = 0;
                    imageData[i] = (byte)b;
                    imageData[i + 1] = (byte)g;
                    imageData[i + 2] = (byte)r;
                    imageData[i + 3] = 255;
                }

            }

            bitmap = imageData.ToSKBitMap(width,height);
            

            canvasScaled = bitmap.Resize(new SKImageInfo(height*4, width*4), SKFilterQuality.High);

            var imageData1 = canvasScaled.Bytes;
            for (var i = 0; i < imageData1.Length; i += 4)
            {

                var v = ~~(MathUtils.NextInt() * 5);

                imageData1[i] += (byte)v;
                imageData1[i + 1] += (byte)v;
                imageData1[i + 2] += (byte)v;

            }
            canvasScaled = imageData1.ToSKBitMap(width*4, height*4);
            Texture texture = new Texture();
            texture.Image = canvasScaled;

            return texture;
        }
        public override void Render()
        {
            var delta = stopWatch.ElapsedMilliseconds / 1000.0f;
            stopWatch.Reset();
            stopWatch.Start();
            firstPersonControl.Update(delta);
            renderer.Render(scene, camera);
        }
    }
}
