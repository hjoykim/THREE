using System.Collections.Generic;

namespace THREE
{
    public class UniformsLib : Dictionary<string, GLUniforms>
    {
        public static Texture LTC_FLOAT_1;
        public static Texture LTC_FLOAT_2;
        public UniformsLib()
        {
            this.Add("common",          this.Common());
            this.Add("specularmap",     this.SpecularMap());
            this.Add("envmap",          this.EnvMap());
            this.Add("aomap",           this.AoMap());
            this.Add("lightmap",        this.LightMap());
            this.Add("emissivemap",     this.EmissiveMap());
            this.Add("bumpmap",         this.BumpMap());
            this.Add("normalmap",       this.NormalMap());
            this.Add("displacementmap", this.DisplacementMap());
            this.Add("roughnessmap",    this.RoughnessMap());
            this.Add("metalnessmap",    this.MetalnessMap());
            this.Add("gradientmap",     this.GradientMap());
            this.Add("fog",             this.Fog());
            this.Add("lights",          this.Lights());
            this.Add("points",          this.Points());
            this.Add("sprite",          this.Sprite());
        }

        private GLUniforms Common()
        {
            return new GLUniforms
            {
                { "diffuse",            new GLUniform {{"value", new Color().SetHex(0xeeeeee)}}},            
                { "opacity",            new GLUniform {{"value", 1.0f}}},

                { "map",                new GLUniform {{"value", null}}},
                { "uvTransform",        new GLUniform {{"value",new Matrix3()}}},
                { "uvTransform2",        new GLUniform {{"value",new Matrix3()}}},

                { "alphaMap",           new GLUniform {{"value", null}}},
            };
        }

        private GLUniforms SpecularMap()
        {
            return new GLUniforms
            {
                { "specularMap",        new GLUniform {{"value",null}}}
            };
        }

        private GLUniforms EnvMap()
        {
            return new GLUniforms
            {
                { "envMap",             new GLUniform {{"value",null}}},
                { "flipEnvMap",         new GLUniform {{"value",-1}}},
                { "reflectivity",       new GLUniform {{"value",1.0f}}},
                { "refractionRatio",    new GLUniform {{"value",0.98f}}},
                { "maxMipLevel",        new GLUniform {{"value",0}}}
            };
        }

        private GLUniforms AoMap()
        {
            return new GLUniforms
            {
                { "aoMap",              new GLUniform {{"value",null}}},
                { "aoMapIntensity",     new GLUniform {{"value",1}}}
            };
        }

        private GLUniforms LightMap()
        {
            return new GLUniforms
            {
                { "lightMap",           new GLUniform {{"value",null}}},
                { "lightMapIntensity",  new GLUniform {{"value",1}}}
            };
        }

        private GLUniforms EmissiveMap()
        {
            return new GLUniforms
            {
                { "emissiveMap",        new GLUniform {{"value",null}}}
            };
        }

        private GLUniforms BumpMap()
        {

            return new GLUniforms 
            {
                { "bumpMap",            new GLUniform {{"value",null }}},
		        { "bumpScale",          new GLUniform {{"value",1}}}
            };

        }

        private GLUniforms NormalMap()
        {
            return new GLUniforms
            {
		        {"normalMap",           new GLUniform {{"value", null }}},
                {"normalScale",         new GLUniform {{"value", new Vector2( 1, 1 ) }}}
            };
        }

        private GLUniforms DisplacementMap()
        {
            return new GLUniforms
            {
		        {"displacementMap",     new GLUniform {{ "value", null }}},
                {"displacementScale",   new GLUniform {{ "value", 1 }}},
		        {"displacementBias",    new GLUniform {{ "value", 0 }}}
            };

        }

        private GLUniforms RoughnessMap()
        {
            return new GLUniforms
            {

		        {"roughnessMap",        new GLUniform {{ "value", null }}}
            };

        }

        private GLUniforms MetalnessMap()
        {
            return new GLUniforms 
            {
		        {"metalnessMap",        new GLUniform {{ "value", null }}}
            };

        }

        private GLUniforms GradientMap()
        {
            return new GLUniforms 
            {
		        {"gradientMap",         new GLUniform {{"value",null }}}
            };
        }

        private GLUniforms Fog()
        {
            return new GLUniforms
            {
		        {"fogDensity",          new GLUniform {{ "value", 0.00025 }}},
		        {"fogNear",             new GLUniform {{ "value", 1 }}},
                {"fogFar",              new GLUniform {{ "value", 2000 }}},
                {"fogColor",            new GLUniform {{ "value", new Color().SetHex(0xffffff)}}}
            };

        }

        private GLUniforms Lights()
        {

            return new GLUniforms
            {
                {"ambientLightColor",       new GLUniform{{ "value",      new List<Color>()}}},

		        {"lightProbe",              new GLUniform{{ "value",      new List<Light>()}}},

		        {"directionalLights",       new GLUniform
                                            {
                                                    { "value",      new List<Light>()}, 
                                                    { "properties", new GLUniform()
                                                                        {
                                                                            { "direction",      new GLUniform()},
			                                                                { "color",          new GLUniform()},
                                        			                        { "shadow",         new GLUniform()},
			                                                                { "shadowBias",     new GLUniform()},
			                                                                { "shadowRadius",   new GLUniform()},
			                                                                { "shadowMapSize",  new GLUniform()}
		                                                                } 
                                                    }
                                            }
                },
                {"directionalLightShadows",       new GLUniform
                                            {
                                                    { "value",      new List<LightShadow>()},
                                                    { "properties", new GLUniform()
                                                                        {
                                                                          
                                                                            { "shadowBias",     new GLUniform()},
                                                                            {"shadowNormalBias",new GLUniform() },
                                                                            { "shadowRadius",   new GLUniform()},
                                                                            { "shadowMapSize",  new GLUniform()}
                                                                        }
                                                    }
                                            }
                },

                {"directionalShadowMap",    new GLUniform{{ "value",      new List<Texture>() }}},
		        {"directionalShadowMatrix", new GLUniform{{ "value",      new List<Matrix4>() }}},

		        {"spotLights",              new GLUniform
                                            {
                                                { "value",      new List<Light>()},
                                                { "properties", new GLUniform()
                                                                    {
			                                                                { "color",          new GLUniform()},
			                                                                { "position",       new GLUniform()},
			                                                                { "direction",      new GLUniform()},
			                                                                { "distance",       new GLUniform()},
			                                                                { "coneCos",        new GLUniform()},
			                                                                { "penumbraCos",    new GLUniform()},
			                                                                { "decay",          new GLUniform()}			                                                              
                                                                    }
                                                }
                                            }
                },

                 {"spotLightShadows",       new GLUniform
                                            {
                                                    { "value",      new List<LightShadow>()},
                                                    { "properties", new GLUniform()
                                                                        {

                                                                            { "shadowBias",     new GLUniform()},
                                                                            { "shadowNormalBias",new GLUniform() },
                                                                            { "shadowRadius",   new GLUniform()},
                                                                            { "shadowMapSize",  new GLUniform()}
                                                                        }
                                                    }
                                            }
                },
                { "spotShadowMap",          new GLUniform{{ "value",      new List<Texture>()}}},
		        { "spotShadowMatrix",       new GLUniform{{ "value",      new List<Matrix4>()}}},

		        { "pointLights",            new GLUniform
                                            {
                                                { "value",      new List<Light>()}, 
                                                { "properties", new GLUniform()
                                                                    {
			                                                                {"color",           new GLUniform()},
			                                                                {"position",        new GLUniform()},
			                                                                {"decay",           new GLUniform()},
			                                                                {"distance",        new GLUniform()}			                                                               
                                                                    }
		                                        }
                                            }
                },

                {"pointLightShadows",       new GLUniform
                                            {
                                                    { "value",      new List<LightShadow>()},
                                                    { "properties", new GLUniform()
                                                                        {

                                                                            { "shadowBias",     new GLUniform()},
                                                                            {"shadowNormalBias",new GLUniform() },
                                                                            { "shadowRadius",   new GLUniform()},
                                                                            { "shadowMapSize",  new GLUniform()},
                                                                            {"shadowCameraNear",new GLUniform()},
                                                                            {"shadowCameraFar", new GLUniform()}
                                                                        }
                                                    }
                                            }
                },
                { "pointShadowMap",         new GLUniform{{ "value",      new List<Texture>()}}},
		        { "pointShadowMatrix",      new GLUniform{{ "value",      new List<Matrix4>()}}},

		        {"hemisphereLights",        new GLUniform
                                            {
                                                { "value",      new List<Light>()},
                                                { "properties", new GLUniform
                                                                        {
                                                                            { "direction",      new GLUniform()},
			                                                                { "skyColor",       new GLUniform()},
			                                                                { "groundColor",    new GLUniform()}
                                                                        }
                                                }
                                            }
                },

		        // TODO (abelnation): RectAreaLight BRDF data needs to be moved from example to main src
		        {"rectAreaLights",          new GLUniform
                                            {
                                                { "value",      new List<Light>()}, 
                                                { "properties", new GLUniform
                                                                {
			                                                        {"color",       new GLUniform()},
			                                                        {"position",    new GLUniform()},
			                                                        {"width",       new GLUniform()},
			                                                        {"height",      new GLUniform()}
		                                                        } 
                                                }
                                            }

        	    },
                {"ltc_1", new GLUniform{{ "value", null}}},
                {"ltc_2", new GLUniform{{ "value", null}}}
            };
        }
      
        private GLUniforms Points()
        {
            return new GLUniforms{
		        {"diffuse",     new GLUniform{{"value", new Color().SetHex(0xeeeeee) }}},
		        {"opacity",     new GLUniform{{"value", 1.0f }}},
		        {"size",        new GLUniform{{"value", 1.0f }}},
		        {"scale",       new GLUniform{{"value", 1.0f }}},
		        {"map",         new GLUniform{{"value", null}}},
		        {"alphaMap",    new GLUniform{{"value", null}}},
		        {"uvTransform", new GLUniform{{"value", new Matrix3()}}}
            };
        }

        private GLUniforms Sprite()
        {
            return new GLUniforms {
		        {"diffuse",         new GLUniform{{ "value", new Color().SetHex( 0xeeeeee ) }}},
		        {"opacity",         new GLUniform{{ "value", 1.0f }}},
                {"center",          new GLUniform{{ "value", new Vector2( 0.5f, 0.5f ) }}},
                {"rotation",        new GLUniform{{ "value", 0.0f }}},
                {"map",             new GLUniform{{ "value", null }}},
                {"alphaMap",        new GLUniform{{ "value", null }}},
                {"uvTransform",     new GLUniform{{ "value", new Matrix3() }}}
	        };
        }
    }
}
