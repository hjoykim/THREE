using OpenTK.Graphics.ES30;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace THREE
{
    public class GLProgram : DisposableObject
    {
        protected static int ProgramIdCount;

        public int Id = ProgramIdCount++;

        public string Code;

        public int UsedTimes=1;       

        public int program { get; set; }

        private GLRenderer renderer;

        //private GLExtensions extensions;


        public string Name;

        //private Material material;

        //private Hashtable defines;

        public Hashtable Diagnostics = new Hashtable();

        private GLUniforms cachedUniforms;

        private Hashtable cachedAttributes;

        public int NumMultiviewViews;

        public GLShader VertexShader;

        public GLShader FragmentShader;

        private GLBindingStates bindingStates;
        public GLProgram()
        {
        }
        public GLProgram(GLRenderer renderer, string cacheKey, Hashtable parameters,GLBindingStates bindingStates)
        {
            this.renderer = renderer;

            this.bindingStates = bindingStates;

            string VertexShader = parameters["vertexShader"] as string;

            string FragmentShader = parameters["fragmentShader"] as string;
            //this.extensions = extensions;

            this.Code = cacheKey;

            //this.material = material;

            Hashtable defines = parameters["defines"] as Hashtable;

           

            string shadowMapTypeDefine = GenerateShadowMapTypeDefine(parameters);

            string envMapTypeDefine = GenerateEnvMapTypeDefine(parameters);

            string envMapModeDefine = GenerateEnvMapModeDefine(parameters);

            string envMapBlendingDefine = GenerateEnvMapBlendingDefine(parameters);

            float gammaFactorDefine = (renderer.GammaFactor > 0) ? renderer.GammaFactor : 1.0f;

            string[] customExtensions = (bool)parameters["isGL2"] == true ? new string[]{""} : GenerateExtensions(parameters);

            string customDefines = GenerateDefines(defines);

            this.program = GL.CreateProgram();

            string prefixVertex, prefixFragment;

            string versionString = (string)parameters["glslVersion"] != null ? "#version " + (string)parameters["glslVersion"] + "\n" : "";

            //this.NumMultiviewViews = (int)parameters["numMultiviewViews"];

            if ((bool)parameters["isRawShaderMaterial"])
            {
                prefixVertex = string.Join("\n", customDefines);
                
                if (prefixVertex.Length > 0)
                    prefixVertex += "\n";

                List<string> customExtensionsList = customExtensions.ToList();
                customExtensionsList.Add(customDefines);

                prefixFragment = string.Join("\n", customExtensionsList);

                if (prefixFragment.Length > 0)
                    prefixFragment += "\n";
            }
            else
            {
               
                string[] prefixVertexs = new string[]
                {
                    //"#version 120",
                    //"#define GL_ES",
                    //"#ifdef GL_ES",
                    "#version 300 es\n",
                    "#define attribute in",
                    "#define varying out",
                    "#define texture2D texture",
                    GeneratePrecision(parameters),
                    //"#endif",
                    "#define SHADER_NAME "+parameters["shaderName"],

                    customDefines,
                
                    (bool)parameters["instancing"]==true ? "#define USE_INSTANCING":"",
                (bool)parameters["instancingColor"]==true? "#define USE_INSTANCING_COLOR":"",
                    (bool)parameters["supportsVertexTextures"] ? "#define VERTEX_TEXTURES" : "",

                    "#define GAMMA_FACTOR " + gammaFactorDefine.ToString(),
               
                    "#define MAX_BONES " + parameters["maxBones"].ToString(),
                    ( (bool)parameters["useFog"] && (bool)parameters["fog"] ) ? "#define USE_FOG" : "",
                    ( (bool)parameters["useFog"] && (bool)parameters["fogExp2"] ) ? "#define FOG_EXP2" : "",
                
                    (bool)parameters["map"] ? "#define USE_MAP" : "",
                    (bool)parameters["envMap"] ? "#define USE_ENVMAP" : "",
                    (bool)parameters["envMap"] ? "#define " + envMapModeDefine : "",
                    (bool)parameters["lightMap"] ? "#define USE_LIGHTMAP" : "",
                    (bool)parameters["aoMap"] ? "#define USE_AOMAP" : "",
                    (bool)parameters["emissiveMap"] ? "#define USE_EMISSIVEMAP" : "",
                    (bool)parameters["bumpMap"] ? "#define USE_BUMPMAP" : "",
                    (bool)parameters["normalMap"] ? "#define USE_NORMALMAP" : "",
                    ( (bool)parameters["normalMap"] && (bool)parameters["objectSpaceNormalMap"] ) ? "#define OBJECTSPACE_NORMALMAP" : "",
                    ( (bool)parameters["normalMap"] && (bool)parameters["tangentSpaceNormalMap"] ) ? "#define TANGENTSPACE_NORMALMAP" : "",
                (bool)parameters["clearcoatMap"] ?"#define USE_CLEARCOATMAP":"",
                (bool)parameters["clearcoatRoughnessMap"]?"#define USE_CLEARCOAT_ROUGHNESSMAP":"",
                    (bool)parameters["clearcoatNormalMap"] ? "#define USE_CLEARCOAT_NORMALMAP" : "",
                    (bool)parameters["displacementMap"] && (bool)parameters["supportsVertexTextures"] ? "#define USE_DISPLACEMENTMAP" : "",
                    (bool)parameters["specularMap"] ? "#define USE_SPECULARMAP" : "",
                    (bool)parameters["roughnessMap"] ? "#define USE_ROUGHNESSMAP" : "",
                    (bool)parameters["metalnessMap"] ? "#define USE_METALNESSMAP" : "",
                    (bool)parameters["alphaMap"] ? "#define USE_ALPHAMAP" : "",
                (bool)parameters["transmissionMap"]?"#define USE_TRANSMISSIONMAP":"",

                    (bool)parameters["vertexTangents"] ? "#define USE_TANGENT" : "",
                    (bool)parameters["vertexColors"] ? "#define USE_COLOR" : "",
                    (bool)parameters["vertexUvs"] ? "#define USE_UV" : "",
                (bool)parameters["uvsVertexOnly"] ? "#define UVS_VERTEX_ONLY":"",
                    (bool)parameters["flatShading"] ? "#define FLAT_SHADED" : "",

                    (bool)parameters["skinning"] ? "#define USE_SKINNING" : "",
                    (bool)parameters["useVertexTexture"] ? "#define BONE_TEXTURE" : "",

                    (bool)parameters["morphTargets"] ? "#define USE_MORPHTARGETS" : "",
                    (bool)parameters["morphNormals"] && (bool)parameters["flatShading"] == false ? "#define USE_MORPHNORMALS" : "",
                    (bool)parameters["doubleSided"] ? "#define DOUBLE_SIDED" : "",
                    (bool)parameters["flipSided"] ? "#define FLIP_SIDED" : "",
               
                    (bool)parameters["shadowMapEnabled"] ? "#define USE_SHADOWMAP" : "",
                    (bool)parameters["shadowMapEnabled"] ? "#define " + shadowMapTypeDefine : "",

                    (bool)parameters["sizeAttenuation"] ? "#define USE_SIZEATTENUATION" : "",

                    (bool)parameters["logarithmicDepthBuffer"] ? "#define USE_LOGDEPTHBUF" : "",
                    (bool)parameters["logarithmicDepthBuffer"] && ( (bool) parameters["renderExtensionFragDepth"]) ? "#define USE_LOGDEPTHBUF_EXT" : "",

                    "uniform mat4 modelMatrix;",
                    "uniform mat4 modelViewMatrix;",
                    "uniform mat4 projectionMatrix;",
                    "uniform mat4 viewMatrix;",
                    "uniform mat3 normalMatrix;",
                    "uniform vec3 cameraPosition;",
                    "uniform bool isOrthographic;",

                    "#ifdef USE_INSTANCING",

                    " attribute mat4 instanceMatrix;",

                    "#endif",

                     "#ifdef USE_INSTANCING_COLOR",

                    " attribute vec3 instanceColor;",

                    "#endif",

                    "attribute vec3 position;",
                    "attribute vec3 normal;",
                    "attribute vec2 uv;",

                    "#ifdef USE_TANGENT",

                    "	attribute vec4 tangent;",

                    "#endif",

                    "#ifdef USE_COLOR",

                    "	attribute vec3 color;",

                    "#endif",

                    "#ifdef USE_MORPHTARGETS",

                    "	attribute vec3 morphTarget0;",
                    "	attribute vec3 morphTarget1;",
                    "	attribute vec3 morphTarget2;",
                    "	attribute vec3 morphTarget3;",

                    "	#ifdef USE_MORPHNORMALS",

                    "		attribute vec3 morphNormal0;",
                    "		attribute vec3 morphNormal1;",
                    "		attribute vec3 morphNormal2;",
                    "		attribute vec3 morphNormal3;",

                    "	#else",

                    "		attribute vec3 morphTarget4;",
                    "		attribute vec3 morphTarget5;",
                    "		attribute vec3 morphTarget6;",
                    "		attribute vec3 morphTarget7;",

                    "	#endif",

                    "#endif",

                    "#ifdef USE_SKINNING",

                    "	attribute vec4 skinIndex;",
                    "	attribute vec4 skinWeight;",

                    "#endif",
                    "\n"

                };

                prefixVertex = string.Join("\n", prefixVertexs);


                string[] prefixFragments = new string[]
                {
                    //"#version 120",
                    //"#define GL_ES",
                    //"#ifdef GL_ES",
                    "#version 300 es\n",
                    String.Join("\n",customExtensions),
                    GeneratePrecision(parameters), 
                    
                    "#define varying in",
                    "out highp vec4 pc_fragColor;",
                    "#define gl_FragColor pc_fragColor",
                    "#define gl_FragDepthEXT gl_FragDepth",
                    "#define texture2D texture",
                    "#define textureCube texture",
                    "#define texture2DProj textureProj",
                    "#define texture2DLodEXT textureLod",
                    "#define texture2DProjLodEXT textureProjLod",
                    "#define textureCubeLodEXT textureLod",
                    "#define texture2DGradEXT textureGrad",
                    "#define texture2DProjGradEXT textureProjGrad",
                    "#define textureCubeGradEXT textureGrad",
                    //"#endif",
                    "#define SHADER_NAME "+parameters["shaderName"],

                    //String.Join("\n",customExtensions),                   

                    customDefines,                    
                        
			        (float)parameters["alphaTest"]!=0 ? "#define ALPHATEST " + (float)parameters["alphaTest"] + ( (float)parameters["alphaTest"] % 1>0 ? "" : ".0" ) : "", // add ".0" if integer

			        "#define GAMMA_FACTOR " + gammaFactorDefine,

			        (  (bool)parameters["useFog"] &&  (bool)parameters["fog"] ) ? "#define USE_FOG" : "",
			        (  (bool)parameters["useFog"] &&  (bool)parameters["fogExp2"] ) ? "#define FOG_EXP2" : "",

			         (bool)parameters["map"] ? "#define USE_MAP" : "",
			         (bool)parameters["matcap"] ? "#define USE_MATCAP" : "",
			         (bool)parameters["envMap"] ? "#define USE_ENVMAP" : "",
			         (bool)parameters["envMap"] ? "#define " + envMapTypeDefine : "",
			         (bool)parameters["envMap"] ? "#define " + envMapModeDefine : "",
			         (bool)parameters["envMap"] ? "#define " + envMapBlendingDefine : "",
			         (bool)parameters["lightMap"] ? "#define USE_LIGHTMAP" : "",
			         (bool)parameters["aoMap"] ? "#define USE_AOMAP" : "",
			         (bool)parameters["emissiveMap"] ? "#define USE_EMISSIVEMAP" : "",
			         (bool)parameters["bumpMap"] ? "#define USE_BUMPMAP" : "",              
			         (bool)parameters["normalMap"] ? "#define USE_NORMALMAP" : "",
			        ((bool)parameters["normalMap"] &&  (bool)parameters["objectSpaceNormalMap"] ) ? "#define OBJECTSPACE_NORMALMAP" : "",
			        ((bool)parameters["normalMap"] &&  (bool)parameters["tangentSpaceNormalMap"] ) ? "#define TANGENTSPACE_NORMALMAP" : "",
                (bool)parameters["clearcoatMap"] ? "#define USE_CLEARCOAT":"",
                (bool)parameters["clearcoatRoughnessMap"] ? "#define USE_CLEARCOAT_ROUGHNESSMAP":"",
                     (bool)parameters["clearcoatNormalMap"] ? "#define USE_CLEARCOAT_NORMALMAP" : "",
			         (bool)parameters["specularMap"] ? "#define USE_SPECULARMAP" : "",
			         (bool)parameters["roughnessMap"] ? "#define USE_ROUGHNESSMAP" : "",
			         (bool)parameters["metalnessMap"] ? "#define USE_METALNESSMAP" : "",
			         (bool)parameters["alphaMap"] ? "#define USE_ALPHAMAP" : "",

			         (bool)parameters["sheen"] ? "#define USE_SHEEN" : "",

                     (bool)parameters["transmissionMap"] ? "#define USE_TRANSMISSIONMAP" : "",

                     (bool)parameters["vertexTangents"] ? "#define USE_TANGENT" : "",
			         (bool)parameters["vertexColors"] ||(bool)parameters["instancingColor"] ? "#define USE_COLOR" : "",
			         (bool)parameters["vertexUvs"] ? "#define USE_UV" : "",
                (bool)parameters["uvsVertexOnly"] ? "#define UVS_VERTEX_ONLY":"",

			         (bool)parameters["gradientMap"] ? "#define USE_GRADIENTMAP" : "",

			         (bool)parameters["flatShading"] ? "#define FLAT_SHADED" : "",

			         (bool)parameters["doubleSided"] ? "#define DOUBLE_SIDED" : "",
			         (bool)parameters["flipSided"] ? "#define FLIP_SIDED" : "",

			         (bool)parameters["shadowMapEnabled"] ? "#define USE_SHADOWMAP" : "",
			         (bool)parameters["shadowMapEnabled"] ? "#define " + shadowMapTypeDefine : "",

			         (bool)parameters["premultipliedAlpha"] ? "#define PREMULTIPLIED_ALPHA" : "",

			         (bool)parameters["physicallyCorrectLights"] ? "#define PHYSICALLY_CORRECT_LIGHTS" : "",

			         (bool)parameters["logarithmicDepthBuffer"] ? "#define USE_LOGDEPTHBUF" : "",
			         (bool)parameters["logarithmicDepthBuffer"] && (  (bool)parameters["rendererExtensionFragDepth"]) ? "#define USE_LOGDEPTHBUF_EXT" : "",

			         ((bool)parameters["extensionShaderTextureLOD"] || (bool)parameters["envMap"]) && (bool)parameters["rendererExtensionShaderTextureLOD"] ? "#define TEXTURE_LOD_EXT" : "",

			        "uniform mat4 viewMatrix;",
			        "uniform vec3 cameraPosition;",
                    "uniform bool isOrthographic;",

			        ( (int)parameters["toneMapping"] != Constants.NoToneMapping ) ? "#define TONE_MAPPING" : "",
			        ( (int)parameters["toneMapping"] != Constants.NoToneMapping ) ? renderer.ShaderLib.getChunk( "tonemapping_pars_fragment") : "", // this code is required here because it is used by the toneMapping() function defined below
			        ( (int)parameters["toneMapping"] != Constants.NoToneMapping ) ? GetToneMappingFunction( "toneMapping", (int)parameters["toneMapping"] ) : "",

			         (bool)parameters["dithering"] ? "#define DITHERING" : "",
		        
 			        renderer.ShaderLib.getChunk("encodings_pars_fragment") , // this code is required here because it is used by the various encoding/decoding function defined below
			         parameters["mapEncoding"]!=null ? GetTexelDecodingFunction( "mapTexelToLinear", (int)parameters["mapEncoding"] ) : "",
			         parameters["matcapEncoding"]!=null ? GetTexelDecodingFunction( "matcapTexelToLinear", (int)parameters["matcapEncoding"] ) : "",
			         parameters["envMapEncoding"]!=null ? GetTexelDecodingFunction( "envMapTexelToLinear", (int)parameters["envMapEncoding"] ) : "",
			         parameters["emissiveMapEncoding"]!=null ? GetTexelDecodingFunction( "emissiveMapTexelToLinear", (int)parameters["emissiveMapEncoding"] ) : "",
                     parameters["lightMapEncoding"]!=null ? GetTexelDecodingFunction("lightMapTexelToLinear",(int)parameters["lightMapEncoding"]) : "",
                     parameters["outputEncoding"]!=null ?GetTexelEncodingFunction("linearToOutputTexel",(int)parameters["outputEncoding"]) :"",
			         !customDefines.Contains("DEPTH_PACKING") && parameters["depthPacking"]!= null  ? "#define DEPTH_PACKING " + (int)parameters["depthPacking"] : "",
                     "\n"			        
                };

                prefixFragment = string.Join("\r\n", prefixFragments);
            }

            VertexShader = ResolveIncludes(VertexShader);
            VertexShader = ReplaceLightsNums(VertexShader, parameters);
            VertexShader = ReplaceClippingPlaneNums(VertexShader, parameters);

            FragmentShader = ResolveIncludes(FragmentShader);
            FragmentShader = ReplaceLightsNums(FragmentShader, parameters);
            FragmentShader = ReplaceClippingPlaneNums(FragmentShader, parameters);
            
            VertexShader = UnrollLoops(VertexShader);
            FragmentShader = UnrollLoops(FragmentShader);

            //Debug.WriteLine(VertexShader);

            //if ((bool)parameters["isGL2"] && !(bool)parameters["isRawShaderMaterial"])
            //{
               
            //    versionString = @"#version 300 es\n";


            //    string[] prefixVertexs = new string[]
            //    {
            //        "#define attribute in",
            //        "#define varying out",
            //        "#define texture2D texture"
            //    };
            //    prefixVertex = String.Join("\n", prefixVertexs) + prefixVertex;

            //    string[] prefixFragments = new string[]
            //    {
            //        "#define varying in",
            //        parameters["glslVersion"]!=null && (string)parameters["glslVersion"] == "300 es" ? "" : "out highp vec4 pc_fragColor;",
            //        parameters["glslVersion"]!=null && (string)parameters["glslVersion"] == "300 es" ? "" : "#define gl_FragColor pc_fragColor",
            //        "#define gl_FragDepthEXT gl_FragDepth",
            //        "#define texture2D texture",
            //        "#define textureCube texture",
            //        "#define texture2DProj textureProj",
            //        "#define texture2DLodEXT textureLod",
            //        "#define texture2DProjLodEXT textureProjLod",
            //        "#define textureCubeLodEXT textureLod",
            //        "#define texture2DGradEXT textureGrad",
            //        "#define texture2DProjGradEXT textureProjGrad",
            //        "#define textureCubeGradEXT textureGrad"
            //    };

            //    prefixFragment = String.Join("\n", prefixFragments) + prefixFragment;

            //}  
            
            string vertexGlsl = prefixVertex + VertexShader+"\r\n";
            string fragmentGlsl = prefixFragment + FragmentShader+"\r\n";

            //File.WriteAllText(parameters["shaderName"]+"_vshader.txt", vertexGlsl);
            //File.WriteAllText(parameters["shaderName"]+"_fshader.txt", fragmentGlsl);
            GLShader glVertexShader = new GLShader(ShaderType.VertexShader, vertexGlsl);
            GLShader glFragmentShader = new GLShader(ShaderType.FragmentShader, fragmentGlsl);


            //Debug.WriteLine(glVertexShader.Code);
            //Debug.WriteLine(glFragmentShader.Code);
            GL.AttachShader(program, glVertexShader.Shader);
            ErrorCode code1 = GL.GetError();
            GL.AttachShader(program, glFragmentShader.Shader);
            ErrorCode code2 = GL.GetError();

            if (parameters["indexOfAttributeName"] != null)
            {
                GL.BindAttribLocation(program, 0, (string)parameters["indexOfAttributeName"]);
            }
            else if ((bool)parameters["morphTargets"] == true)
            {
                GL.BindAttribLocation(program, 0, "position");
            }

            GL.LinkProgram(program);

            if ((bool)renderer.debug["checkShaderErrors"])
            {
                string programLog = GL.GetProgramInfoLog(program);
                string vertexLog = GL.GetShaderInfoLog(glVertexShader.Shader);
                string fragmentLog = GL.GetShaderInfoLog(glFragmentShader.Shader);

                bool runnable = true;
                bool haveDiagnostics = true;
                int linkStatus;
                GL.GetProgram(program, GetProgramParameterName.LinkStatus, out linkStatus);
                if (linkStatus == 0)
                {
                    runnable = false;
                    
                    string vertexErrors = GetShaderErrors(glVertexShader, "vertex");
                    if (!String.IsNullOrEmpty(vertexErrors))
                    {
                        Debug.WriteLine(vertexErrors);
                        Debug.WriteLine(glVertexShader.Code);
                    }
                   
                    string fragmentErrors = GetShaderErrors(glFragmentShader, "fragment");
                    if (!String.IsNullOrEmpty(fragmentErrors))
                    {
                        Debug.WriteLine(fragmentErrors);
                        Debug.WriteLine(glFragmentShader.Code);
                    }
                        int validateState;
                    GL.GetProgram(program,GetProgramParameterName.ValidateStatus,out validateState);
                    string errorMessage = String.Format("THREE.Renderers.gl.GLProgram: shader error {0}, GL.VALIDATE_STATUS {1}, GL.GetProgramInfoLog {2}",
                        GL.GetError(), validateState, programLog + "\n" + vertexLog + "\n" + fragmentLog);                                
                    
                    
                    throw new Exception(errorMessage);
                }
                else if (!String.IsNullOrEmpty(programLog))
                {
                    Trace.TraceWarning("THREE.Renderers.gl.GLProgram: GL.GetProgramInfoLog() :" + programLog);
                }
                else if (String.IsNullOrEmpty(vertexLog) || String.IsNullOrEmpty(fragmentLog))
                {
                    haveDiagnostics = false;
                    //int length;
                    //string source;
                    //GL.GetShaderSource(glVertexShader.Shader, 1024, out length, out source);
                    //Debug.WriteLine(source);
                }

                if (haveDiagnostics)
                {
                    this.Diagnostics.Add("runnable", runnable);                   
                    this.Diagnostics.Add("programLog", programLog);
                    this.Diagnostics.Add("vertexShader", new Hashtable() { { "log", vertexLog }, { "prefix", prefixVertex } });
                    this.Diagnostics.Add("fragmentShader", new Hashtable() { { "log", fragmentLog }, { "prefix", prefixFragment } });
                }

            }

            GL.DeleteShader(glVertexShader.Shader);
            GL.DeleteShader(glFragmentShader.Shader);

            this.VertexShader = glVertexShader;
            this.FragmentShader = glFragmentShader;
            
        }

        private string RemoveEmptyLines(string shaderSource)
        {
            string[] lines = shaderSource.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string newSource = string.Empty;

            foreach (string line in lines)
            {
                newSource += line + Environment.NewLine;
            }

            return newSource;
        }
        private string AddLineNumbers(string code)
        {
            var lines = code.Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = (i + 1) + ": " + lines[i];
            }
            return string.Join("\n", lines);
        }

        private string[] GetEncodingComponents(int encoding)
        {
            
            switch (encoding)
            {
                case 3000 : //Constants.LinearEncoding :
                    return new string[2] { "Linear", "( value )" };
                case 3001://Constants.sRGBEncoding
                    return new string[2] { "sRGB", "( value )" };
                case 3002: //Constants.RGBEEncoding
                    return new string[2] { "RGBE", "( value )" };
                case 3004:// Constants.RGBM7Encoding
                    return new string[2] { "RGBM", "( value )" };
                case 3005://Constants.RGBM16Encoding
                    return new string[2] { "RGBM", "( value )" };
                case 3006: // Constants.RGBDEncoding
                    return new string[2] { "RGBD", "( value )" };
                case 3007: // Constants.GammaEncoding
                    return new string[2] { "Gamma", "( value )" };
                case 3003://Constants.LogLuvEncoding
                    return new string[2] { "LogLuv", "( value )" };
                default:
                    Debug.WriteLine("Unsupported encoding:" + encoding);
                    return new string[2] { "Linear", "( Value )" };
            }
        }

        private string GetShaderErrors(GLShader shader, string type)
        {
            int status;
            GL.GetShader(shader.Shader, ShaderParameter.CompileStatus,out status);

            string log = GL.GetShaderInfoLog(shader.Shader);

            if (status == 1 && String.IsNullOrEmpty(log.Trim())) return "";

            int length;
            string source;

            GL.GetShaderSource(shader.Shader, 4096000, out length, out source);

            return "THREE.Renderers.gl.GLProgram: GL.GetShaderInfoLog() " + type + "\n" + log + AddLineNumbers(source);

        }

        private string GetTexelDecodingFunction(string functionName, int encoding)
        {
            string[] components = GetEncodingComponents(encoding);

            return "vec4 " + functionName + " ( vec4 value ) { return " + components[0] + "ToLinear" + components[1] + ";}";
        }

        private string GetTexelEncodingFunction(string functionName, int encoding)
        {
            string[] components = GetEncodingComponents(encoding);

            return "vec4 " + functionName + " ( vec4 value ) { return LinearTo" + components[0] + components[1] + ";}";
        }

        private string GetToneMappingFunction(string functionName, int toneMapping ) {

	        string toneMappingName;

	        switch ( toneMapping ) {

		        case 1 : //Constants.LinearToneMapping:
			        toneMappingName = "Linear";
			        break;

		        case 2 : //Constants.ReinhardToneMapping:
			        toneMappingName = "Reinhard";
			        break;

		        case 3: //Constants.CineonToneMapping:
                    toneMappingName = "OptimizedCineon";
			        break;

		        case 4: //Constants.ACESFilmicToneMapping:
                    toneMappingName = "ACESFilmic";
			        break;

		        case 5 : //Constants.CustomToneMapping:
			        toneMappingName = "Custom";
			        break;

		        default:
			        Trace.WriteLine( "unsupported toneMapping: " + toneMapping );
                    toneMappingName = "Linear";
                    break;

	        }

	        return "vec3 " + functionName + "( vec3 color ) { return " + toneMappingName + "ToneMapping( color ); }";

        }
        private string[] GenerateExtensions(Hashtable parameters)
        {
            string[] chunks = new string[4];
            

            if ((bool)parameters["extensionDerivatives"] || (bool)parameters["envMapCubeUV"] || (bool)parameters["bumpMap"] || (bool)parameters["tangentSpaceNormalMap"] || (bool)parameters["clearcoatNormalMap"] || (bool)parameters["flatShading"])
                chunks[0] = "#extension GL_OES_standard_derivatives : enable";
            else
                chunks[0] = "";

            if (((bool)parameters["extensionFragDepth"] || (bool)parameters["logarithmicDepthBuffer"]) && (bool)parameters["renderExtensionFragDepth"])
                chunks[1] = "#extension GL_EXT_frag_depth : enable";
            else
                chunks[1] = "";

            if ((bool)parameters["extensionDrawBuffers"] && (bool)parameters["renderExtensionsDrawBuffers"])
                chunks[2] = "#extension GL_EXT_draw_buffers : require";            
            else
                chunks[2] = "";

            if (((bool)parameters["extensionShaderTextureLOD"] || (bool)parameters["envMap"]) && (bool)parameters["renderExtensionShaderTextureLod"])
                chunks[3] = "#extension GL_EXT_shader_texture_lod : enable";
            else
                chunks[3] = "";

            return chunks;
        }

        private string GenerateDefines(Hashtable defines)
        {
            if (defines.Count <= 0) return string.Empty;

            List<string> chunks = new List<string>();
            foreach (DictionaryEntry entry in defines)
            {
                if (entry.Value==null) continue;

                chunks.Add(string.Format("#define {0} {1}",entry.Key,entry.Value));
            }

            return String.Join("\n", chunks).Trim();
        }

        private Hashtable FetchAttributeLocations(int program)
        {
            Hashtable attributes = new Hashtable();

            int n;

            GL.GetProgram(program, GetProgramParameterName.ActiveAttributes, out n);

            for (int i = 0; i < n; i++)
            {
                int size;

                ActiveAttribType attrType;

                string name = GL.GetActiveAttrib(program, i, out size, out attrType);

                int location = GL.GetAttribLocation(program, name);

                attributes.Add(name, location);
            }

            return attributes;
        }

        private string ReplaceLightsNums(string str, Hashtable parameters)
        {
            return str
                .Replace("NUM_DIR_LIGHTS", parameters["numDirLights"].ToString())
                .Replace("NUM_SPOT_LIGHTS", parameters["numSpotLights"].ToString())
                .Replace("NUM_RECT_AREA_LIGHTS", parameters["numRectAreaLights"].ToString())
                .Replace("NUM_POINT_LIGHTS", parameters["numPointLights"].ToString())
                .Replace("NUM_HEMI_LIGHTS", parameters["numHemiLights"].ToString())
                .Replace("NUM_DIR_LIGHT_SHADOWS", parameters["numDirLightShadows"].ToString())
                .Replace("NUM_SPOT_LIGHT_SHADOWS", parameters["numSpotLightShadows"].ToString())
                .Replace("NUM_POINT_LIGHT_SHADOWS", parameters["numPointLightShadows"].ToString());
        }

        private string ReplaceClippingPlaneNums(string str, Hashtable parameters)
        {
            return str
                .Replace("NUM_CLIPPING_PLANES", parameters["numClippingPlanes"].ToString())
                .Replace("UNION_CLIPPING_PLANES", ((int)parameters["numClippingPlanes"]- (int)parameters["numClipIntersection"]).ToString());
        }

        private string includePattern = @"[ \t]*#include +<([\w\d./]+)>";

        private string ResolveIncludes(string source)
        {
            source = Regex.Replace(source, includePattern, IncludeReplacer);

            return source;
        }

        private string IncludeReplacer(Match match)
        {

            List<string> result = new List<string>();
            if (match.Groups.Count > 1)
            {
                for (int ctr = 1; ctr < match.Groups.Count; ctr++)
                {
                    string include = match.Groups[ctr].Value;
                    string source = renderer.ShaderLib.getChunk(include);
                    if (String.IsNullOrEmpty(source))
                    {
                        throw new Exception("Can not resolve #include<" + include + ">");
                    }
                    result.Add(ResolveIncludes(source));    
                }
                
            }

            return String.Join("\n", result);
        }

        private string deprecatedUnrollLoopPattern = @"#pragma unroll_loop[\s]+?for \( int i \= (\d+)\; i < (\d+)\; i \+\+ \) \{([\s\S]+?)(?=\})\}";
        private string unrollLoopPattern = @"#pragma unroll_loop_start\s+for\s*\(\s*int\s+i\s*=\s*(\d+)\s*;\s*i\s*<\s*(\d+)\s*;\s*i\s*\+\+\s*\)\s*{([\s\S]+?)}\s+#pragma unroll_loop_end";

        //private string loopPattern=@"#pragma unroll_loop[\s]+?for \( int i \= (\d+)\; i < (\d+)\; i \+\+ \) \{([\s\S]+?)(?=\})\}";

        private string UnrollLoops(string source)
        {
            source = Regex.Replace(source, unrollLoopPattern, LoopReplacer);
            source = Regex.Replace(source, deprecatedUnrollLoopPattern, LoopReplacer);
            return source;
        }

        private string LoopReplacer(Match match)
        {
            string result = "";
            
            if (match.Groups.Count > 1)
            {
                int start = Convert.ToInt32(match.Groups[1].Value);
                int end = Convert.ToInt32(match.Groups[2].Value);
                string snippet = match.Groups[3].Value;
                if (start == 0 && end == 0)
                {
                    result = "";
                }
                else
                {
                    for (int i = start; i < end; i++)
                    {
                        result += Regex.Replace(snippet, @"\[\s*i\s*\]", "[" + i + "]") 
                                .Replace(@"UNROLLED_LOOP_INDEX", i.ToString());
                    }
                }
            }

            return result;
        }

        private string GeneratePrecision(Hashtable parameters)
        {
            string precisionstring =
                "precision " + parameters["precision"] + " float; \n" +
                "precision " + parameters["precision"] + " int; \n";

            if (parameters["precision"].Equals("highp"))
            {
                precisionstring += @"#define HIGH_PRECISION";
            }
            else if (parameters["precision"].Equals("mediump"))
            {
                precisionstring += @"#define MEDIUM_PRECISION";
            }
            else if (parameters["precision"].Equals("lowp"))
            {
                precisionstring += @"#define LOW_PRECISION";
            }

            return precisionstring;

        }

        private string GenerateShadowMapTypeDefine(Hashtable parameters)
        {
            var shadowMapTypeDefine = "SHADOWMAP_TYPE_BASIC";

	        if ( (int)parameters["shadowMapType"] == Constants.PCFShadowMap ) {

		        shadowMapTypeDefine = "SHADOWMAP_TYPE_PCF";

	        } else if ( (int)parameters["shadowMapType"] == Constants.PCFSoftShadowMap ) {

		        shadowMapTypeDefine = "SHADOWMAP_TYPE_PCF_SOFT";

            }
            else if ((int)parameters["shadowMapType"] == Constants.VSMShadowMap)
            {

		        shadowMapTypeDefine = "SHADOWMAP_TYPE_VSM";

	        }

	        return shadowMapTypeDefine;
        }
        private string GenerateEnvMapTypeDefine(Hashtable parameters)
        {
            var envMapTypeDefine = "ENVMAP_TYPE_CUBE";

            if ((bool)parameters["envMap"])
            {
                int envMapMode = (int)parameters["envMapMode"];
                switch (envMapMode)
                {

                    case 301: //Constants.CubeReflectionMapping:
                    case 302: //Constants.CubeRefractionMapping:
                        envMapTypeDefine = "ENVMAP_TYPE_CUBE";
                        break;

                    case 306://Constants.CubeUVReflectionMapping:
                    case 307://Constants.CubeUVRefractionMapping:
                        envMapTypeDefine = "ENVMAP_TYPE_CUBE_UV";
                        break;

                    //case 303://Constants.EquirectangularReflectionMapping:
                    //case 304://Constants.EquirectangularRefractionMapping:
                    //    envMapTypeDefine = "ENVMAP_TYPE_EQUIREC";
                    //    break;

                    //case 305://Constants.SphericalReflectionMapping:
                    //    envMapTypeDefine = "ENVMAP_TYPE_SPHERE";
                    //    break;

                }

            }
            
            return envMapTypeDefine;
        }

        private string GenerateEnvMapModeDefine(Hashtable parameters)
        {
            var envMapModeDefine = "ENVMAP_MODE_REFLECTION";

	        if ( (bool)parameters["envMap"] ) {
                int envMapMode = (int)parameters["envMapMode"];
		        switch (envMapMode ) {

			        case 302://Constants.CubeRefractionMapping:
			        case 304://Constants.EquirectangularRefractionMapping:
				        envMapModeDefine = "ENVMAP_MODE_REFRACTION";
				        break;

		        }

	        }

	        return envMapModeDefine;
        }

        private string GenerateEnvMapBlendingDefine(Hashtable parameters)
        {
	        var envMapBlendingDefine = "ENVMAP_BLENDING_NONE";

	        if ( (bool)parameters["envMap"] ) {
                int? combine = (int?)parameters["combine"];
                if (combine != null)
                {
                    switch (combine)
                    {

                        case 0://Constants.MultiplyOperation:
                            envMapBlendingDefine = "ENVMAP_BLENDING_MULTIPLY";
                            break;

                        case 1://Constants.MixOperation:
                            envMapBlendingDefine = "ENVMAP_BLENDING_MIX";
                            break;

                        case 2://Constants.AddOperation:
                            envMapBlendingDefine = "ENVMAP_BLENDING_ADD";
                            break;

                    }
                }
	        }

	        return envMapBlendingDefine;
        }

        public GLUniforms GetUniforms()
        {
            if (cachedUniforms == null)
                cachedUniforms = new GLUniforms(program);

            return cachedUniforms;
        }

        public Hashtable GetAttributes()
        {
            if (cachedAttributes == null)
                cachedAttributes = FetchAttributeLocations(program);

            return cachedAttributes;
        }

        public override void Dispose()
        {
            if (!renderer.Context.IsDisposed)
            {
                GL.DeleteProgram(program);             
                this.program = 0;
            }
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
