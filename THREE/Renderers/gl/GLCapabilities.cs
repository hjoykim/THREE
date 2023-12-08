using OpenTK.Graphics.ES30;
using System;
using System.Linq;


namespace THREE
{
    public struct GLCapabilitiesParameters
    {
        public string precision;

        public bool logarithmicDepthBuffer;

    }

    public class GLCapabilities
    {
        public bool IsGL2;

        public string precision;

        public bool logarithmicDepthBuffer;

        public int maxTextures;

        public int maxVertexTextures;

        public int maxTextureSize;

        public int maxCubemapSize;

        public int maxAttributes;

        public int maxVertexUniforms;

        public int maxVaryings;

        public int maxFragmentUniforms;

        public bool vertexTextures;

        public bool floatFragmentTextures;

        public bool floatVertexTextures;

        public float maxAnisotropy;

        public int maxSamples;

        private GLExtensions Extensions;
        
        public GLCapabilities(GLExtensions Extensions, ref GLCapabilitiesParameters parameters)
        {
            this.IsGL2 = Extensions.Get("GL_ARB_ES3_compatibility") >-1 ? true : false;

            //this.IsGL2 = false;

            this.Extensions = Extensions;

            if (parameters.precision != null)
                this.precision = parameters.precision;
            else
                this.precision = "highp";

            string maxPrecision = GetMaxPrecision(this.precision);

            if(!maxPrecision.Equals(this.precision))
            {
                this.precision = maxPrecision;
            }

            //this.logarithmicDepthBuffer = parameters.logarithmicDepthBuffer == true;

            GL.GetInteger(GetPName.MaxTextureImageUnits,out this.maxTextures);
            GL.GetInteger(GetPName.MaxVertexTextureImageUnits, out this.maxVertexTextures);
            GL.GetInteger(GetPName.MaxTextureSize, out this.maxTextureSize);
            GL.GetInteger(GetPName.MaxCubeMapTextureSize, out this.maxCubemapSize);
            GL.GetInteger(GetPName.MaxVertexAttribs, out this.maxAttributes);
            GL.GetInteger(GetPName.MaxVertexUniformVectors, out this.maxVertexUniforms);
            GL.GetInteger(GetPName.MaxVaryingVectors, out this.maxVaryings);
            GL.GetInteger(GetPName.MaxFragmentUniformVectors, out this.maxFragmentUniforms);

            this.vertexTextures = this.maxVertexTextures > 0;
            this.floatFragmentTextures = this.IsGL2 || Extensions.ExtensionsName.Contains("GL_ARB_texture_float");
            this.floatVertexTextures = this.vertexTextures && this.floatFragmentTextures;

            GL.GetInteger(GetPName.MaxSamples, out this.maxSamples);

            this.maxSamples = IsGL2 ? this.maxSamples : 0;
            
        }
        
        public float GetMaxAnisotropy()
        {

            if (this.Extensions.ExtensionsName.Contains("GL_ARB_texture_filter_anisotropic"))
            {
                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out this.maxAnisotropy);
            }
            else
                this.maxAnisotropy = 0;

            return this.maxAnisotropy;


            throw new NotImplementedException();
            
        }

        public string GetMaxPrecision(string precision)
        {
            if (precision.Equals("highp"))
            {
                int range, value1,value2;
                GL.GetShaderPrecisionFormat(ShaderType.VertexShader, ShaderPrecision.HighFloat, out range, out value1);
                GL.GetShaderPrecisionFormat(ShaderType.FragmentShader, ShaderPrecision.HighFloat, out range, out value2);

                if (value1 > 0 && value2 > 0)
                {
                    return "highp";
                }
                precision = "mediump";
            }
            if (precision.Equals("mediump"))
            {
                int range, value1, value2;
                GL.GetShaderPrecisionFormat(ShaderType.VertexShader, ShaderPrecision.MediumFloat, out range, out value1);
                GL.GetShaderPrecisionFormat(ShaderType.FragmentShader, ShaderPrecision.MediumFloat, out range, out value2);

                if (value1 > 0 && value2 > 0)
                {
                    return "mediump";
                }
            }
            return "lowp";
        }
    }
}
