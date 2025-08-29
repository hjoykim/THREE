using System;
using System.Collections.Generic;
using System.Linq;

namespace THREE
{
    public class Volume
    {
        private float _lowerThreshold = float.NegativeInfinity;
        private float _upperThreshold = float.PositiveInfinity;

        public int XLength { get; set; } = 1;
        public int YLength { get; set; } = 1;
        public int ZLength { get; set; } = 1;
        public string[] AxisOrder { get; set; } = { "x", "y", "z" };
        public Array Data { get; set; }
        public float[] Spacing { get; set; } = { 1, 1, 1 };
        public float[] Offset { get; set; } = { 0, 0, 0 };
        public Matrix4 Matrix { get; set; }
        public Matrix4 InverseMatrix { get; set; }
        public List<VolumeSlice> SliceList { get; set; } = new List<VolumeSlice>();
        public int[] RASDimensions { get; set; }
        public int[] Dimensions { get; set; }
        public float WindowLow { get; set; }
        public float WindowHigh { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public string DataType { get; set; }
        public object Header { get; set; }

        public float LowerThreshold
        {
            get => _lowerThreshold;
            set
            {
                _lowerThreshold = value;
                SliceList.ForEach(slice => slice.geometryNeedsUpdate = true);
            }
        }

        public float UpperThreshold
        {
            get => _upperThreshold;
            set
            {
                _upperThreshold = value;
                SliceList.ForEach(slice => slice.geometryNeedsUpdate = true);
            }
        }

        public Volume() { }

        public Volume(int xLength, int yLength, int zLength, string type, byte[] arrayBuffer)
        {
            XLength = xLength;
            YLength = yLength;
            ZLength = zLength;
            AxisOrder = new string[] { "x", "y", "z" };

            Data = CreateTypedArray(type, arrayBuffer);

            if (Data.Length != XLength * YLength * ZLength)
            {
                throw new Exception("Error in Volume constructor, lengths are not matching arrayBuffer size");
            }

            Matrix = new Matrix4();
        }

        private Array CreateTypedArray(string type, byte[] arrayBuffer)
        {
            switch (type.ToLower())
            {
                case "uint8":
                case "uchar":
                case "unsigned char":
                case "uint8_t":
                    return ConvertToArray<byte>(arrayBuffer);
                case "int8":
                case "signed char":
                case "int8_t":
                    return ConvertToArray<sbyte>(arrayBuffer);
                case "int16":
                case "short":
                case "short int":
                case "signed short":
                case "signed short int":
                case "int16_t":
                    return ConvertToArray<short>(arrayBuffer);
                case "uint16":
                case "ushort":
                case "unsigned short":
                case "unsigned short int":
                case "uint16_t":
                    return ConvertToArray<ushort>(arrayBuffer);
                case "int32":
                case "int":
                case "signed int":
                case "int32_t":
                    return ConvertToArray<int>(arrayBuffer);
                case "uint32":
                case "uint":
                case "unsigned int":
                case "uint32_t":
                    return ConvertToArray<uint>(arrayBuffer);
                case "float32":
                case "float":
                    return ConvertToArray<float>(arrayBuffer);
                case "float64":
                case "double":
                    return ConvertToArray<double>(arrayBuffer);
                default:
                    return ConvertToArray<byte>(arrayBuffer);
            }
        }

        private T[] ConvertToArray<T>(byte[] buffer) where T : struct
        {
            var elementSize = System.Runtime.InteropServices.Marshal.SizeOf<T>();
            var length = buffer.Length / elementSize;
            var result = new T[length];
            
            for (int i = 0; i < length; i++)
            {
                var bytes = new byte[elementSize];
                Array.Copy(buffer, i * elementSize, bytes, 0, elementSize);
                
                if (typeof(T) == typeof(byte)) result[i] = (T)(object)bytes[0];
                else if (typeof(T) == typeof(sbyte)) result[i] = (T)(object)(sbyte)bytes[0];
                else if (typeof(T) == typeof(short)) result[i] = (T)(object)BitConverter.ToInt16(bytes, 0);
                else if (typeof(T) == typeof(ushort)) result[i] = (T)(object)BitConverter.ToUInt16(bytes, 0);
                else if (typeof(T) == typeof(int)) result[i] = (T)(object)BitConverter.ToInt32(bytes, 0);
                else if (typeof(T) == typeof(uint)) result[i] = (T)(object)BitConverter.ToUInt32(bytes, 0);
                else if (typeof(T) == typeof(float)) result[i] = (T)(object)BitConverter.ToSingle(bytes, 0);
                else if (typeof(T) == typeof(double)) result[i] = (T)(object)BitConverter.ToDouble(bytes, 0);
            }
            
            return result;
        }

        public float GetData(int i, int j, int k)
        {
            var index = k * XLength * YLength + j * XLength + i;
            return Convert.ToSingle(Data.GetValue(index));
        }

        public int Access(int i, int j, int k)
        {
            return k * XLength * YLength + j * XLength + i;
        }

        public int[] ReverseAccess(int index)
        {
            var z = index / (YLength * XLength);
            var y = (index - z * YLength * XLength) / XLength;
            var x = index - z * YLength * XLength - y * XLength;
            return new int[] { x, y, z };
        }

        public Volume Map(Func<float, int, Array, float> functionToMap)
        {
            var length = Data.Length;
            
            for (int i = 0; i < length; i++)
            {
                var currentValue = Convert.ToSingle(Data.GetValue(i));
                var newValue = functionToMap(currentValue, i, Data);
                Data.SetValue(newValue, i);
            }

            return this;
        }

        public ExtractedPlane ExtractPerpendicularPlane(string axis, int rasIndex)
        {
            float firstSpacing, secondSpacing, positionOffset;
            int ijkIndex;

            var axisInIJK = new Vector3();
            var firstDirection = new Vector3();
            var secondDirection = new Vector3();
            var planeMatrix = new Matrix4();
            var dimensions = new Vector3(XLength, YLength, ZLength);

            switch (axis)
            {
                case "x":
                    axisInIJK.Set(1, 0, 0);
                    firstDirection.Set(0, 0, -1);
                    secondDirection.Set(0, -1, 0);
                    firstSpacing = Spacing[Array.IndexOf(AxisOrder, "z")];
                    secondSpacing = Spacing[Array.IndexOf(AxisOrder, "y")];
                    
                    planeMatrix.Multiply(new Matrix4().MakeRotationY((float)Math.PI / 2));
                    positionOffset = (RASDimensions[0] - 1) / 2f;
                    planeMatrix.SetPosition(new Vector3(rasIndex - positionOffset, 0, 0));
                    break;
                    
                case "y":
                    axisInIJK.Set(0, 1, 0);
                    firstDirection.Set(1, 0, 0);
                    secondDirection.Set(0, 0, 1);
                    firstSpacing = Spacing[Array.IndexOf(AxisOrder, "x")];
                    secondSpacing = Spacing[Array.IndexOf(AxisOrder, "z")];
                    
                    planeMatrix.Multiply(new Matrix4().MakeRotationX(-(float)Math.PI / 2));
                    positionOffset = (RASDimensions[1] - 1) / 2f;
                    planeMatrix.SetPosition(new Vector3(0, rasIndex - positionOffset, 0));
                    break;
                    
                case "z":
                default:
                    axisInIJK.Set(0, 0, 1);
                    firstDirection.Set(1, 0, 0);
                    secondDirection.Set(0, -1, 0);
                    firstSpacing = Spacing[Array.IndexOf(AxisOrder, "x")];
                    secondSpacing = Spacing[Array.IndexOf(AxisOrder, "y")];
                    
                    positionOffset = (RASDimensions[2] - 1) / 2f;
                    planeMatrix.SetPosition(new Vector3(0, 0, rasIndex - positionOffset));
                    break;
            }

            firstDirection.ApplyMatrix4(InverseMatrix).Normalize();
            secondDirection.ApplyMatrix4(InverseMatrix).Normalize();
            axisInIJK.ApplyMatrix4(InverseMatrix).Normalize();

            var iLength = (int)Math.Floor(Math.Abs(firstDirection.Dot(dimensions)));
            var jLength = (int)Math.Floor(Math.Abs(secondDirection.Dot(dimensions)));
            var planeWidth = (int)Math.Abs(iLength * firstSpacing);
            var planeHeight = (int)Math.Abs(jLength * secondSpacing);

            ijkIndex = (int)Math.Abs(Math.Round(new Vector3(0, 0, rasIndex).ApplyMatrix4(InverseMatrix).Dot(axisInIJK)));

            var baseVectors = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };
            var directions = new Vector3[] { firstDirection, secondDirection, axisInIJK };

            var iDirection = directions.FirstOrDefault(x => Math.Abs(x.Dot(baseVectors[0])) > 0.9f);
            var jDirection = directions.FirstOrDefault(x => Math.Abs(x.Dot(baseVectors[1])) > 0.9f);
            var kDirection = directions.FirstOrDefault(x => Math.Abs(x.Dot(baseVectors[2])) > 0.9f);

            Func<int, int, int> sliceAccess = (i, j) =>
            {
                var si = (iDirection == axisInIJK) ? ijkIndex : (firstDirection == iDirection ? i : j);
                var sj = (jDirection == axisInIJK) ? ijkIndex : (firstDirection == jDirection ? i : j);
                var sk = (kDirection == axisInIJK) ? ijkIndex : (firstDirection == kDirection ? i : j);

                var accessI = (iDirection.Dot(baseVectors[0]) > 0) ? si : (XLength - 1) - si;
                var accessJ = (jDirection.Dot(baseVectors[1]) > 0) ? sj : (YLength - 1) - sj;
                var accessK = (kDirection.Dot(baseVectors[2]) > 0) ? sk : (ZLength - 1) - sk;

                return Access(accessI, accessJ, accessK);
            };

            return new ExtractedPlane
            {
                ILength = iLength,
                JLength = jLength,
                SliceAccess = sliceAccess,
                Matrix = planeMatrix,
                PlaneWidth = planeWidth,
                PlaneHeight = planeHeight
            };
        }

        public VolumeSlice ExtractSlice(string axis, int index)
        {
            var slice = new VolumeSlice(this, index, axis);
            SliceList.Add(slice);
            return slice;
        }

        public Volume RepaintAllSlices()
        {
            SliceList.ForEach(slice => slice.Repaint());
            return this;
        }

        public float[] ComputeMinMax()
        {
            var min = float.PositiveInfinity;
            var max = float.NegativeInfinity;
            var dataSize = Data.Length;

            for (int i = 0; i < dataSize; i++)
            {
                var value = Convert.ToSingle(Data.GetValue(i));
                if (!float.IsNaN(value))
                {
                    min = Math.Min(min, value);
                    max = Math.Max(max, value);
                }
            }

            Min = min;
            Max = max;
            return new float[] { min, max };
        }
    }
}