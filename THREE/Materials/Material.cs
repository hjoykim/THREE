using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Textures;
using OpenTK.Graphics.ES30;
using THREE.Math;
using THREE.Renderers.gl;
namespace THREE.Materials
{
    public class Material : Hashtable,IDisposable,ICloneable
    {
        private static int materialIdCount;

        public int Id = materialIdCount++;

        public Guid Uuid = Guid.NewGuid();

        public Hashtable Defines = new Hashtable();

        public string Name;

        public string type = "Material";

        public bool Fog = true;

        public int Blending = Constants.NormalBlending;
        
        public int Side = Constants.FrontSide;

        public bool FlatShading = false;

        public bool VertexTangents = false;

        public bool VertexColors = false;
        
        public float Opacity = 1;

        public bool Transparent = false;

        public Color? Color;

        public Color Specular;

        public int BlendSrc = Constants.SrcAlphaFactor;

        public int BlendDst = Constants.OneMinusSrcAlphaFactor;

        public int BlendEquation = Constants.AddEquation;

        public int? BlendSrcAlpha;               

        public int? BlendDstAlpha;       

        public int? BlendEquationAlpha;

        public int DepthFunc = Constants.LessEqualDepth;

        public bool DepthTest = true;

        public bool DepthWrite = true;

        public int StencilWriteMask = 0xff;

	    public int StencilFunc = Constants.AlwaysStencilFunc;

	    public int StencilRef = 0;

	    public int StencilFuncMask = 0xff;

	    public int StencilFail = Constants.KeepStencilOp;

	    public int StencilZFail = Constants.KeepStencilOp;

	    public int StencilZPass = Constants.KeepStencilOp;

	    public bool StencilWrite = false;

        public List<Plane> ClippingPlanes = new List<Plane>();

        public bool ClipIntersection = false;

        public bool ClipShadows = false;

        public int? ShadowSide;
        
        public bool ColorWrite = true;

        public string Precision = null;

        public bool PolygonOffset = false;

        public float PolygonOffsetFactor = 0.0f;

        public float PolygonOffsetUnits = 0.0f;

        public bool Dithering = false;

        public float AlphaTest = 0;

        public bool PremultipliedAlpha = false;

        public bool Visible = true;

        public bool ToneMapped = true;

        public Hashtable UserData = new Hashtable();

        private bool needsUpdate;

       

        public bool NeedsUpdate  
        {
            get
            {
                return needsUpdate;
            }
            set
            {
                needsUpdate=value;
                if (needsUpdate) this.Version++;
            }
        }

        public string glslVersion ="";

        public string IndexOfAttributeName;

        public bool MorphTargets ;

        public bool MorphNormals;

        public Texture Map ;

        public Texture AlphaMap ;

        public Texture SpecularMap ;

        public Texture EnvMap ;

        public Texture NormalMap ;

        public int NormalMapType = -1;

        public Vector2 NormalScale ;

        public Texture BumpMap ;

        public float BumpScale ;

        public Texture LightMap ;

        public Texture AoMap ;

        public Texture EmissiveMap ;

        public Texture DisplacementMap;

        public float DisplacementScale;

        public float DisplacementBias;

        public float Clearcoat;

        public Texture ClearcoatMap;

        public float ClearcoatRoughness;

        public Texture ClearcoatRoughnessMap;

        public Vector2 ClearcoatNormalScale;

        public Texture ClearcoatNormalMap ;

        public Texture RoughnessMap ;

        public Texture MetalnessMap ;

        public Texture GradientMap ;

        public Texture TransmissionMap;

        public Color? Sheen ;

        public Color? Emissive;

        public float EmissiveIntensity = 1;

        public int Combine ;

        public bool SizeAttenuation;

        public bool Skinning;       

        public int DepthPacking = Constants.BasicDepthPacking;

        public GLProgram Program;

        public int numSupportedMorphTargets = 0;

        public int numSupportedMorphNormals = 0;

        public bool Clipping = false;

        public float Rotation = 0;

        public float Reflectivity =1;

        public float RefractionRatio;

        public float LightMapIntensity = 1;

        public float AoMapIntensity = 1;

        public float EnvMapIntensity = 1;

        public float LineWidth = 1;

        public bool Wireframe ;

        public float WireframeLineWidth = 1 ;

        public string WireframeLineCap ;

        public string WireframeLineJoin ;

        public float Shininess;

        public int Version = 0;

        public float Roughness = 0.5f;

        public float Metalness = 0.5f;

        public event EventHandler<EventArgs> Disposed;
                
        public Material()
        {
        }

        protected Material(Material source)
        {
            type = source.type;

            Name = source.Name;

		    Fog = source.Fog;

		    Blending = source.Blending;
		    
            Side = source.Side;
		    
            FlatShading = source.FlatShading;
		    
            VertexColors = source.VertexColors;

		    Opacity = source.Opacity;
		    
            Transparent = source.Transparent;

		    BlendSrc = source.BlendSrc;
		    
            BlendDst = source.BlendDst;
		    
            BlendEquation = source.BlendEquation;
		    
            BlendSrcAlpha = source.BlendSrcAlpha;
		    
            BlendDstAlpha = source.BlendDstAlpha;
		    
            BlendEquationAlpha = source.BlendEquationAlpha;

		    DepthFunc = source.DepthFunc;
		    
            DepthTest = source.DepthTest;
		    
            DepthWrite = source.DepthWrite;

		    StencilWrite = source.StencilWrite;
		    
            StencilWriteMask = source.StencilWriteMask;
		    
            StencilFunc = source.StencilFunc;
		    
            StencilRef = source.StencilRef;
		    
            StencilFuncMask = source.StencilFuncMask;
		    
            StencilFail = source.StencilFail;
		    
            StencilZFail = source.StencilZFail;
		    
            StencilZPass = source.StencilZPass;

		    ColorWrite = source.ColorWrite;

		    Precision = source.Precision;

		    PolygonOffset = source.PolygonOffset;
		    
            PolygonOffsetFactor = source.PolygonOffsetFactor;
		    
            PolygonOffsetUnits = source.PolygonOffsetUnits;

		    Dithering = source.Dithering;

		    AlphaTest = source.AlphaTest;
		    
            PremultipliedAlpha = source.PremultipliedAlpha;

		    Visible = source.Visible;

		    ToneMapped = source.ToneMapped;

		    UserData = (Hashtable)source.UserData.Clone();

            NeedsUpdate = source.NeedsUpdate;

            ClipShadows = source.ClipShadows;
		    
            ClipIntersection = source.ClipIntersection;

            ClippingPlanes = source.ClippingPlanes.GetRange(0, source.ClippingPlanes.Count);

		    ShadowSide = source.ShadowSide;

            Version = source.Version;

            glslVersion = source.glslVersion;

            IndexOfAttributeName = source.IndexOfAttributeName;

            MorphTargets = source.MorphTargets;

            MorphNormals = source.MorphNormals;

            NormalMapType = source.NormalMapType;

            NormalScale = source.NormalScale;

            BumpScale = source.BumpScale;

            DisplacementScale = source.DisplacementScale;

            DisplacementBias = source.DisplacementBias;

            Clearcoat = source.Clearcoat;

            ClearcoatRoughness = source.ClearcoatRoughness;

            ClearcoatNormalScale = source.ClearcoatNormalScale;

            Sheen = source.Sheen;

            Emissive = source.Emissive;

            EmissiveIntensity = source.EmissiveIntensity;

            Combine = source.Combine;

            DepthPacking = source.DepthPacking;

            Program = source.Program;

            numSupportedMorphNormals = source.numSupportedMorphNormals;

            numSupportedMorphTargets = source.numSupportedMorphTargets;

            Clipping = source.Clipping;

            Rotation = source.Rotation;

            Reflectivity = source.Reflectivity;

            RefractionRatio = source.RefractionRatio;

            LightMapIntensity = source.LightMapIntensity;

            AoMapIntensity = source.AoMapIntensity;

            EnvMapIntensity = source.EnvMapIntensity;

            LineWidth = source.LineWidth;

            Wireframe = source.Wireframe;

            WireframeLineWidth = source.WireframeLineWidth;

            WireframeLineCap = source.WireframeLineCap;

            WireframeLineJoin = source.WireframeLineJoin;

            Roughness = source.Roughness;

            Metalness = source.Metalness;
            
        }

        public new object Clone() 
        {
            return new Material(this);
        }

        protected void SetValues(Hashtable values)
        {
            if (values == null)
                return;

            foreach (DictionaryEntry item in values)
            {
                var newValue = item.Value;
                var key = item.Key as string;

                var type = this.GetType();
                var propertyInfo = type.GetProperty(key, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(this, newValue);
                }
                else
                {
                    var fieldInfo = type.GetField(key, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(this, newValue);
                    }
                    else
                    {
                        Trace.TraceWarning("attribute {0} not found", key);
                    }
                }
            }
        }

       ~Material()
        {
            this.Dispose(false);
        }
        public string customProgramCacheKey;
        public virtual void Dispose()
        {
            Dispose(disposed);
        }
        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
                handler(this, new EventArgs());
        }
        
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            try
            {
                this.RaiseDisposed();
                this.disposed = true;
            }
            finally
            {

            }
            this.disposed = true;
        }
    }
}
