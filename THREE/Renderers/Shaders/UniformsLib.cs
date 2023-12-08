using System.Collections.Generic;
using System.Runtime.Serialization;

namespace THREE
{
    [Serializable]
    public class UniformsLib : Dictionary<string, Uniforms>
    {
        public static Texture LTC_FLOAT_1;
        public static Texture LTC_FLOAT_2;
        public UniformsLib()
        {
            this.Add("common", this.Common());
            this.Add("specularmap", this.SpecularMap());
            this.Add("envmap", this.EnvMap());
            this.Add("aomap", this.AoMap());
            this.Add("lightmap", this.LightMap());
            this.Add("emissivemap", this.EmissiveMap());
            this.Add("bumpmap", this.BumpMap());
            this.Add("normalmap", this.NormalMap());
            this.Add("displacementmap", this.DisplacementMap());
            this.Add("roughnessmap", this.RoughnessMap());
            this.Add("metalnessmap", this.MetalnessMap());
            this.Add("gradientmap", this.GradientMap());
            this.Add("fog", this.Fog());
            this.Add("lights", this.Lights());
            this.Add("points", this.Points());
            this.Add("sprite", this.Sprite());
        }

        public UniformsLib(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private Uniforms Common()
        {
            return new Uniforms
            {
                { "diffuse",            new Uniform {{"value", new Color().SetHex(0xeeeeee)}}},
                { "opacity",            new Uniform {{"value", 1.0f}}},

                { "map",                new Uniform {{"value", null}}},
                { "uvTransform",        new Uniform {{"value",new Matrix3()}}},
                { "uvTransform2",        new Uniform {{"value",new Matrix3()}}},

                { "alphaMap",           new Uniform {{"value", null}}},
            };
        }

        private Uniforms SpecularMap()
        {
            return new Uniforms
            {
                { "specularMap",        new Uniform {{"value",null}}}
            };
        }

        private Uniforms EnvMap()
        {
            return new Uniforms
            {
                { "envMap",             new Uniform {{"value",null}}},
                { "flipEnvMap",         new Uniform {{"value",-1}}},
                { "reflectivity",       new Uniform {{"value",1.0f}}},
                { "refractionRatio",    new Uniform {{"value",0.98f}}},
                { "maxMipLevel",        new Uniform {{"value",0}}}
            };
        }

        private Uniforms AoMap()
        {
            return new Uniforms
            {
                { "aoMap",              new Uniform {{"value",null}}},
                { "aoMapIntensity",     new Uniform {{"value",1}}}
            };
        }

        private Uniforms LightMap()
        {
            return new Uniforms
            {
                { "lightMap",           new Uniform {{"value",null}}},
                { "lightMapIntensity",  new Uniform {{"value",1}}}
            };
        }

        private Uniforms EmissiveMap()
        {
            return new Uniforms
            {
                { "emissiveMap",        new Uniform {{"value",null}}}
            };
        }

        private Uniforms BumpMap()
        {

            return new Uniforms
            {
                { "bumpMap",            new Uniform {{"value",null }}},
                { "bumpScale",          new Uniform {{"value",1}}}
            };

        }

        private Uniforms NormalMap()
        {
            return new Uniforms
            {
                {"normalMap",           new Uniform {{"value", null }}},
                {"normalScale",         new Uniform {{"value", new Vector2( 1, 1 ) }}}
            };
        }

        private Uniforms DisplacementMap()
        {
            return new Uniforms
            {
                {"displacementMap",     new Uniform {{ "value", null }}},
                {"displacementScale",   new Uniform {{ "value", 1 }}},
                {"displacementBias",    new Uniform {{ "value", 0 }}}
            };

        }

        private Uniforms RoughnessMap()
        {
            return new Uniforms
            {

                {"roughnessMap",        new Uniform {{ "value", null }}}
            };

        }

        private Uniforms MetalnessMap()
        {
            return new Uniforms
            {
                {"metalnessMap",        new Uniform {{ "value", null }}}
            };

        }

        private Uniforms GradientMap()
        {
            return new Uniforms
            {
                {"gradientMap",         new Uniform {{"value",null }}}
            };
        }

        private Uniforms Fog()
        {
            return new Uniforms
            {
                {"fogDensity",          new Uniform {{ "value", 0.00025 }}},
                {"fogNear",             new Uniform {{ "value", 1 }}},
                {"fogFar",              new Uniform {{ "value", 2000 }}},
                {"fogColor",            new Uniform {{ "value", new Color().SetHex(0xffffff)}}}
            };

        }

        private Uniforms Lights()
        {

            return new Uniforms
            {
                {"ambientLightColor",       new Uniform{{ "value",      new List<Color>()}}},

                {"lightProbe",              new Uniform{{ "value",      new List<Light>()}}},

                {"directionalLights",       new Uniform
                                            {
                                                    { "value",      new List<Light>()},
                                                    { "properties", new Uniform()
                                                                        {
                                                                            { "direction",      new Uniform()},
                                                                            { "color",          new Uniform()},
                                                                            { "shadow",         new Uniform()},
                                                                            { "shadowBias",     new Uniform()},
                                                                            { "shadowRadius",   new Uniform()},
                                                                            { "shadowMapSize",  new Uniform()}
                                                                        }
                                                    }
                                            }
                },
                {"directionalLightShadows",       new Uniform
                                            {
                                                    { "value",      new List<LightShadow>()},
                                                    { "properties", new Uniform()
                                                                        {

                                                                            { "shadowBias",     new Uniform()},
                                                                            {"shadowNormalBias",new Uniform() },
                                                                            { "shadowRadius",   new Uniform()},
                                                                            { "shadowMapSize",  new Uniform()}
                                                                        }
                                                    }
                                            }
                },

                {"directionalShadowMap",    new Uniform{{ "value",      new List<Texture>() }}},
                {"directionalShadowMatrix", new Uniform{{ "value",      new List<Matrix4>() }}},

                {"spotLights",              new Uniform
                                            {
                                                { "value",      new List<Light>()},
                                                { "properties", new Uniform()
                                                                    {
                                                                            { "color",          new Uniform()},
                                                                            { "position",       new Uniform()},
                                                                            { "direction",      new Uniform()},
                                                                            { "distance",       new Uniform()},
                                                                            { "coneCos",        new Uniform()},
                                                                            { "penumbraCos",    new Uniform()},
                                                                            { "decay",          new Uniform()}
                                                                    }
                                                }
                                            }
                },

                 {"spotLightShadows",       new Uniform
                                            {
                                                    { "value",      new List<LightShadow>()},
                                                    { "properties", new Uniform()
                                                                        {

                                                                            { "shadowBias",     new Uniform()},
                                                                            { "shadowNormalBias",new Uniform() },
                                                                            { "shadowRadius",   new Uniform()},
                                                                            { "shadowMapSize",  new Uniform()}
                                                                        }
                                                    }
                                            }
                },
                { "spotShadowMap",          new Uniform{{ "value",      new List<Texture>()}}},
                { "spotShadowMatrix",       new Uniform{{ "value",      new List<Matrix4>()}}},

                { "pointLights",            new Uniform
                                            {
                                                { "value",      new List<Light>()},
                                                { "properties", new Uniform()
                                                                    {
                                                                            {"color",           new Uniform()},
                                                                            {"position",        new Uniform()},
                                                                            {"decay",           new Uniform()},
                                                                            {"distance",        new Uniform()}
                                                                    }
                                                }
                                            }
                },

                {"pointLightShadows",       new Uniform
                                            {
                                                    { "value",      new List<LightShadow>()},
                                                    { "properties", new Uniform()
                                                                        {

                                                                            { "shadowBias",     new Uniform()},
                                                                            {"shadowNormalBias",new Uniform() },
                                                                            { "shadowRadius",   new Uniform()},
                                                                            { "shadowMapSize",  new Uniform()},
                                                                            {"shadowCameraNear",new Uniform()},
                                                                            {"shadowCameraFar", new Uniform()}
                                                                        }
                                                    }
                                            }
                },
                { "pointShadowMap",         new Uniform{{ "value",      new List<Texture>()}}},
                { "pointShadowMatrix",      new Uniform{{ "value",      new List<Matrix4>()}}},

                {"hemisphereLights",        new Uniform
                                            {
                                                { "value",      new List<Light>()},
                                                { "properties", new Uniform
                                                                        {
                                                                            { "direction",      new Uniform()},
                                                                            { "skyColor",       new Uniform()},
                                                                            { "groundColor",    new Uniform()}
                                                                        }
                                                }
                                            }
                },

		        // TODO (abelnation): RectAreaLight BRDF data needs to be moved from example to main src
		        {"rectAreaLights",          new Uniform
                                            {
                                                { "value",      new List<Light>()},
                                                { "properties", new Uniform
                                                                {
                                                                    {"color",       new Uniform()},
                                                                    {"position",    new Uniform()},
                                                                    {"width",       new Uniform()},
                                                                    {"height",      new Uniform()}
                                                                }
                                                }
                                            }

                },
                {"ltc_1", new Uniform{{ "value", null}}},
                {"ltc_2", new Uniform{{ "value", null}}}
            };
        }

        private Uniforms Points()
        {
            return new Uniforms{
                {"diffuse",     new Uniform{{"value", new Color().SetHex(0xeeeeee) }}},
                {"opacity",     new Uniform{{"value", 1.0f }}},
                {"size",        new Uniform{{"value", 1.0f }}},
                {"scale",       new Uniform{{"value", 1.0f }}},
                {"map",         new Uniform{{"value", null}}},
                {"alphaMap",    new Uniform{{"value", null}}},
                {"uvTransform", new Uniform{{"value", new Matrix3()}}}
            };
        }

        private Uniforms Sprite()
        {
            return new Uniforms {
                {"diffuse",         new Uniform{{ "value", new Color().SetHex( 0xeeeeee ) }}},
                {"opacity",         new Uniform{{ "value", 1.0f }}},
                {"center",          new Uniform{{ "value", new Vector2( 0.5f, 0.5f ) }}},
                {"rotation",        new Uniform{{ "value", 0.0f }}},
                {"map",             new Uniform{{ "value", null }}},
                {"alphaMap",        new Uniform{{ "value", null }}},
                {"uvTransform",     new Uniform{{ "value", new Matrix3() }}}
            };
        }
    }
}
