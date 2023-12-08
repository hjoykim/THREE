using System;
using System.Collections.Generic;

namespace THREE
{
    public class Face3 : ICloneable
    {
        public int a;

        public int b;

        public int c;

        public Vector3 Normal = new Vector3();

        public List<Vector3> VertexNormals = new List<Vector3>();

        public Color Color;

        public List<Color> VertexColors = new List<Color>();

        public List<Vector4> VertexTangents = new List<Vector4>();

        public int MaterialIndex = 0;

        public Face3(int a, int b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.Normal = new Vector3(1, 1, 1);
            this.Color = Color.ColorName(ColorKeywords.white);
            this.MaterialIndex = 0;
        }
        public Face3(int a, int b, int c, Vector3 normal)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.Normal = normal;           
        }

        public Face3(int a, int b, int c, Vector3 normal, Color color, int materialIndex = 0)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.Normal = normal;
            this.Color = color;
            this.MaterialIndex = materialIndex;
        }

        public Face3(int a, int b, int c, List<Vector3> vertexNormals, List<Color> vertexColors, int materialIndex = 0)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.VertexNormals = vertexNormals;
            this.VertexColors = vertexColors;
            this.MaterialIndex = materialIndex;
        }
        protected Face3(Face3 other)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new Face3(this);
        }
    }
}
