using System.Collections;

namespace THREE
{
    
    public class ShaderMaterial : Material
    {
        public struct Extensions
        {
            public bool derivatives;
            public bool fragDepth;
            public bool drawBuffers;
            public bool shaderTextureLOD;
        }

        public GLUniforms Uniforms;

        public Extensions extensions;        

        public GLAttributes Attributes;       

        public string VertexShader =
            "void main() {\n\t" +
            "   gl_Position = projectionMatrix*modelViewMatrix*vec4(position,1.0);\n" +
            "}";

        public string FragmentShader=
            "void main() {\n\t" +
            "   gl_FragColor = vec4(1.0,0.0,0.0,1.0);\n" +
            "}";

        public int Shading = (int)Constants.SmoothShading;

        public bool Lights = false;

        //public bool Skinning = false;

        //public bool MorphTargets { get; set; }

        //public bool MorphNormals = false;

        public string Index0AttributeName = null;

        public bool UniformsNeedUpdate = false;
        
        public Hashtable DefaultAttributeValues = new Hashtable();

        public ShaderMaterial(Hashtable parameters = null) : base()
        {
            this.type = "ShaderMaterial";

            this.Attributes = new GLAttributes();

            this.Uniforms = new GLUniforms();

            this.Wireframe = false;

            this.WireframeLineWidth = 1;

            this.extensions = new Extensions()
            {
               derivatives =false,
               fragDepth =false,
               drawBuffers=false,
               shaderTextureLOD=false
            };

            this.DefaultAttributeValues = new Hashtable()
            {
                {"color",new float[]{1,1,1}},
                {"uv",new float[]{0,0} },
                {"uv2",new float[]{0,0} }
            };

            if(parameters!=null)
                this.SetValues(parameters);
        }

        protected ShaderMaterial(ShaderMaterial other) :base(other)
        {
            this.FragmentShader = other.FragmentShader;
            this.VertexShader = other.VertexShader;
            this.Uniforms = UniformsUtils.CloneUniforms(other.Uniforms);
            this.Defines = (Hashtable)other.Defines.Clone();

            this.Wireframe = other.Wireframe;
            this.WireframeLineWidth = other.WireframeLineWidth;

            this.Lights = other.Lights;
            this.Clipping = other.Clipping;
            this.Skinning = other.Skinning;

            this.MorphTargets = other.MorphTargets;
            this.MorphNormals = other.MorphNormals;

            this.extensions = other.extensions;
        }

        public new object Clone()
        {
            return new ShaderMaterial(this);
        }
    }
}
