using FastDeepCloner;
using SkiaSharp;
using System;
using System.Drawing;
using System.Reflection.Emit;

namespace THREE
{
    public class VolumeSlice
    {
        private int _index;
        private Volume volume;
        private string axis;
        private SKBitmap canvas;
        private SKBitmap canvasBuffer;
        private PlaneBufferGeometry geometry;
        private Mesh mesh;
        public bool geometryNeedsUpdate;
        
        public int ILength { get; private set; }
        public int JLength { get; private set; }
        public Func<int, int, int> SliceAccess { get; private set; }
        public Matrix4 Matrix { get; private set; }
        public uint[] ColorMap { get; set; }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                geometryNeedsUpdate = true;
            }
        }

        public string Axis
        {
            get => axis;
            set => axis = value;
        }

        public Mesh Mesh => mesh;

        public VolumeSlice(Volume volume, int index = 0, string axis = "z")
        {
            this.volume = volume;
            this._index = index;
            this.axis = axis;
            
            UpdateGeometry();

            var canvasMap = new Texture(canvas);
            canvasMap.MinFilter = Constants.LinearFilter;
            canvasMap.WrapS = canvasMap.WrapT = Constants.ClampToEdgeWrapping;
            
            var material = new MeshBasicMaterial();
            material.Map = canvasMap;
            material.Side = Constants.DoubleSide;
            material.Transparent = true;

            mesh = new Mesh(geometry, material);
            mesh.MatrixAutoUpdate = false;
            
            geometryNeedsUpdate = true;
            Repaint();
        }

        public void Repaint()
        {
            if (geometryNeedsUpdate)
            {
                UpdateGeometry();
            }

            var iLength = ILength;
            var jLength = JLength;
            var sliceAccess = SliceAccess;
            var volumeData = volume.Data;
            var upperThreshold = volume.UpperThreshold;
            var lowerThreshold = volume.LowerThreshold;
            var windowLow = volume.WindowLow;
            var windowHigh = volume.WindowHigh;

            canvasBuffer = new SKBitmap(ILength, JLength, SKColorType.Bgra8888, SKAlphaType.Premul);
            var pixmap = canvasBuffer.PeekPixels();

            unsafe
            {
                unsafe
                {
                    byte* ptr = (byte*)pixmap.GetPixels().ToPointer();
                    int pixelCount = 0;

                    if (volume.DataType == "label")
                    {
                        for (int j = 0; j < jLength; j++)
                        {
                            for (int i = 0; i < iLength; i++)
                            {
                                var k = sliceAccess(i, j);
                                int label = 0;
                                if (volumeData is float[] floatArray)
                                {
                                    label = (int)floatArray[sliceAccess(i, j)];
                                }
                                else if (volumeData is int[] intArray)
                                {
                                    label = (int)intArray[sliceAccess(i, j)];
                                }
                                else
                                {
                                    label = (int)Convert.ToSingle(volumeData.GetValue(sliceAccess(i, j)));
                                }
                                
                                label = label >= ColorMap.Length ? (label % ColorMap.Length) + 1 : label;
                                var color = ColorMap[label];

                                ptr[4 * pixelCount] = (byte)((color >> 0) & 0xff);      // B
                                ptr[4 * pixelCount + 1] = (byte)((color >> 8) & 0xff);  // G
                                ptr[4 * pixelCount + 2] = (byte)((color >> 16) & 0xff); // R
                                ptr[4 * pixelCount + 3] = (byte)((color >> 24) & 0xff); // A
                                pixelCount++;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < jLength; j++)
                        {
                            for (int i = 0; i < iLength; i++)
                            {
                                var value = 0; 
                                if (volumeData is float[] floatArray)
                                {
                                    value = (int)floatArray[sliceAccess(i, j)];
                                }
                                else if (volumeData is int[] intArray)
                                {
                                    value = (int)intArray[sliceAccess(i, j)];
                                }
                                else
                                {
                                    value = (int)Convert.ToSingle(volumeData.GetValue(sliceAccess(i, j)));
                                }
                                byte alpha = 0xff;

                                alpha = upperThreshold >= value ? (lowerThreshold <= value ? alpha : (byte)0) : (byte)0;

                                value = (int)Math.Floor(255 * (value - windowLow) / (windowHigh - windowLow));
                                var byteValue = (byte)(value > 255 ? 255 : (value < 0 ? 0 : value));

                                ptr[4 * pixelCount] = byteValue;     // B
                                ptr[4 * pixelCount + 1] = byteValue; // G
                                ptr[4 * pixelCount + 2] = byteValue; // R
                                ptr[4 * pixelCount + 3] = alpha;     // A
                                pixelCount++;
                            }
                        }
                    }
                }
            }
            mesh.Material.Map.NeedsUpdate = true;
        }

        public void UpdateGeometry()
        {
            var extracted = volume.ExtractPerpendicularPlane(axis, _index);
            SliceAccess = extracted.SliceAccess;
            JLength = extracted.JLength;
            ILength = extracted.ILength;
            Matrix = extracted.Matrix;

            canvas?.Dispose();
            canvasBuffer?.Dispose();

            canvas = new SKBitmap(extracted.PlaneWidth, extracted.PlaneHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
            canvasBuffer = new SKBitmap(ILength, JLength, SKColorType.Bgra8888, SKAlphaType.Premul);

            geometry?.Dispose();
            geometry = new PlaneBufferGeometry(extracted.PlaneWidth, extracted.PlaneHeight);

            if (mesh != null)
            {
                mesh.Geometry = geometry;
                mesh.Matrix = Matrix4.Identity();
                mesh.ApplyMatrix4(Matrix);
            }

            geometryNeedsUpdate = false;
        }

        public void Dispose()
        {
            canvas?.Dispose();
            canvasBuffer?.Dispose();
            geometry?.Dispose();
        }
    }

    public class ExtractedPlane
    {
        public Func<int, int, int> SliceAccess;
        public int JLength { get; set; }
        public int ILength { get; set; }
        public Matrix4 Matrix { get; set; }
        public int PlaneWidth { get; set; }
        public int PlaneHeight { get; set; }
    }
}