using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class UniformsCache : Hashtable
    {
        public UniformsCache()
            : base()
        {
            this.Add("DirectionalLight", new Hashtable()
                {
                    {"direction", Vector3.Zero()},
                    {"color",new Color()}
                });

            this.Add("SpotLight", new Hashtable()
                {
                    {"position",Vector3.Zero()},
                    {"direction",Vector3.Zero()},
                    {"color",new Color()},
                    {"distance",0.0f},
                    {"coneCos",0.0f},
                    {"penumbraCos",0.0f},
                    {"decay",0.0f}
                });

            this.Add("PointLight", new Hashtable()
                {
                    {"position", Vector3.Zero()},
                    {"color", new Color()},
                    {"distance", 0},
                    {"decay", 0}
                });

            this.Add("HemisphereLight", new Hashtable()
                {
                    {"direction", Vector3.Zero()},
                    {"skyColor", new Color()},
                    {"groundColor", new Color()}
                });

            this.Add("RectAreaLight", new Hashtable()
                {
                    {"color", new Color()},
                    {"position", Vector3.Zero()},
                    {"halfWidth", Vector3.Zero()},
                    {"halfHeight", Vector3.Zero()}
					// TODO (abelnation): set RectAreaLight shadow uniforms
				});

        }
    }
    public class ShadowUniformsCache : Hashtable
    {
        public ShadowUniformsCache()
            : base()
        {
            this.Add("DirectionalLight", new Hashtable()
                {
                    {"shadowBias",0.0f},
                    {"shadowNormalBias",0.0f },
                    {"shadowRadius",1.0f},
                    {"shadowMapSize",Vector2.Zero()}
                });

            this.Add("SpotLight", new Hashtable()
                { 
                    {"shadowBias",0.0f},
                    {"shadowNormalBias",0.0f },
                    {"shadowRadius",1.0f},
                    {"shadowMapSize",Vector2.Zero()}
                });

            this.Add("PointLight", new Hashtable()
                { 
                    {"shadowBias", 0},
                    {"shadowNormalBias",0.0f },
                    {"shadowRadius", 1},
                    {"shadowMapSize", Vector2.Zero()},
                    {"shadowCameraNear", 1},
                    {"shadowCameraFar", 1000}
                });


        }
    }
    public class GLLights
    {
        private UniformsCache cache = new UniformsCache();
        private ShadowUniformsCache shadowCache = new ShadowUniformsCache();
        public Hashtable state;

        private int nextVersion = 0;

        private Vector3 vector3 = Vector3.Zero();
        private Matrix4 matrix4 = Matrix4.Identity();
        private Quaternion quaternion = Quaternion.Identity();
        private Matrix4 matrix42 = Matrix4.Identity();

        public GLLights()
        {
            state = new Hashtable()
                    {
                        {"version", 0},

                        {"hash", new Hashtable()
                            {
                                {"directionalLength", -1},
                                {"pointLength", -1},
                                {"spotLength", -1},
                                {"rectAreaLength", - 1},
                                {"hemiLength", - 1},

                                {"numDirectionalShadows", - 1},
                                {"numPointShadows", - 1},
                                {"numSpotShadows", - 1},
                             }
                        },

                        {"ambient", new float[]{ 0, 0, 0 }},
                        {"probe", null},
                        {"directional", null},
                        {"directionalShadow",null },
                        {"directionalShadowMap", null},
                        {"directionalShadowMatrix", null},
                        {"spot", null},
                        {"spotShadow",null },
                        {"spotShadowMap", null},
                        {"spotShadowMatrix", null},
                        {"rectArea", null},
                        {"rectAreaLTC1",null },
                        {"rectAreaLTC2",null },
                        {"point", null},
                        {"pointShadow",null },
                        {"pointShadowMap", null},
                        {"pointShadowMatrix", null},
                        {"hemi", null},

                        {"numDirectionalShadows", - 1},
                        {"numPointShadows", - 1},
                        {"numSpotShadows", - 1}
                    };

            Vector3 Zero = Vector3.Zero();
            state["probe"] = new Vector3[] { Zero, Zero, Zero, Zero, Zero, Zero, Zero, Zero, Zero };
        }

        public void Setup(List<Light> lights, Camera camera)
        {

            Color ambientColor = Color.Hex(0x000000);
            int directionalLength = 0;

            var pointLength = 0;
            var spotLength = 0;
            var rectAreaLength = 0;
            var hemiLength = 0;

            var numDirectionalShadows = 0;
            var numPointShadows = 0;
            var numSpotShadows = 0;

            var viewMatrix = camera.MatrixWorldInverse;

            lights.Sort(delegate (Light lightA, Light lightB)
            {
                return (lightB.CastShadow ? 1 : 0) - (lightA.CastShadow ? 1 : 0);
            });

            List<Hashtable> directionalShadowList = new List<Hashtable>();

            List<Texture> directionalShadowMapList = new List<Texture>();

            List<Matrix4> directionalShadowMatrixList = new List<Matrix4>();

            List<Hashtable> directionalList = new List<Hashtable>();

            List<Hashtable> spotShadowList = new List<Hashtable>();

            List<Texture> spotShadowMapList = new List<Texture>();

            List<Matrix4> spotShadowMatrixList = new List<Matrix4>();

            List<Hashtable> spotList = new List<Hashtable>();


            List<Hashtable> rectAreaList = new List<Hashtable>();

            List<Hashtable> pointShadowList = new List<Hashtable>();

            List<Texture> pointShadowMapList = new List<Texture>();

            List<Matrix4> pointShadowMatrixList = new List<Matrix4>();

            List<Hashtable> pointList = new List<Hashtable>();

            List<Hashtable> hemiList = new List<Hashtable>();

            for (int i = 0; i < lights.Count; i++)
            {
                var light = lights[i];

                var color = light.Color;
                var intensity = light.Intensity;
                var distance = light.Distance;

                var shadowMap = (light.Shadow != null && light.Shadow.Map != null) ? light.Shadow.Map.Texture : null;


                if (light is AmbientLight)
                {
                    color.MultiplyScalar(intensity);
                    ambientColor.Add(color);
                }
                else if (light is LightProbe)
                {
                    Vector3[] probe = (Vector3[])state["probe"];
                    for (int j = 0; j < 9; j++)
                    {
                        probe[j] = light.sh.Coefficients[j] * intensity;
                    }
                }
                else if (light is DirectionalLight)
                {
                    Hashtable uniforms = (Hashtable)(cache[light.type] as Hashtable).Clone();
                    Color lightColor = light.Color;

                    uniforms["color"] = lightColor.MultiplyScalar(light.Intensity);

                    Vector3 direction = Vector3.Zero().SetFromMatrixPosition(light.MatrixWorld);

                    vector3.SetFromMatrixPosition(light.Target.MatrixWorld);

                    direction.Sub(vector3);

                    direction.TransformDirection(viewMatrix);

                    uniforms["direction"] = direction;                  
                   
                  
                    //uniforms["shadow"] = light.CastShadow;

                    if (light.CastShadow)
                    {
                        var shadow = light.Shadow;
                        Hashtable shadowUniforms = (Hashtable)(shadowCache[light.type] as Hashtable).Clone();
                        shadowUniforms["shadowBias"] = shadow.Bias;
                        shadowUniforms["shadowNormalBias"] = shadow.NormalBias;
                        shadowUniforms["shadowRadius"] = shadow.Radius;
                        shadowUniforms["shadowMapSize"] = shadow.MapSize;

                        directionalShadowList.Add(shadowUniforms);
                        directionalShadowMapList.Add(shadowMap);
                        directionalShadowMatrixList.Add(light.Shadow.Matrix);

                        //state["directionalShadowMap"] = shadowMapList;
                        //state["directionalShadowMatrix"] = shadowMatrixList;
                        //state["directional"]
                        numDirectionalShadows++;
                    }
                    directionalList.Add(uniforms);
                    directionalLength++;
                }
                else if (light is SpotLight)
                {
                    Hashtable uniforms = (Hashtable)(cache[light.type] as Hashtable).Clone();
                    //Hashtable uniforms = (Hashtable)cache[light.type];

                    Vector3 position = Vector3.Zero().SetFromMatrixPosition(light.MatrixWorld);
                    position.ApplyMatrix4(viewMatrix);
                    uniforms["position"] = position;

                    Color lightColor = light.Color;

                    uniforms["color"] = lightColor.MultiplyScalar(light.Intensity);
                    uniforms["distance"] = distance;

                    Vector3 direction = Vector3.Zero().SetFromMatrixPosition(light.MatrixWorld);

                    vector3.SetFromMatrixPosition(light.Target.MatrixWorld);

                    direction.Sub(vector3);

                    direction.TransformDirection(viewMatrix);

                    uniforms["direction"] = direction;

                    uniforms["coneCos"] = (float)System.Math.Cos(light.Angle);
                    uniforms["penumbraCos"] = (float)System.Math.Cos(light.Angle * (1 - light.Penumbra));
                    uniforms["decay"] = light.Decay;

                    

                    if (light.CastShadow)
                    {
                        var shadow = light.Shadow;

                        Hashtable shadowUniforms = (Hashtable)(shadowCache[light.type] as Hashtable).Clone();

                        shadowUniforms["shadowBias"] = shadow.Bias;
                        shadowUniforms["shadowNormalBias"] = shadow.NormalBias;
                        shadowUniforms["shadowRadius"] = shadow.Radius;
                        shadowUniforms["shadowMapSize"] = shadow.MapSize;

                        spotShadowList.Add(shadowUniforms);
                        spotShadowMapList.Add(shadowMap);
                        spotShadowMatrixList.Add(light.Shadow.Matrix);

                        numSpotShadows++;
                    }
                    spotList.Add(uniforms);
                    spotLength++;
                }
                else if (light is RectAreaLight)
                {
                    Hashtable uniforms = (Hashtable)(cache[light.type] as Hashtable).Clone();

                    Color lightColor = light.Color;
                    uniforms["color"] = lightColor.MultiplyScalar(light.Intensity);

                    Vector3 position = Vector3.Zero().SetFromMatrixPosition(light.MatrixWorld);
                    position.ApplyMatrix4(viewMatrix);
                    uniforms["position"] = position;

                    matrix42 = Matrix4.Identity();
                    matrix4.Copy(light.MatrixWorld);
                    matrix4 = viewMatrix * matrix4;
                    matrix42.ExtractRotation(matrix4);

                    Vector3 halfWidth = new Vector3(light.Width * 0.5f, 0.0f, 0.0f);
                    Vector3 halfHeight = new Vector3(0.0f, light.Height * 0.5f, 0.0f);

                    halfWidth = halfWidth.ApplyMatrix4(matrix42);
                    halfHeight = halfHeight.ApplyMatrix4(matrix42);

                    uniforms["halfWidth"] = halfWidth;
                    uniforms["halfHeight"] = halfHeight;

                    //TODO (abelnation):RectAreaLight distance
                    //uniforms["distance"] = distance

                    rectAreaList.Add(uniforms);

                    rectAreaLength++;

                }
                else if (light is PointLight)
                {
                    Hashtable uniforms = (Hashtable)(cache[light.type] as Hashtable).Clone();
                    Vector3 position = Vector3.Zero().SetFromMatrixPosition(light.MatrixWorld);
                    position.ApplyMatrix4(viewMatrix);
                    uniforms["position"] = position;

                    Color lightColor = light.Color;
                    uniforms["color"] = lightColor.MultiplyScalar(light.Intensity);

                    uniforms["distance"] = distance;
                    uniforms["decay"] = light.Decay;

                   

                    if (light.CastShadow)
                    {
                        var shadow = light.Shadow;

                        Hashtable shadowUniforms = (Hashtable)(shadowCache[light.type] as Hashtable).Clone();

                        shadowUniforms["shadowBias"] = shadow.Bias;
                        shadowUniforms["shadowNormalBias"] = shadow.NormalBias;
                        shadowUniforms["shadowRadius"] = shadow.Radius;
                        shadowUniforms["shadowMapSize"] = shadow.MapSize;                      
                        shadowUniforms["shadowCameraNear"] = shadow.Camera.Near;
                        shadowUniforms["shadowCameraFar"] = shadow.Camera.Far;

                        pointShadowList.Add(shadowUniforms);
                        pointShadowMapList.Add(shadowMap);
                        pointShadowMatrixList.Add(light.Shadow.Matrix);

                        numPointShadows++;
                    }

                    pointList.Add(uniforms);

                    pointLength++;
                }
                else if (light is HemisphereLight)
                {
                    Hashtable uniforms = (Hashtable)(cache[light.type] as Hashtable).Clone();

                    Vector3 direction = Vector3.Zero().SetFromMatrixPosition(light.MatrixWorld);

                    direction.TransformDirection(viewMatrix);

                    uniforms["direction"] = direction;

                    Color lightColor = light.Color;
                    uniforms["skyColor"] = lightColor.MultiplyScalar(light.Intensity);

                    Color groundColor = light.GroundColor;
                    uniforms["groundColor"] = groundColor.MultiplyScalar(light.Intensity);

                    hemiList.Add(uniforms);

                    hemiLength++;
                }
            }



            if (rectAreaLength > 0)
            {
                if (UniformsLib.LTC_FLOAT_1 != null)
                {
                    state["rectAreaLTC1"] = UniformsLib.LTC_FLOAT_1;
                }
                if (UniformsLib.LTC_FLOAT_1 != null)
                {
                    state["rectAreaLTC2"] = UniformsLib.LTC_FLOAT_2;
                }
                //if (state.Contains("rectAreaLTC1")) state["rectAreaLTC1"] = TextureLoader.LoadEmbedded("ltc_1.png");
                //if (state.Contains("rectAreaLTC2")) state["rectAreaLTC2"] = TextureLoader.LoadEmbedded("ltc_2.png");
            }

            state["ambient"] = ambientColor;

            Hashtable hash = (Hashtable)state["hash"];

            if ((int)hash["directionalLength"] != directionalLength ||
                (int)hash["pointLength"] != pointLength ||
                (int)hash["spotLength"] != spotLength ||
                (int)hash["rectAreaLength"] != rectAreaLength ||
                (int)hash["hemiLength"] != hemiLength ||
                (int)hash["numDirectionalShadows"] != numDirectionalShadows ||
                (int)hash["numPointShadows"] != numPointShadows ||
                (int)hash["numSpotShadows"] != numSpotShadows)
            {
                hash["directionalLength"] = directionalLength;
                hash["pointLength"] = pointLength;
                hash["spotLength"] = spotLength;
                hash["rectAreaLength"] = rectAreaLength;
                hash["hemiLength"] = hemiLength;

                hash["numDirectionalShadows"] = numDirectionalShadows;
                hash["numPointShadows"] = numPointShadows;
                hash["numSpotShadows"] = numSpotShadows;

                state["version"] = nextVersion++;
            }

            state["directionalShadow"] = directionalShadowList.ToArray();
            state["directionalShadowMap"] = directionalShadowMapList.ToArray();
            state["directionalShadowMatrix"] = directionalShadowMatrixList.ToArray();
            state["directional"] = directionalList.ToArray();
            state["spotShadow"] = spotShadowList.ToArray();
            state["spotShadowMap"] = spotShadowMapList.ToArray();
            state["spotShadowMatrix"] = spotShadowMatrixList.ToArray();
            state["spot"] = spotList.ToArray();
            state["rectArea"] = rectAreaList.ToArray();
            state["pointShadow"] = pointShadowList.ToArray();
            state["pointShadowMap"] = pointShadowMapList.ToArray();
            state["pointShadowMatrix"] = pointShadowMatrixList.ToArray();
            state["point"] = pointList.ToArray();
            state["hemi"] = hemiList.ToArray();
        }

    }
}
