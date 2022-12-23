using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace THREE
{

    public class GLPrograms
    {
        public List<GLProgram> Programs = new List<GLProgram>();

        private bool isGL2;

        private bool logarithmicDepthBuffer;

        private bool floatVertexTextures;

        private string precision;

        private int maxVertexUniforms;

        private bool vertexTextures;

        private Dictionary<string, string> shaderIDs = new Dictionary<string, string>();

        private List<string> parameterNames;

        private GLCapabilities capabilities;

        private GLRenderer renderer;

        private GLExtensions extensions;

        public ShaderLib ShaderLib = new ShaderLib();

        private GLCubeMap cubeMaps;

        private GLBindingStates bindingStates;

        private GLClipping clipping;
        public GLPrograms(GLRenderer renderer,GLCubeMap cubeMaps, GLExtensions extension, GLCapabilities capabilities,GLBindingStates bindingStates,GLClipping clipping)
        {
           
            this.maxVertexUniforms = capabilities.maxVertexUniforms;
            this.vertexTextures = capabilities.vertexTextures;

            this.capabilities = capabilities;
            this.renderer = renderer;
            this.extensions = extension;

            this.cubeMaps = cubeMaps;
            this.bindingStates = bindingStates;
            this.clipping = clipping;

            this.isGL2 = capabilities.IsGL2;
            this.logarithmicDepthBuffer = capabilities.logarithmicDepthBuffer;
            this.floatVertexTextures = capabilities.floatVertexTextures;

            this.precision = capabilities.precision;

            shaderIDs.Add("MeshDepthMaterial", "depth");
		    shaderIDs.Add("MeshDistanceMaterial", "distanceRGBA");
		    shaderIDs.Add("MeshNormalMaterial", "normal");
		    shaderIDs.Add("MeshBasicMaterial", "basic");
		    shaderIDs.Add("MeshLambertMaterial", "lambert");
		    shaderIDs.Add("MeshPhongMaterial", "phong");
		    shaderIDs.Add("MeshToonMaterial", "toon");
		    shaderIDs.Add("MeshStandardMaterial", "physical");
		    shaderIDs.Add("MeshPhysicalMaterial", "physical");
		    shaderIDs.Add("MeshMatcapMaterial", "matcap");
		    shaderIDs.Add("LineBasicMaterial", "basic");
		    shaderIDs.Add("LineDashedMaterial", "dashed");
		    shaderIDs.Add("PointsMaterial", "points");
		    shaderIDs.Add("ShadowMaterial", "shadow");
            shaderIDs.Add("SpriteMaterial", "sprite");

            parameterNames = new List<string>() {
                "precision", "isGL2", "supportsVertexTextures", "outputEncoding", "instancing", "instancingColor",
		        "map", "mapEncoding", "matcap", "matcapEncoding", "envMap", "envMapMode", "envMapEncoding", "envMapCubeUV",
		        "lightMap", "lightMapEncoding","aoMap", "emissiveMap", "emissiveMapEncoding", "bumpMap", "normalMap", "objectSpaceNormalMap", "tangentSpaceNormalMap", "clearcoatNormalMap", "displacementMap", "specularMap",
		        "roughnessMap", "metalnessMap", "gradientMap",
		        "alphaMap", "combine", "vertexColors", "vertexTangents", "vertexUvs", "uvsVertexOnly","fog", "useFog", "fogExp2",
		        "flatShading", "sizeAttenuation", "logarithmicDepthBuffer", "skinning",
		        "maxBones", "useVertexTexture", "morphTargets", "morphNormals",
		        "maxMorphTargets", "maxMorphNormals", "premultipliedAlpha",
		        "numDirLights", "numPointLights", "numSpotLights", "numHemiLights", "numRectAreaLights",
		        "numDirLightShadows", "numPointLightShadows", "numSpotLightShadows",
		        "shadowMapEnabled", "shadowMapType", "toneMapping", "physicallyCorrectLights",
		        "alphaTest", "doubleSided", "flipSided", "numClippingPlanes", "numClipIntersection", "depthPacking", "dithering",
		        "sheen","transmissionMap"};
                
        }

        private int GetMaxBones(SkinnedMesh skinnedMesh)
        {
            var skeleton = skinnedMesh.Skeleton;
            var bones = skeleton.Bones;

            if (this.floatVertexTextures)
            {
                return 1024;
            }
            else
            {
                // default for when object is not specified
                // ( for example when prebuilding shader to be used with multiple objects )
                //
                //  - leave some extra space for other uniforms
                //  - limit here is ANGLE's 254 max uniform vectors
                //    (up to 54 should be safe)

                var nVertexUniforms = maxVertexUniforms;
                var nVertexMatrices = (int)System.Math.Floor((float)((nVertexUniforms - 20) / 4));

                var maxBones = System.Math.Min(nVertexMatrices, bones.Length);

                if (maxBones < bones.Length)
                {
                    Trace.TraceWarning("THREE.Renderers.GLRenderer: Skeleton has" + bones.Length + " bones. This GPU supports " + maxBones + ".");
                    return 0;
                }

                return maxBones;
            }
        }
        private int GetTextureEncodingFromMap(Texture map)
        {
            int encoding = Constants.LinearEncoding;

            if (map != null)
            {
                encoding = Constants.LinearEncoding;
            }
            else if (map is Texture)
            {
                encoding = map.Encoding;
            }
            else if (map is GLRenderTarget)
            {
                //    Trace.TraceWarning("THREE.Renderers.gl.GLPrograms.GetTextureEncodingFromMap: don't use render targets as textures. Use their property instead.");
                encoding = (map as GLRenderTarget).Texture.Encoding;
            }

            return encoding;
        }
       
      
        public Hashtable GetParameter(Material material, GLLights lights, List<Light> shadows, Scene scene,Object3D object3D)
        {

            Fog fog = scene.Fog;

            Texture environment = material is MeshStandardMaterial ? scene.Environment : null;

            Texture envMap = cubeMaps.Get(material.EnvMap!=null?material.EnvMap : environment);

            string shaderID = "";
            if (shaderIDs.ContainsKey(material.type))
                shaderID = shaderIDs[material.type];

            int maxBones = object3D is SkinnedMesh ? GetMaxBones((SkinnedMesh)object3D) : 0;
                        
            if (!string.IsNullOrEmpty(material.Precision))
            {
                this.precision = this.capabilities.GetMaxPrecision(material.Precision);

                if (!this.precision.Equals(material.Precision))
                {
                    Trace.TraceWarning("THREE.Renderers.gl.GLPrograms.GetParameters:" + material.Precision + " not supported. using " + this.precision + " instead.");
                }
            }

            string vertexShader;
            string fragmentShader;

            if(!string.IsNullOrEmpty(shaderID))
            {
                GLShader shader = (GLShader)ShaderLib[shaderID];
                vertexShader = shader.VertexShader;
                fragmentShader = shader.FragmentShader;
            }
            else
            {
                vertexShader = (material as ShaderMaterial).VertexShader;
                fragmentShader = (material as ShaderMaterial).FragmentShader;
            }

            GLRenderTarget currentRenderTarget = this.renderer.GetRenderTarget();

            //int numMultiviewViews = currentRenderTarget != null && currentRenderTarget.IsGLMultiviewRenderTarget ? currentRenderTarget.NumViews : 0;

            Hashtable parameters = new Hashtable();
            parameters.Add("isGL2",isGL2);

            parameters.Add("shaderId",shaderID);

            parameters.Add("shaderName", material.type);

            parameters.Add("vertexShader", vertexShader);
            parameters.Add("fragmentShader", fragmentShader);
            parameters.Add("defines", material.Defines);
            parameters.Add("isRawShaderMaterial", material is RawShaderMaterial);
            parameters.Add("glslVersion", material.glslVersion);
            
            parameters.Add("precision",precision);

            parameters.Add("instancing", object3D is InstancedMesh ? true:false);
            parameters.Add("instancingColor", object3D is InstancedMesh && (object3D as InstancedMesh).InstanceColor != null);
            parameters.Add("supportsVertexTextures",vertexTextures);

            //parameters.Add("numMultiviewViews",numMultiviewViews);

            parameters.Add("outputEncoding", GetTextureEncodingFromMap(currentRenderTarget == null ? null : currentRenderTarget.Texture));
            
            parameters.Add("map",material.Map!=null);
            
            parameters.Add("mapEncoding",GetTextureEncodingFromMap(material.Map));
            
            parameters.Add("matcap", material is MeshMatcapMaterial ? (material as MeshMatcapMaterial).Matcap!=null:false);
            
            parameters.Add("matcapEncoding",material is MeshMatcapMaterial ? GetTextureEncodingFromMap((material as MeshMatcapMaterial).Matcap):Constants.LinearEncoding);
            
            parameters.Add("envMap", envMap!=null);
            
            parameters.Add("envMapMode", material.EnvMap!=null? material.EnvMap.Mapping :(int?)null);
            
            parameters.Add("envMapEncoding", GetTextureEncodingFromMap( material.EnvMap));
            
            parameters.Add("envMapCubeUV",material.EnvMap!=null && ( ( material.EnvMap.Mapping == Constants.CubeUVReflectionMapping ) || ( material.EnvMap.Mapping == Constants.CubeUVRefractionMapping )));
            
            parameters.Add("lightMap", material.LightMap!=null);
            
            parameters.Add("lightMapEncoding", GetTextureEncodingFromMap(material.LightMap));

            parameters.Add("aoMap", material.AoMap!=null);

            parameters.Add("emissiveMap", material.EmissiveMap!=null);

            parameters.Add("emissiveMapEncoding", GetTextureEncodingFromMap( material.EmissiveMap));

            parameters.Add("bumpMap",material.BumpMap!=null);

            parameters.Add("normalMap",material.NormalMap!=null);

            parameters.Add("objectSpaceNormalMap", material.NormalMapType == Constants.ObjectSpaceNormalMap);

            parameters.Add("tangentSpaceNormalMap", material.NormalMapType == Constants.TangentSpaceNormalMap);

            parameters.Add("clearcoatMap", material.ClearcoatMap != null);

            parameters.Add("clearcoatRoughnessMap", material.ClearcoatRoughnessMap != null);

            parameters.Add("clearcoatNormalMap", material.ClearcoatNormalMap!=null);
            
            parameters.Add("displacementMap", material.DisplacementMap!=null);
            
            parameters.Add("roughnessMap", material.RoughnessMap!=null);
            
            parameters.Add("metalnessMap", material.MetalnessMap!=null);
            
            parameters.Add("specularMap", material.SpecularMap!=null);

            parameters.Add("alphaMap",  material.AlphaMap!=null);

            parameters.Add("gradientMap",material.GradientMap!=null);

            parameters.Add("sheen", material.Sheen!=null);

            parameters.Add("combine", material.Combine != 0 ? material.Combine : (int?)null);

            parameters.Add("transmissionMap", material.TransmissionMap!=null);

                

            parameters.Add("vertexTangents", ( material.NormalMap!=null && material.VertexTangents ));
            
            parameters.Add("vertexColors", material.VertexColors);
            
            parameters.Add("vertexUvs", material.Map!=null || material.BumpMap!=null || material.NormalMap!=null || material.SpecularMap!=null || material.AlphaMap!=null || material.EmissiveMap!=null || material.RoughnessMap!=null || material.MetalnessMap!=null || material.ClearcoatNormalMap!=null);
            
            parameters.Add("uvsVertexOnly",!(material.Map!=null || material.BumpMap!=null || material.NormalMap!=null || material.SpecularMap!=null || material.AlphaMap!=null || material.EmissiveMap!=null || material.RoughnessMap!=null || material.MetalnessMap!=null || material.ClearcoatNormalMap!=null ) && material.DisplacementMap!=null);
            
            parameters.Add("fog", fog!=null);
            
            parameters.Add("useFog", material.Fog);
            
            parameters.Add("fogExp2", ( fog!=null && fog is FogExp2 ));
            
            parameters.Add("flatShading", material.FlatShading);
            
            parameters.Add("sizeAttenuation", material.SizeAttenuation);
            
            parameters.Add("logarithmicDepthBuffer", logarithmicDepthBuffer);
            
            parameters.Add("skinning", material.Skinning && maxBones > 0);
            
            parameters.Add("maxBones", maxBones);
            
            parameters.Add("useVertexTexture", floatVertexTextures);
            
            parameters.Add("morphTargets", material.MorphTargets);
            
            parameters.Add("morphNormals", material.MorphNormals);
            
            parameters.Add("maxMorphTargets", renderer.MaxMorphTargets);
            
            parameters.Add("maxMorphNormals", renderer.MaxMorphNormals);
            
            parameters.Add("numDirLights", lights.state["directional"] != null ? (lights.state["directional"] as Hashtable[]).Length : 0);
            
            parameters.Add("numPointLights", lights.state["point"] != null ? (lights.state["point"] as Hashtable[]).Length : 0);
            
            parameters.Add("numSpotLights", lights.state["spot"] != null ? (lights.state["spot"] as Hashtable[]).Length : 0);
            
            parameters.Add("numRectAreaLights", lights.state["rectArea"] != null ? (lights.state["rectArea"] as Hashtable[]).Length : 0);
            
            parameters.Add("numHemiLights", lights.state["hemi"] != null ? (lights.state["hemi"] as Hashtable[]).Length : 0);
            
            parameters.Add("numDirLightShadows", lights.state["directionalShadowMap"] != null ? (lights.state["directionalShadowMap"] as Texture[]).Length : 0);
            
            parameters.Add("numPointLightShadows", lights.state["pointShadowMap"] != null ? (lights.state["pointShadowMap"] as Texture[]).Length : 0);
            
            parameters.Add("numSpotLightShadows", lights.state["spotShadowMap"] != null ? (lights.state["spotShadowMap"] as Texture[]).Length : 0);
            
            parameters.Add("numClippingPlanes", clipping.numPlanes);
            parameters.Add("numClipIntersection", clipping.numIntersection);
            
            parameters.Add("dithering", material.Dithering);
            
            parameters.Add("shadowMapEnabled", renderer.ShadowMap.Enabled && shadows.Count > 0);
            parameters.Add("shadowMapType", renderer.ShadowMap.type);
            
            parameters.Add("toneMapping", material.ToneMapped ? renderer.ToneMapping : Constants.NoToneMapping);
            parameters.Add("physicallyCorrectLights", renderer.PhysicallyCorrectLights);
            
            parameters.Add("premultipliedAlpha", material.PremultipliedAlpha);
            
            parameters.Add("alphaTest", material.AlphaTest);
            parameters.Add("doubleSided", material.Side == Constants.DoubleSide);
            parameters.Add("flipSided", material.Side == Constants.BackSide);
            
            parameters.Add("depthPacking",material is MeshDepthMaterial ? (material as MeshDepthMaterial).DepthPacking : 0);
            
            parameters.Add("indexOfAttributeName", material.IndexOAttributeName);

            parameters.Add("extensionDerivatives", (material is ShaderMaterial) && (material as ShaderMaterial).extensions.derivatives);

            parameters.Add("extensionFragDepth", (material is ShaderMaterial) && (material as ShaderMaterial).extensions.fragDepth);

            parameters.Add("extensionDrawBuffers", (material is ShaderMaterial) && (material as ShaderMaterial).extensions.drawBuffers);

            parameters.Add("extensionShaderTextureLOD", (material is ShaderMaterial) && (material as ShaderMaterial).extensions.shaderTextureLOD);

            parameters.Add("rendererExtensionFragDepth", isGL2 || extensions.Get("EXT_frag_depth") > -1);


            parameters.Add("rendererExtensionDrawBuffers", isGL2 || extensions.Get("GL_draw_buffers") > -1);

            parameters.Add("rendererExtensionShaderTextureLOD", isGL2 || extensions.Get("EXT_shader_texture_lod") > -1);

            parameters.Add("customProgramCacheKey", material.customProgramCacheKey);

            return parameters;
        }
        public string getProgramCacheKey(Material material, Hashtable parameters)
        {
            var array = new List<string>();

            if (!string.IsNullOrEmpty((string)parameters["shaderId"]))
            {
                array.Add((string)parameters["shaderId"]);
            }
            else
            {
                array.Add((material as ShaderMaterial).FragmentShader);
                array.Add((material as ShaderMaterial).VertexShader);
            }

            if (material.Defines != null)
            {
                foreach (DictionaryEntry entry in material.Defines)
                {
                    array.Add((string)entry.Key);
                    if (entry.Value is string)
                        array.Add((string)entry.Value);
                    else
                        array.Add(Convert.ToString(entry.Value));
                }

            }

            if ((bool)parameters["isRawShaderMaterial"] == false)
            {
                for (int i = 0; i < parameterNames.Count; i++)
                {
                    object obj = parameters[parameterNames[i]];
                    string parameterKey = obj != null ? obj.ToString() : "";
                    array.Add(parameterKey);
                }
            }

            //array.Add(material.OnBeforeCompile.ToString());
            array.Add(renderer.outputEncoding.ToString());
            array.Add(renderer.GammaFactor.ToString());


            return string.Join(",", array);
        }
        public GLUniforms GetUniforms(Material material)
        {
            string shaderId = "";

            if (shaderIDs.ContainsKey(material.type))
                shaderId = shaderIDs[material.type];



            GLUniforms uniforms;

            if (!string.IsNullOrEmpty(shaderId))
            {
                GLShader shader = (GLShader)ShaderLib[shaderId];
                uniforms = (GLUniforms)UniformsUtils.CloneUniforms(shader.Uniforms);
            }
            else
            {
                uniforms = (material as ShaderMaterial).Uniforms;
            }

            return uniforms;
        }
        public GLProgram AcquireProgram(Hashtable parameters, string cacheKey)
        {
            GLProgram program = null;
            for (var p = 0; p < Programs.Count; p++)
            {
                var programInfo = Programs[p];

                if (programInfo.Code.Equals(cacheKey))
                {
                    program = programInfo;
                    ++program.UsedTimes;
                    break;
                }
            }

            if (program == null)
            {
                program = new GLProgram(renderer, cacheKey, parameters, bindingStates);
                Programs.Add(program);
            }

            return program;
        }
       

       

        public void ReleaseProgram(GLProgram program)
        {
            if (--program.UsedTimes == 0)
            {
                var i = Programs.IndexOf(program);
                if(i>=0)
                    Programs.RemoveAt(i);
            }
        }

       


    }
}
