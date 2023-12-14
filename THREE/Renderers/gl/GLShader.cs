namespace THREE
{
    [Serializable]
    public class GLShader
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public string Code { get; set; }
        public string VertexShader { get; set; }
        public string FragmentShader { get; set; }
        public GLUniforms Uniforms { get; set; }

        public int ShaderId { get; set; }

    }
}
