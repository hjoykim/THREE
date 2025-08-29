using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace THREE
{
    public class NRRDLoader 
    {
        public NRRDLoader() { }

        public Volume Load(string filePath)
        {
            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                long length = reader.BaseStream.Length;
                byte[] allBytes = reader.ReadBytes((int)length);
                return Parse(allBytes);
            }

        }

        public Volume Parse(byte[] data)
        {
            var _data = data;
            var _dataPointer = 0;
            var _nativeLittleEndian = BitConverter.IsLittleEndian;
            var _littleEndian = true;
            var headerObject = new NRRDHeader();

            object Scan(string type, int chunks = 1)
            {
                var _chunkSize = 1;
                Type _arrayType = typeof(byte);

                switch (type)
                {
                    case "uchar":
                        _arrayType = typeof(byte);
                        break;
                    case "schar":
                        _arrayType = typeof(sbyte);
                        break;
                    case "ushort":
                        _arrayType = typeof(ushort);
                        _chunkSize = 2;
                        break;
                    case "sshort":
                        _arrayType = typeof(short);
                        _chunkSize = 2;
                        break;
                    case "uint":
                        _arrayType = typeof(uint);
                        _chunkSize = 4;
                        break;
                    case "sint":
                        _arrayType = typeof(int);
                        _chunkSize = 4;
                        break;
                    case "float":
                        _arrayType = typeof(float);
                        _chunkSize = 4;
                        break;
                    case "complex":
                    case "double":
                        _arrayType = typeof(double);
                        _chunkSize = 8;
                        break;
                }

                var bytes = new byte[chunks * _chunkSize];
                Array.Copy(_data, _dataPointer, bytes, 0, chunks * _chunkSize);
                _dataPointer += chunks * _chunkSize;

                if (_nativeLittleEndian != _littleEndian)
                {
                    FlipEndianness(bytes, _chunkSize);
                }

                if (chunks == 1)
                {
                    return _arrayType == typeof(byte) ? bytes[0] : 
                           _arrayType == typeof(sbyte) ? (sbyte)bytes[0] :
                           _arrayType == typeof(ushort) ? BitConverter.ToUInt16(bytes, 0) :
                           _arrayType == typeof(short) ? BitConverter.ToInt16(bytes, 0) :
                           _arrayType == typeof(uint) ? BitConverter.ToUInt32(bytes, 0) :
                           _arrayType == typeof(int) ? BitConverter.ToInt32(bytes, 0) :
                           _arrayType == typeof(float) ? BitConverter.ToSingle(bytes, 0) :
                           BitConverter.ToDouble(bytes, 0);
                }

                return bytes;
            }

            void FlipEndianness(byte[] array, int chunkSize)
            {
                for (int i = 0; i < array.Length; i += chunkSize)
                {
                    for (int j = i + chunkSize - 1, k = i; j > k; j--, k++)
                    {
                        var tmp = array[k];
                        array[k] = array[j];
                        array[j] = tmp;
                    }
                }
            }

            void ParseHeader(string header)
            {
                var lines = header.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    if (Regex.IsMatch(line, @"NRRD\d+"))
                    {
                        headerObject.IsNrrd = true;
                    }
                    else if (!line.StartsWith("#"))
                    {
                        var match = Regex.Match(line, @"(.*):(.*)");
                        if (match.Success)
                        {
                            var field = match.Groups[1].Value.Trim();
                            var data = match.Groups[2].Value.Trim();
                            
                            ParseField(headerObject, field, data);
                        }
                    }
                }

                if (!headerObject.IsNrrd)
                    throw new Exception("Not an NRRD file");

                if (headerObject.Encoding == "bz2" || headerObject.Encoding == "bzip2")
                    throw new Exception("Bzip is not supported");

                if (headerObject.Vectors == null)
                {
                    headerObject.Vectors = new List<float[]>
                    {
                        new float[] { 1, 0, 0 },
                        new float[] { 0, 1, 0 },
                        new float[] { 0, 0, 1 }
                    };

                    if (headerObject.Spacings != null)
                    {
                        for (int i = 0; i <= 2; i++)
                        {
                            if (i < headerObject.Spacings.Count && !float.IsNaN(headerObject.Spacings[i]))
                            {
                                for (int j = 0; j <= 2; j++)
                                {
                                    headerObject.Vectors[i][j] *= headerObject.Spacings[i];
                                }
                            }
                        }
                    }
                }
            }

            Array ParseDataAsText(byte[] data, int start = 0, int? end = null)
            {
                var endPos = end ?? data.Length;
                var number = "";
                var lengthOfResult = headerObject.Sizes.Aggregate(1, (prev, curr) => prev * curr);
                var baseNum = headerObject.Encoding == "hex" ? 16 : 10;
                var result = Array.CreateInstance(headerObject.ArrayType, lengthOfResult);
                var resultIndex = 0;

                for (int i = start; i < endPos; i++)
                {
                    var value = data[i];
                    if ((value < 9 || value > 13) && value != 32)
                    {
                        number += (char)value;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(number))
                        {
                            var parsed = headerObject.ArrayType == typeof(float) || headerObject.ArrayType == typeof(double)
                                ? (object)double.Parse(number)
                                : Convert.ToInt32(number, baseNum);
                            result.SetValue(parsed, resultIndex++);
                        }
                        number = "";
                    }
                }

                if (!string.IsNullOrEmpty(number))
                {
                    var parsed = headerObject.ArrayType == typeof(float) || headerObject.ArrayType == typeof(double)
                        ? (object)double.Parse(number)
                        : Convert.ToInt32(number, baseNum);
                    result.SetValue(parsed, resultIndex);
                }

                return result;
            }

            var _bytes = (byte[])Scan("uchar", data.Length);
            var _length = _bytes.Length;
            string _header = null;
            var _dataStart = 0;

            for (int i = 1; i < _length; i++)
            {
                if (_bytes[i - 1] == 10 && _bytes[i] == 10)
                {
                    _header = ParseChars(_bytes, 0, i - 2);
                    _dataStart = i + 1;
                    break;
                }
            }

            ParseHeader(_header);

            var dataBytes = new byte[_bytes.Length - _dataStart];
            Array.Copy(_bytes, _dataStart, dataBytes, 0, dataBytes.Length);

            if (headerObject.Encoding.StartsWith("gz"))
            {
                using (var input = new MemoryStream(dataBytes))
                using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                using (var output = new MemoryStream())
                {
                    gzip.CopyTo(output);
                    dataBytes = output.ToArray();
                }
            }
            else if (headerObject.Encoding == "ascii" || headerObject.Encoding == "text" || 
                     headerObject.Encoding == "txt" || headerObject.Encoding == "hex")
            {
                var parsedData = ParseDataAsText(dataBytes);
                dataBytes = new byte[parsedData.Length * GetTypeSize(headerObject.ArrayType)];
                Buffer.BlockCopy(parsedData, 0, dataBytes, 0, dataBytes.Length);
            }

            var volume = new Volume();
            volume.Header = headerObject;
            volume.Data = CreateTypedArray(headerObject.ArrayType, dataBytes);
            
            var minMax = volume.ComputeMinMax();
            volume.WindowLow = minMax[0];
            volume.WindowHigh = minMax[1];

            volume.Dimensions = new int[] { headerObject.Sizes[0], headerObject.Sizes[1], headerObject.Sizes[2] };
            volume.XLength = volume.Dimensions[0];
            volume.YLength = volume.Dimensions[1];
            volume.ZLength = volume.Dimensions[2];

            if (headerObject.Vectors != null)
            {
                var xIndex = headerObject.Vectors.FindIndex(v => v[0] != 0);
                var yIndex = headerObject.Vectors.FindIndex(v => v[1] != 0);
                var zIndex = headerObject.Vectors.FindIndex(v => v[2] != 0);

                var axisOrder = new string[3];
                if (xIndex >= 0) axisOrder[xIndex] = "x";
                if (yIndex >= 0) axisOrder[yIndex] = "y";
                if (zIndex >= 0) axisOrder[zIndex] = "z";
                volume.AxisOrder = axisOrder;
            }
            else
            {
                volume.AxisOrder = new string[] { "x", "y", "z" };
            }

            var spacingX = new Vector3().FromArray(headerObject.Vectors[0]).Length();
            var spacingY = new Vector3().FromArray(headerObject.Vectors[1]).Length();
            var spacingZ = new Vector3().FromArray(headerObject.Vectors[2]).Length();
            volume.Spacing = new float[] { spacingX, spacingY, spacingZ };

            volume.Matrix = new Matrix4();
            var transitionMatrix = new Matrix4();

            if (headerObject.Space == "left-posterior-superior")
            {
                transitionMatrix.Set(-1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            }
            else if (headerObject.Space == "left-anterior-superior")
            {
                transitionMatrix.Set(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1);
            }

            if (headerObject.Vectors == null)
            {
                volume.Matrix.Set(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            }
            else
            {
                var v = headerObject.Vectors;
                var ijkToTransition = new Matrix4().Set(
                    v[0][0], v[1][0], v[2][0], 0,
                    v[0][1], v[1][1], v[2][1], 0,
                    v[0][2], v[1][2], v[2][2], 0,
                    0, 0, 0, 1
                );

                volume.Matrix = new Matrix4().MultiplyMatrices(ijkToTransition, transitionMatrix);
            }

            volume.InverseMatrix = new Matrix4().Copy(volume.Matrix).Invert();

            if (volume.LowerThreshold == float.NegativeInfinity)
                volume.LowerThreshold = minMax[0];
            if (volume.UpperThreshold == float.PositiveInfinity)
                volume.UpperThreshold = minMax[1];

            return volume;
        }

        public string ParseChars(byte[] array, int start = 0, int? end = null)
        {
            var endPos = end ?? array.Length;
            var output = new StringBuilder();
            
            for (int i = start; i < endPos; i++)
            {
                output.Append((char)array[i]);
            }
            
            return output.ToString();
        }

        private void ParseField(NRRDHeader header, string field, string data)
        {
            switch (field)
            {
                case "type":
                    header.ArrayType = ParseType(data);
                    header.Type = data;
                    break;
                case "endian":
                    header.Endian = data;
                    break;
                case "encoding":
                    header.Encoding = data;
                    break;
                case "dimension":
                    header.Dim = int.Parse(data);
                    break;
                case "sizes":
                    header.Sizes = data.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(int.Parse).ToList();
                    break;
                case "space":
                    header.Space = data;
                    break;
                case "space origin":
                    var origin = data.Split('(')[1].Split(')')[0].Split(',');
                    header.SpaceOrigin = origin;
                    break;
                case "space directions":
                    var matches = Regex.Matches(data, @"\(.*?\)");
                    header.Vectors = matches.Cast<Match>()
                        .Select(m => m.Value.Trim('(', ')').Split(',').Select(float.Parse).ToArray())
                        .ToList();
                    break;
                case "spacings":
                    header.Spacings = data.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(float.Parse).ToList();
                    break;
            }
        }

        private Type ParseType(string data)
        {
            switch (data)
            {
                case "uchar":
                case "unsigned char":
                case "uint8":
                case "uint8_t":
                    return typeof(byte);
                case "signed char":
                case "int8":
                case "int8_t":
                    return typeof(sbyte);
                case "short":
                case "short int":
                case "signed short":
                case "signed short int":
                case "int16":
                case "int16_t":
                    return typeof(short);
                case "ushort":
                case "unsigned short":
                case "unsigned short int":
                case "uint16":
                case "uint16_t":
                    return typeof(ushort);
                case "int":
                case "signed int":
                case "int32":
                case "int32_t":
                    return typeof(int);
                case "uint":
                case "unsigned int":
                case "uint32":
                case "uint32_t":
                    return typeof(uint);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                default:
                    throw new Exception($"Unsupported NRRD data type: {data}");
            }
        }

        private int GetTypeSize(Type type)
        {
            if (type == typeof(byte) || type == typeof(sbyte)) return 1;
            if (type == typeof(short) || type == typeof(ushort)) return 2;
            if (type == typeof(int) || type == typeof(uint) || type == typeof(float)) return 4;
            if (type == typeof(double)) return 8;
            return 1;
        }

        private Array CreateTypedArray(Type type, byte[] data)
        {
            var elementSize = GetTypeSize(type);
            var length = data.Length / elementSize;
            var array = Array.CreateInstance(type, length);

            for (int i = 0; i < length; i++)
            {
                var offset = i * elementSize;
                object value = null;

                if (type == typeof(byte)) value = data[offset];
                else if (type == typeof(sbyte)) value = (sbyte)data[offset];
                else if (type == typeof(short)) value = BitConverter.ToInt16(data, offset);
                else if (type == typeof(ushort)) value = BitConverter.ToUInt16(data, offset);
                else if (type == typeof(int)) value = BitConverter.ToInt32(data, offset);
                else if (type == typeof(uint)) value = BitConverter.ToUInt32(data, offset);
                else if (type == typeof(float)) value = BitConverter.ToSingle(data, offset);
                else if (type == typeof(double)) value = BitConverter.ToDouble(data, offset);

                array.SetValue(value, i);
            }

            return array;
        }
    }

    public class NRRDHeader
    {
        public bool IsNrrd { get; set; }
        public Type ArrayType { get; set; }
        public string Type { get; set; }
        public string Endian { get; set; }
        public string Encoding { get; set; }
        public int Dim { get; set; }
        public List<int> Sizes { get; set; }
        public string Space { get; set; }
        public string[] SpaceOrigin { get; set; }
        public List<float[]> Vectors { get; set; }
        public List<float> Spacings { get; set; }
    }
}