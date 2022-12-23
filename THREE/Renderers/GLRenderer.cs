using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace THREE
{
    public class GLRenderer : DisposableObject
    {
        public OpenTK.Graphics.IGraphicsContext Context;

        public event EventHandler<EventArgs> Disposed;

        //public Dictionary<string,RenderInfo> sceneList = new Dictionary<string,RenderInfo>();

        private GLRenderState CurrentRenderState;

        private GLRenderList CurrentRenderList;

        public bool AutoClear = true;

        public bool AutoClearColor = true;

        public bool AutoClearDepth = true;

        public bool AutoClearStencil = true;

        // scene graph
        public bool SortObjects = true;

        //user-defined clipping
        public List<Plane> ClippingPlanes = new List<Plane>();

        public bool LocalClippingEnabled = false;

        // physically based shading

        public float GammaFactor = 2.0f; // for backwards compatibility

        public int outputEncoding = Constants.LinearEncoding;
        // physically lights

        public bool PhysicallyCorrectLights = false;

        // tone mapping
        public int ToneMapping = Constants.LinearToneMapping;

        public float ToneMappingExposure = 1.0f;

        public float ToneMappingWhitePoint = 1.0f;

        // morphs

        public int MaxMorphTargets = 8;

        public int MaxMorphNormals = 4;

        public ShaderLib ShaderLib = new ShaderLib();

        GLCapabilitiesParameters parameters;

        public GLExtensions extensions;

        public GLCapabilities capabilities;

        public GLState state;

        public GLInfo info;

        public GLProperties properties;

        GLTextures textures;

        GLAttributes attributes;

        GLGeometries geometries;

        GLUtils utils;

        GLObjects objects;

        GLMorphtargets morphtargets;

        GLPrograms programCache;

        GLRenderStates renderStates;

        GLRenderLists renderLists;

        GLBackground background;

        GLBufferRenderer bufferRenderer;

        GLIndexedBufferRenderer indexedBufferRenderer;

        private bool premultipliedAlpha = true;

        public Hashtable debug = new Hashtable();

        private int _currentActiveMipmapLevel = 0;

        public GLShadowMap ShadowMap;

        public GLMultiview Multiview;

        public GLBindingStates bindingStates;

        private Scene emptyScene;

        GLCubeMap cubeMaps;

        GLMaterials materials;

        public bool ShadowMapEnabled
        {
            get
            {
                return ShadowMap.Enabled;
            }
            set
            {
                ShadowMap.Enabled = value;
            }
        }

        public int ShadowMapType
        {
            get
            {
                return ShadowMap.ShadowMapType;
            }
            set
            {
                ShadowMap.ShadowMapType = value;
            }
        }

        public int ShadowMapCullFace
        {
            get
            {
                return ShadowMap.type;
            }
            set
            {
                ShadowMap.type = value;
            }
        }
        #region internal properties

        private int? _framebuffer = null;

        private int _currentActiveCubeFace = 0;

        private GLRenderTarget _currentRenderTarget = null;

        private int? _currentFramebuffer = null;

        private int _currentMaterialId = -1;

        private Camera _currentCamera;

        private Camera _currentArrayCamera;

        private Vector4 _currentViewport = Vector4.Zero();

        private Vector4 _currentScissor = Vector4.Zero();

        private bool? _currentScissorTest = null;



        private int _pixelRatio = 1;

        private Vector4 _viewport = Vector4.Zero();

        private Vector4 _scissor = Vector4.Zero();

        private bool _scissorTest = false;

        // frustum

        private Frustum _frustum = new Frustum();

        // clipping 

        private GLClipping _clipping = new GLClipping();

        private bool _clippingEnabled = false;

        private bool _localClippingEnabled = false;

        // camera matrices cache

        private Matrix4 _projScreenMatrix = Matrix4.Identity();

        private Vector3 _vector3 = Vector3.Zero();

        public int Width;
        public int Height;
        public float AspectRatio => Width / Height;

        #endregion

        #region constructor
        public GLRenderer() 
        {
           
        }

        public GLRenderer(IGraphicsContext context, int width, int height)  : base()
        {
            this.Context = context;
            this._viewport = new Vector4(0, 0, width, height);
            this.Width = width;
            this.Height = height;
            this.Init();
        }
        #endregion

        public float GetPixelRatio()
        {
            return _pixelRatio;
        }

        public void ClearDepth()
        {
            Clear(false, true, false);
        }
        public void ClearStencil()
        {
            Clear(false, false, true);
        }
        public Vector2 GetSize(Vector2 target=null)
        {
            if(target==null)
            {
                target = new Vector2();
            }
            target.Set(Width, Height);

            return target;
        }
        public void SetSize(float width,float height)
        {
            Width = (int)System.Math.Floor(width*_pixelRatio);
            Height =(int)System.Math.Floor(height * _pixelRatio);

            SetViewport(Width, Height);
        }
        public void Clear(bool? color=null, bool? depth=null, bool? stencil=null)
        {
            int bits= 0;
            
            if (color==null || color ==true) bits |= (int)ClearBufferMask.ColorBufferBit;
            if (depth==null || depth==true) bits |= (int)ClearBufferMask.DepthBufferBit;
            if (stencil==null || stencil==true) bits |= (int)ClearBufferMask.StencilBufferBit;


            ClearBufferMask mask = (ClearBufferMask)Enum.ToObject(typeof(ClearBufferMask), bits);
            
            if(Context.IsCurrent)
                GL.Clear(mask);
        }
        public Color GetClearColor()
        {
            return this.background.ClearColor;
        }

        public float GetClearAlpha()
        {
            return this.background.ClearAlpha;
        }
        public void SetClearAlpha(float alpha)
        {
            background.SetClearAlpha(alpha);
        }
        public void SetClearColor(Color color, float alpha=1)
        {
            this.background.SetClearColor(color,alpha);
        }
        public void SetClearColor(int color,float alpha = 1)
        {
            SetClearColor(Color.Hex(color), alpha);
        }
        public void SetViewport(int width, int height)
        {
            _viewport.Set(0, 0, width, height);

            this._currentViewport = (_viewport * _pixelRatio).Floor();
            state.Viewport(_currentViewport);
        }
        public void SetViewport(int x,int y,int width,int height)
        {
            _viewport.Set(x, y, width, height);
            this._currentViewport = (_viewport * _pixelRatio).Floor();
            state.Viewport(_currentViewport);
        }
        private void InitGLContext()
        {
            this.extensions = new GLExtensions();
            this.parameters = new GLCapabilitiesParameters();
            this.capabilities = new GLCapabilities(extensions, ref parameters);

            this._viewport.Set(0, 0, this.Width, this.Height);
            this._scissor.Set(0, 0, this.Width, this.Height);
            if (capabilities.IsGL2 == false)
            {
                //extensions.get( 'WEBGL_depth_texture' );
                //extensions.get( 'OES_texture_float' );
                //extensions.get( 'OES_texture_half_float' );
                //extensions.get( 'OES_texture_half_float_linear' );
                //extensions.get( 'OES_standard_derivatives' );
                //extensions.get( 'OES_element_index_uint' );
                //extensions.get( 'ANGLE_instanced_arrays' );
            }

            //Extensions.Get("OES_texture_float_linear");
            this.utils = new GLUtils(extensions, capabilities);
            this.state = new GLState(extensions, utils, capabilities);
            this._currentScissor = (_scissor * _pixelRatio).Floor();
            this._currentViewport = (_viewport * _pixelRatio).Floor();

            state.Scissor(_currentScissor);
            state.Viewport(_currentViewport);

            this.info = new GLInfo();

            this.properties = new GLProperties();
            
            this.textures = new GLTextures(this.Context, extensions, state, properties, capabilities, utils, info);
            
            this.attributes = new GLAttributes();

            this.geometries = new GLGeometries(this,this.attributes,this.info);

            this.objects = new GLObjects(this.geometries, this.attributes, this.info);
            
            this.morphtargets = new GLMorphtargets();

            this.cubeMaps = new GLCubeMap(this);

            this.bindingStates = new GLBindingStates(this.Context, extensions, attributes, capabilities);

            this.programCache = new GLPrograms(this, cubeMaps,extensions,capabilities,bindingStates,_clipping);

            this.renderLists = new GLRenderLists();

            this.renderStates = new GLRenderStates();

            this.background = new GLBackground(this,cubeMaps, state, objects, premultipliedAlpha);

            this.bufferRenderer = new GLBufferRenderer(this,extensions,info,capabilities);

            this.indexedBufferRenderer = new GLIndexedBufferRenderer(this, extensions, info, capabilities);

           

            this.info.programs = programCache.Programs;

            this.emptyScene = new Scene();

           

            this.materials = new GLMaterials(properties);
        }

        private int  GetTargetPixelRatio() 
        {

		    return _currentRenderTarget == null ? _pixelRatio : 1;

	    }
        private void OnMaterialDispose(object sender, EventArgs e)
        {

        }
        public GLRenderLists GetRenderLists()
        {
            return renderLists;
        }
        public GLRenderList GetRenderList()
        {
            return CurrentRenderList;
        }
        public void SetRenderList(GLRenderList renderList)
        {
            CurrentRenderList = renderList;
        }
        public GLRenderState GetRenderState()
        {
            return CurrentRenderState;
        }
        public void SetRenderState(GLRenderState renderState)
        {
            CurrentRenderState = renderState;
        }
        #region Private Renderring 

        #region Buffer deallocation

        private void DeallocateMaterial(Material material)
        {
            if (!this.Context.IsDisposed && this.Context.IsCurrent)
            {
                ReleaseMaterialProgramReference(material);
                properties.Remove(material);
            }
        }

        private void ReleaseMaterialProgramReference(Material material)
        {
            var programInfo = properties.Get(material)["program"];

            //material.Program = null;

            if (programInfo != null)
            {
                programCache.ReleaseProgram((GLProgram)programInfo);
            }

        }
        #endregion

        #region Buffer Rendering
        private void RenderObjectImmediate(Object3D object3D, GLProgram program)
        {
            RenderBufferImmediate(object3D, program);
        }

        private void RenderBufferImmediate(Object3D object3D, GLProgram program)
        {
            state.InitAttributes();
            /*
            var buffers = properties.Get(object3D);

            if (object3D.hasPositions && !buffers.position) buffers.position = _gl.createBuffer();
            if (object3D.hasNormals && !buffers.normal) buffers.normal = _gl.createBuffer();
            if (object3D.hasUvs && !buffers.uv) buffers.uv = _gl.createBuffer();
            if (object3D.hasColors && !buffers.color) buffers.color = _gl.createBuffer();

            var programAttributes = program.getAttributes();

            if (object3D.hasPositions)
            {

                _gl.bindBuffer(_gl.ARRAY_BUFFER, buffers.position);
                _gl.bufferData(_gl.ARRAY_BUFFER, object3D.positionArray, _gl.DYNAMIC_DRAW);

                state.enableAttribute(programAttributes.position);
                _gl.vertexAttribPointer(programAttributes.position, 3, _gl.FLOAT, false, 0, 0);

            }

            if (object3D.hasNormals)
            {

                _gl.bindBuffer(_gl.ARRAY_BUFFER, buffers.normal);
                _gl.bufferData(_gl.ARRAY_BUFFER, object3D.normalArray, _gl.DYNAMIC_DRAW);

                state.enableAttribute(programAttributes.normal);
                _gl.vertexAttribPointer(programAttributes.normal, 3, _gl.FLOAT, false, 0, 0);

            }

            if (object3D.hasUvs)
            {

                _gl.bindBuffer(_gl.ARRAY_BUFFER, buffers.uv);
                _gl.bufferData(_gl.ARRAY_BUFFER, object3D.uvArray, _gl.DYNAMIC_DRAW);

                state.enableAttribute(programAttributes.uv);
                _gl.vertexAttribPointer(programAttributes.uv, 2, _gl.FLOAT, false, 0, 0);

            }

            if (object3D.hasColors)
            {

                _gl.bindBuffer(_gl.ARRAY_BUFFER, buffers.color);
                _gl.bufferData(_gl.ARRAY_BUFFER, object3D.colorArray, _gl.DYNAMIC_DRAW);

                state.enableAttribute(programAttributes.color);
                _gl.vertexAttribPointer(programAttributes.color, 3, _gl.FLOAT, false, 0, 0);

            }

            state.disableUnusedAttributes();

            _gl.drawArrays(_gl.TRIANGLES, 0, object3D.count);

            object3D.count = 0;
            */
        }

        public void RenderBufferDirect(Camera camera, Scene scene, Geometry geometry, Material material, Object3D object3D, DrawRange? group)
        {

            if (scene == null) scene = emptyScene;

            var frontFaceCW = (object3D is Mesh && object3D.MatrixWorld.Determinant() < 0);
                        
            var program = SetProgram(camera, scene, material, object3D);

            state.SetMaterial(material, frontFaceCW);           

            var index = (geometry as BufferGeometry).Index;
            BufferAttribute<float> position = null;
            if ((geometry as BufferGeometry).Attributes.ContainsKey("position"))
            {
                var bufferGeom = geometry as BufferGeometry;
                position = (BufferAttribute<float>)(bufferGeom.Attributes["position"]);
            }

            //

            if (index != null && index.Count == 0) return;
            //if (position != null || position.count === 0) return;
            if (position == null) return;

            //

            var rangeFactor = 1;

            if (material.Wireframe == true)
            {

                index = geometries.GetWireframeAttribute<int>(geometry);
                rangeFactor = 2;

            }

            if(material.MorphTargets || material.MorphNormals)
            {
                morphtargets.Update(object3D, geometry as BufferGeometry, material, program);
            }

            bindingStates.Setup(object3D, material, program, geometry, index);

            BufferType attribute = null;
            var renderer = bufferRenderer;

            if (index != null)
            {
                attribute = attributes.Get<int>(index);

                renderer = (GLBufferRenderer)indexedBufferRenderer;
                (renderer as GLIndexedBufferRenderer).SetIndex(attribute);

            }
            //if (updateBuffers)
            //{

            //    SetupVertexAttributes(object3D, geometry, material, program);

            //    if (index != null)
            //    {

            //        GL.BindBuffer(BufferTarget.ElementArrayBuffer, attribute.buffer);

            //    }

            //}

            //

            var dataCount = (index != null) ? index.count : position is InterleavedBufferAttribute<float> ? (position as InterleavedBufferAttribute<float>).count : position.count;

            var rangeStart = (geometry as BufferGeometry).DrawRange.Start * rangeFactor;
            var rangeCount = (geometry as BufferGeometry).DrawRange.Count * rangeFactor;

            var groupStart = group != null ? group.Value.Start * rangeFactor : 0;
            var groupCount = group != null ? group.Value.Count * rangeFactor : float.PositiveInfinity;

            var drawStart = (float)System.Math.Max(rangeStart, groupStart);
            var drawEnd = (float)System.Math.Min(System.Math.Min(dataCount, rangeStart + rangeCount), groupStart + groupCount) - 1;

            var drawCount = (float)System.Math.Max(0, drawEnd - drawStart + 1);

            if (drawCount == 0) return;

            //

            if (object3D is Mesh)
            {

                if (material.Wireframe == true)
                {
                    state.SetLineWidth(material.WireframeLineWidth * GetTargetPixelRatio());
                    renderer.SetMode(PrimitiveType.Lines);

                }
                else
                {
                    renderer.SetMode(PrimitiveType.Triangles);
                }

            }
            else if (object3D is Line)
            {

                float lineWidth;
                if(material is LineBasicMaterial)
                {
                    lineWidth = (material as LineBasicMaterial).LineWidth;
                }
                else
                {
                    lineWidth = 1f; // Not using Line*Material
                }

                state.SetLineWidth(lineWidth * GetTargetPixelRatio());

                if (object3D is LineSegments)
                {

                    renderer.SetMode(PrimitiveType.Lines);

                }
                else if (object3D is LineLoop)
                {

                    renderer.SetMode(PrimitiveType.LineLoop);

                }
                else
                {

                    renderer.SetMode(PrimitiveType.LineStrip);

                }

            }
            else if (object3D is Points)
            {
                OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.PointSprite);
                OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.ProgramPointSize);
                renderer.SetMode(PrimitiveType.Points);
            }
            else if (object3D is Sprite)
            {

                renderer.SetMode(PrimitiveType.Triangles);

            }

            if (object3D is InstancedMesh)
            {
                renderer.RenderInstances(geometry, (int)drawStart, (int)drawCount, (object3D as InstancedMesh).Count);

            }
            else if (geometry is InstancedBufferGeometry)
            {
                var instanceCount = System.Math.Min((geometry as InstancedBufferGeometry).InstanceCount, (geometry as InstancedBufferGeometry).MaxInstanceCount.Value);
                renderer.RenderInstances(geometry, (int)drawStart, (int)drawCount, instanceCount);

            }
            else
            {

                renderer.Render((int)drawStart,(int)drawCount);

            }
        }


        #endregion
          
       

        // this function is called by Render()
        //private void RenderSceneList(RenderInfo renderInfo)
        public void Render(Scene scene,Camera camera)
        {
            //Scene scene = renderInfo.Scene;
            //Camera camera = renderInfo.Camera;

            if (scene == null || camera == null)
            {
                throw new Exception("THREE.Renderers.RenderSceneList : scene or camera is null");
            }
            if (!(camera is Camera))
            {
                throw new Exception("THREE.Renderers.RenderSceneList : camera is not an instance of THREE.Cameras.Camera");
            }

            bindingStates.ResetDefaultState();
            _currentMaterialId = -1;
            _currentCamera = null;

            GLRenderTarget renderTarget = null;            

            bool forceClear = false;        

            //update scene graph
            if (scene.AutoUpdate == true) scene.UpdateMatrixWorld();

            //update camera matrices and frustum

            if (camera.Parent == null) camera.UpdateMatrixWorld();

            //if (xr.enabled === true && xr.isPresenting === true)
            //{

            //    camera = xr.getCamera(camera);

            //}

            if (scene.OnBeforeRender != null)
            {
                scene.OnBeforeRender(this, scene, camera, null, null, null, renderTarget != null ? renderTarget : _currentRenderTarget);
            }

            CurrentRenderState = renderStates.Get(scene, camera);
            CurrentRenderState.Init();

           

            _projScreenMatrix = camera.ProjectionMatrix * camera.MatrixWorldInverse;
            _frustum.SetFromProjectionMatrix(this._projScreenMatrix);

            _localClippingEnabled = this.LocalClippingEnabled;
            _clippingEnabled = _clipping.Init(this.ClippingPlanes, _localClippingEnabled, camera);
            
            CurrentRenderList = renderLists.Get(scene, camera);
            CurrentRenderList.Init();

            ProjectObject(scene, camera, 0, this.SortObjects);

            CurrentRenderList.Finish();

            if (this.SortObjects == true)
            {
                CurrentRenderList.Sort();
            }

            if ( _clippingEnabled ) _clipping.BeginShadows();

		    var shadowsArray = CurrentRenderState.State.ShadowsArray;

		    ShadowMap.Render( shadowsArray, scene, camera );

		    CurrentRenderState.SetupLights( camera );

		    if ( _clippingEnabled ) _clipping.EndShadows();

		    //

		    if ( this.info.AutoReset ) this.info.Reset();

		    if ( renderTarget != null ) {

			    this.SetRenderTarget( renderTarget );

		    }          

            //this.AutoClear = scene.ClearBeforeRender;

		    background.Render( CurrentRenderList, scene, camera, forceClear );

		    // render scene

		    var opaqueObjects = CurrentRenderList.Opaque;
		    var transparentObjects = CurrentRenderList.Transparent;

		    if ( opaqueObjects.Count>0 ) RenderObjects( opaqueObjects, scene, camera );
		    if ( transparentObjects.Count>0 ) RenderObjects( transparentObjects, scene, camera );

            if (scene.OnAfterRender != null)
            {
                scene.OnAfterRender(this, scene, camera);
            }            

            if ( _currentRenderTarget != null ) {

			    // Generate mipmap if we're using any kind of mipmap filtering

			    textures.UpdateRenderTargetMipmap( _currentRenderTarget );

			    // resolve multisample renderbuffers to a single-sample texture if necessary

			    textures.UpdateMultisampleRenderTarget( _currentRenderTarget );

		    }

		    // Ensure depth buffer writing is enabled so it can be cleared on next render

		    state.buffers.depth.SetTest( true );
		    state.buffers.depth.SetMask( true );
		    state.buffers.color.SetMask( true );

		    state.SetPolygonOffset( false );                      

		    CurrentRenderList = null;
		    CurrentRenderState = null;

        }

        private void ProjectObject(Object3D object3D, Camera camera, int groupOrder, bool sortObjects)
        {
            if (object3D.Visible == false) return;

            var visible = object3D.Layers.Test(camera.Layers);

            if (visible)
            {
                if(object3D is Group)
                {
                    groupOrder = object3D.RenderOrder;
                }
                else if (object3D is LOD)
                {
                    if ((object3D as LOD).AutoUpdate == true)
                        (object3D as LOD).Update(camera);
                }
                else if (object3D is Light)
                {
                    CurrentRenderState.PushLight((Light)object3D);

                    if (object3D.CastShadow)
                    {
                        CurrentRenderState.PushShadow((Light)object3D);
                    }
                }
                else if (object3D is Sprite)
                {
                    if (!object3D.FrustumCulled || _frustum.IntersectsSprite(object3D as Sprite))
                    {

                        if (sortObjects)
                        {

                            _vector3.SetFromMatrixPosition(object3D.MatrixWorld).ApplyMatrix4(_projScreenMatrix);

                        }

                        var geometry = objects.Update(object3D);
                        var material = object3D.Material;

                        if (material.Visible)
                        {

                            CurrentRenderList.Push(object3D, geometry, material, groupOrder, _vector3.Z, null);

                        }

                    }

                }
                else if (object3D is ImmediateRenderObject)
                {
                    if (sortObjects)
                    {
                        _vector3.SetFromMatrixPosition(object3D.MatrixWorld).ApplyMatrix4(_projScreenMatrix);
                    }

                    CurrentRenderList.Push(object3D, null, object3D.Material, groupOrder, _vector3.Z, null);
                }
                else if (object3D is Mesh || object3D is Line || object3D is Points)
                {
                    if (object3D is SkinnedMesh)
                    {
                        //update skeleton only once in a frame

                        if ((object3D as SkinnedMesh).Skeleton.Frame != info.render.Frame)
                        {
                            (object3D as SkinnedMesh).Skeleton.Update();
                            (object3D as SkinnedMesh).Skeleton.Frame = info.render.Frame;
                        }
                    }
                    if (!object3D.FrustumCulled || _frustum.IntersectsObject(object3D))
                    {
                        if (sortObjects)
                        {
                            _vector3.SetFromMatrixPosition(object3D.MatrixWorld).ApplyMatrix4(_projScreenMatrix);
                        }

                        var geometry = objects.Update(object3D);
                        var material = object3D.Material;
                        if (object3D.Materials.Count > 1)
                        {
                            var materials = object3D.Materials;
                            var groups = geometry.Groups;

                            for (int i = 0; i < groups.Count; i++)
                            {
                                var group = groups[i];
                                var groupMaterial = materials[group.MaterialIndex];

                                if (groupMaterial != null && groupMaterial.Visible)
                                {
                                    
                                    CurrentRenderList.Push(object3D, (BufferGeometry)geometry, groupMaterial, groupOrder, _vector3.Z, group);
                                }
                            }
                        }
                        else if (material.Visible)
                        {
                            CurrentRenderList.Push(object3D, (BufferGeometry)geometry, material, groupOrder, _vector3.Z, null);
                        }

                    }
                }
            }

            var children = object3D.Children;

            for (int i = 0; i < children.Count; i++)
            {
                ProjectObject(children[i], camera, groupOrder, sortObjects);
            }
        }

        private void RenderObjects(List<RenderItem> renderList, Scene scene, Camera camera)
        {
            var overrideMaterial = scene is Scene ? scene.OverrideMaterial : null;
            for (int i = 0; i < renderList.Count; i++)
            {
                var renderItem = renderList[i];

                var object3D = renderItem.Object3D;
                var geometry = renderItem.Geometry;
                var material = overrideMaterial == null ? renderItem.Material : overrideMaterial;
                var group = renderItem.Group;

                if (camera is ArrayCamera)
                {
                    _currentArrayCamera = camera;

                    // if(vr.)
                    //{
                    
                    //}
                    //else 
                    //{
                    var cameras = (camera as ArrayCamera).Cameras;
                    for (int j = 0; j < cameras.Count; j++)
                    {
                        Camera camera2 = cameras[j];

                        if (object3D.Layers.Test(camera2.Layers))
                        {

                            state.Viewport(_currentViewport.Copy(camera2.Viewport));

                            CurrentRenderState.SetupLights(camera2);
                            RenderObject(object3D, scene, camera2,geometry, material, group);
                        }
                    }

                    // }

                }
                else
                {
                    _currentArrayCamera = null;

                    RenderObject(object3D, scene, camera, geometry, material, group);
                }
            }
        }

        private void RenderObject(Object3D object3D, Scene scene, Camera camera, Geometry geometry, Material material, DrawRange? group)
        {
            //TODO:
            if(object3D.OnBeforeRender!=null)
                object3D.OnBeforeRender(this, scene, camera, geometry, material, group,null);

            CurrentRenderState = renderStates.Get(scene, _currentArrayCamera != null ? _currentArrayCamera : camera);

            object3D.ModelViewMatrix = camera.MatrixWorldInverse * object3D.MatrixWorld;
            object3D.NormalMatrix.GetNormalMatrix(object3D.ModelViewMatrix);

            if (object3D is ImmediateRenderObject)
            {
                var program = SetProgram(camera, scene, material, object3D);

                state.SetMaterial(material);

                RenderObjectImmediate(object3D, program);
            }
            else
            {
                RenderBufferDirect(camera, scene, geometry, material, object3D, group);
            }

            //TODO:
            //object3D.OnAfterRender()
            CurrentRenderState = renderStates.Get(scene, _currentArrayCamera != null ? _currentArrayCamera : camera);

        }

        #endregion
        private void InitMaterial(Material material, Scene scene, Object3D object3D)
        {
            var fog = scene.Fog;
            var materialProperties = this.properties.Get(material);

            var lights = CurrentRenderState.State.Lights;
            var shadowsArray = CurrentRenderState.State.ShadowsArray;

            var lightsStateVersion = lights.state["version"];

            var parameters = programCache.GetParameter(material, lights, shadowsArray, scene, object3D);

            var programCacheKey = programCache.getProgramCacheKey(material, parameters).Replace("False","false").Replace("True","true");

            var program = (GLProgram)materialProperties["program"];

            var programChange = true;

            if (program == null)
            {
                material.Disposed += (sender, e) =>
                {
                    DeallocateMaterial(material);
                };
            }
            else if (!program.Code.Equals(programCacheKey))
            {
                ReleaseMaterialProgramReference(material);
            }
            else if (materialProperties["lightsStateVersion"] != lightsStateVersion)
            {
                materialProperties["lightsStateVersion"] = lightsStateVersion;
                programChange = false;
            }
            else if (parameters["shaderId"] != null)
            {
                // same glsl and uniform list
                //const environment = material.isMeshStandardMaterial ? scene.environment : null;
                //materialProperties.envMap = cubemaps.get(material.envMap || environment);
                return;
            }
            else
            {
                // only rebuild uniform list
                programChange = false;
            }

            if (programChange)
            {

                parameters["uniforms"] = programCache.GetUniforms(material);

                if (material.OnBeforeCompile != null)
                    material.OnBeforeCompile(parameters,this);

                program = programCache.AcquireProgram(parameters, programCacheKey);
                              

                materialProperties["program"] = program;
                material.Program = program;
                materialProperties["uniforms"] = parameters["uniforms"];
                materialProperties["outputEncoding"] = parameters["outputEncoding"];
            }

            var uniforms = materialProperties["uniforms"] as GLUniforms;

            if (!(material is ShaderMaterial) && !(material is RawShaderMaterial) || material.Clipping == true)
            {
                materialProperties["numClippingPlanes"] = _clipping.numPlanes;
                materialProperties["numIntersection"] = _clipping.numIntersection;
                uniforms["clippingPlanes"] = _clipping.uniform;
            }
            materialProperties["environment"] = material is MeshStandardMaterial ? scene.Environment : null;
            materialProperties["fog"] = scene.Fog;
            materialProperties["envMap"] = cubeMaps.Get(material.EnvMap!=null ? material.EnvMap : materialProperties["environment"] as Texture);

            materialProperties["needsLights"] = MaterialNeedsLights(material);
            materialProperties["lightsStateVersion"] = lightsStateVersion;

            if ((bool)materialProperties["needsLights"])
            {
                (uniforms["ambientLightColor"] as GLUniform)["value"] = lights.state["ambient"];
                (uniforms["lightProbe"] as GLUniform)["value"] = lights.state["probe"];
                (uniforms["directionalLights"] as GLUniform)["value"] = lights.state["directional"];
                (uniforms["directionalLightShadows"] as GLUniform)["value"] = lights.state["directionalShadow"];
                (uniforms["spotLights"] as GLUniform)["value"] = lights.state["spot"];
                (uniforms["spotLightShadows"] as GLUniform)["value"] = lights.state["spotShadow"];
                (uniforms["rectAreaLights"] as GLUniform)["value"] = lights.state["rectArea"];
                (uniforms["ltc_1"] as GLUniform)["value"] = lights.state["rectAreaLTC1"];
                (uniforms["ltc_2"] as GLUniform)["value"] = lights.state["rectAreaLTC2"];
                (uniforms["pointLights"] as GLUniform)["value"] = lights.state["point"];
                (uniforms["pointLightShadows"] as GLUniform)["value"] = lights.state["pointShadow"];
                (uniforms["hemisphereLights"] as GLUniform)["value"] = lights.state["hemi"];

                (uniforms["directionalShadowMap"] as GLUniform)["value"] = lights.state["directionalShadowMap"];
                (uniforms["directionalShadowMatrix"] as GLUniform)["value"] = lights.state["directionalShadowMatrix"];
                (uniforms["spotShadowMap"] as GLUniform)["value"] = lights.state["spotShadowMap"];
                (uniforms["spotShadowMatrix"] as GLUniform)["value"] = lights.state["spotShadowMatrix"];
                (uniforms["pointShadowMap"] as GLUniform)["value"] = lights.state["pointShadowMap"];
                (uniforms["pointShadowMatrix"] as GLUniform)["value"] = lights.state["pointShadowMatrix"];
            }

            var progUniforms = (materialProperties["program"] as GLProgram).GetUniforms();
            List<GLUniform> uniformsList = GLUniforms.SeqWithValue(progUniforms.Seq, uniforms);
            materialProperties["uniformsList"] = uniformsList;
            
        }

        #region public Render function

        public virtual void Init()
        {

            debug.Add("checkShaderErrors", true);

            this.InitGLContext();

            //VR omitted

            Multiview = new GLMultiview(this);

            ShadowMap = new GLShadowMap(this, objects, this.capabilities.maxTextureSize);
        }

        public Vector4 GetCurrentViewport(Vector4 target)
        {
            target.Copy(_currentViewport);

            return target;
        }

        public virtual void SetGraphicsContext(IGraphicsContext context, int width, int height)
        {
            Context = context;
            Resize(width, height);
        }

        public virtual void Resize(int width, int height)
        {
            //foreach (string key in sceneList.Keys)
            //{
            //    RenderInfo info = sceneList[key];
            //    info.Camera.MatrixWorldNeedsUpdate = true;
            //}
            Width = width;
            Height = height;

            this._viewport.Set(0, 0, width, height);
            this._currentViewport.Set(0, 0, width, height);       
        }
        #endregion

        private void InitTestures()
        {

        }
        public GLRenderTarget GetRenderTarget()
        {
            return _currentRenderTarget;
        }

        public void SetRenderTarget(GLRenderTarget renderTarget, int? activeCubeFace=null, int? activeMipmapLevel=null)
        {
            _currentRenderTarget = renderTarget;
            
            if(activeCubeFace!=null)
                _currentActiveCubeFace = activeCubeFace.Value;

            if(activeMipmapLevel!=null)
            _currentActiveMipmapLevel = activeMipmapLevel.Value;

            if (renderTarget != null && (properties.Get(renderTarget) as Hashtable)["glFramebuffer"] == null)
            {
                textures.SetupRenderTarget(renderTarget);
            }
            
            var framebuffer = _framebuffer;
		    var isCube = false;

		    if ( renderTarget!=null ) {

			    if ( renderTarget is GLCubeRenderTarget ) 
                {

                    var glFramebuffer =(int[]) (properties.Get( renderTarget ) as Hashtable)["glFramebuffer"];

				    framebuffer = glFramebuffer[activeCubeFace!=null ? activeCubeFace.Value : 0 ];
				    isCube = true;

			    } 
                else if ( renderTarget is GLMultisampleRenderTarget ) 
                {
                    var glFramebuffer =(int) (properties.Get( renderTarget ) as Hashtable)["glMultisampledFramebuffer"];
				    framebuffer = glFramebuffer;

			    } 
                else 
                {
                    var glFramebuffer =(int) (properties.Get( renderTarget ) as Hashtable)["glFramebuffer"];
				    framebuffer = glFramebuffer;

			    }

			    _currentViewport.Copy( renderTarget.Viewport );
			    _currentScissor.Copy( renderTarget.Scissor );
			    _currentScissorTest = renderTarget.ScissorTest;

		    } 
            else 
            {

			    _currentViewport.Copy( _viewport ).MultiplyScalar( _pixelRatio ).Floor();
			    _currentScissor.Copy( _scissor ).MultiplyScalar( _pixelRatio ).Floor();
			    _currentScissorTest = _scissorTest;

		    }

		    if ( _currentFramebuffer != framebuffer ) {

			    GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer==null? 0:(int)framebuffer);
			    _currentFramebuffer = framebuffer;

		    }

		    state.Viewport( _currentViewport );
		    state.Scissor( _currentScissor );
		    state.SetScissorTest( _currentScissorTest.Value );

		    if ( isCube ) {

			    Hashtable textureProperties = (Hashtable)properties.Get( renderTarget.Texture );
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget2d.TextureCubeMapPositiveX + (activeCubeFace != null ? activeCubeFace.Value : 0), (int)textureProperties["glTexture"], activeMipmapLevel != null ? activeMipmapLevel.Value : 0);

		    }            
        }

        public void ReadRenderTargetPixels(GLRenderTarget renderTarget, float x,float y, int width, int height, byte[] buffer,int? activeCubeFaceIndex)
        {
            if (renderTarget == null)
            {
                //console.error('THREE.WebGLRenderer.readRenderTargetPixels: renderTarget is null THREE.WebGLRenderTarget.');
                return;
            }

            //var glFramebuffer = (int[])(properties.Get(renderTarget) as Hashtable)["glFramebuffer"];
            int framebuffer;
            if (renderTarget is GLCubeRenderTarget)
            {
                framebuffer = ((int[])(properties.Get(renderTarget) as Hashtable)["glFramebuffer"])[activeCubeFaceIndex != null ? activeCubeFaceIndex.Value : 0];
            }
            else if (renderTarget is GLMultisampleRenderTarget)
            {
                framebuffer = (int)(properties.Get(renderTarget) as Hashtable)["glMultisampledFramebuffer"];
            }
            else
            {
                framebuffer = (int)(properties.Get(renderTarget) as Hashtable)["glFramebuffer"];
            }

            if (framebuffer != 0)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

                try
                {
                    var texture = renderTarget.Texture;
                    var textureFormat = texture.Format;
                    var textureType = texture.Type;

                    if (textureFormat != Constants.RGBAFormat)
                    {
                        //console.error('THREE.WebGLRenderer.readRenderTargetPixels: renderTarget is not in RGBA or implementation defined format.');
                        return;
                    }

                    // the following if statement ensures valid read requests (no out-of-bounds pixels, see Three.js Issue #8604)
                    if ((x >= 0 && x <= (renderTarget.Width - width)) && (y >= 0 && y <= (renderTarget.Height - height)))
                    {
                        GL.ReadPixels((int)x, (int)y, width, height, utils.Convert(textureFormat), utils.Convert(textureType), buffer);
                    }

                }
                finally
                {
                    // restore framebuffer of current render target if necessary=
                    if (_currentRenderTarget != null) 
                    {
                        if (renderTarget is GLCubeRenderTarget)
                        {
                            framebuffer = ((int[])(properties.Get(_currentRenderTarget) as Hashtable)["glFramebuffer"])[activeCubeFaceIndex != null ? activeCubeFaceIndex.Value : 0];
                        }
                        else if (renderTarget is GLMultisampleRenderTarget)
                        {
                            framebuffer = (int)(properties.Get(_currentRenderTarget) as Hashtable)["glMultisampledFramebuffer"];
                        }
                        else
                        {
                            framebuffer = (int)(properties.Get(_currentRenderTarget) as Hashtable)["glFramebuffer"];
                        }
                    }
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
                }
            }
        }

        public void CopyFramebufferToTexture(Vector2 position, Texture texture, int? level = null)
        {
            if (level == null) level = 0;

            var levelScale = (float)System.Math.Pow(2, -level.Value);
            var width = (float)System.Math.Floor(texture.Image.Width * levelScale);
            var height = (float)System.Math.Floor(texture.Image.Height * levelScale);
            var glFormat = utils.Convert(texture.Format);

            textures.SetTexture2D(texture, 0);

            GL.CopyTexImage2D(TextureTarget2d.Texture2D, level.Value, (TextureCopyComponentCount)glFormat, (int)position.X, (int)position.Y, (int)width, (int)height, 0);
            //GL.CopyTexImage2D(All.Texture2D, level.Value, glFormat, (int)position.X, (int)position.Y, (int)width, (int)height, 0);
            state.UnbindTexture();
        }

        private GLProgram SetProgram(Camera camera, Scene scene, Material material, Object3D object3D)
        {
            //if(scene.isScene!=true) scene = emptyScene;

            textures.ResetTextureUnits();

            Fog fog = scene.Fog;

            var environment = material is MeshStandardMaterial ? scene.Environment : null;
            var encoding = (_currentRenderTarget == null) ? outputEncoding : _currentRenderTarget.Texture.Encoding;
            var envMap = cubeMaps.Get(material.EnvMap!=null? material.EnvMap : environment);


            var materialProperties = properties.Get(material);

            var lights = CurrentRenderState.State.Lights;

            if (_clippingEnabled)
            {
                if (_localClippingEnabled || !camera.Equals(_currentCamera))
                {
                    var useCache = camera == _currentCamera && material.Id == _currentMaterialId;

                    // we might wnat to call this function with some ClippingGroup
                    // object instead of the material, once it becomes feasible
                    _clipping.SetState(material.ClippingPlanes, material.ClipIntersection, material.ClipShadows, camera, materialProperties, useCache);
                }
            }

            int version = materialProperties.ContainsKey("version") ? (int)materialProperties["version"] : -1;

            if (version == material.Version)
            {
               
                if (material.Fog && (Fog)materialProperties["fog"] != fog)
                {
                    InitMaterial(material, scene, object3D);
                }
                else if(materialProperties.ContainsKey("environment") && (Texture)materialProperties["environment"] != environment)
                {
                    InitMaterial(material, scene, object3D);
                }
                else if (materialProperties.ContainsKey("needsLights") && (bool)materialProperties["needsLights"])
                {
                    if (materialProperties.ContainsKey("lightsStateVersion") && lights.state.ContainsKey("version") && (int)materialProperties["lightsStateVersion"] != (int)lights.state["version"])
                    {
                        InitMaterial(material, scene, object3D);
                    }
                }
                else if (materialProperties.ContainsKey("numClippingPlanes") && ((int)materialProperties["numClippingPlanes"] != _clipping.numPlanes ||
                    (materialProperties.ContainsKey("numInterSection") && (int)materialProperties["numInterSection"] != _clipping.numIntersection)))
                {
                    InitMaterial(material, scene, object3D);
                }
                else if(materialProperties.ContainsKey("outputEncoding") && (int)materialProperties["outputEncoding"]!=encoding)
                {
                    InitMaterial(material, scene, object3D);
                }
                else if(materialProperties.ContainsKey("envMap") && (Texture)materialProperties["envMap"]!=envMap)
                {
                    InitMaterial(material, scene, object3D);
                }

            }
            else
            {
                InitMaterial(material, scene, object3D);
                materialProperties["version"] = material.Version;
            }

            GLProgram program = (GLProgram)materialProperties["program"];
            GLUniforms p_uniforms = program.GetUniforms();
            GLUniforms m_uniforms = materialProperties["uniforms"] as GLUniforms;


            if ((bool)materialProperties["needsLights"])
            {
                (m_uniforms["ambientLightColor"] as GLUniform)["value"] = lights.state["ambient"];
                (m_uniforms["lightProbe"] as GLUniform)["value"] = lights.state["probe"];
                (m_uniforms["directionalLights"] as GLUniform)["value"] = lights.state["directional"];
                (m_uniforms["directionalLightShadows"] as GLUniform)["value"] = lights.state["directionalShadow"];
                (m_uniforms["spotLights"] as GLUniform)["value"] = lights.state["spot"];
                (m_uniforms["spotLightShadows"] as GLUniform)["value"] = lights.state["spotShadow"];
                (m_uniforms["rectAreaLights"] as GLUniform)["value"] = lights.state["rectArea"];
                (m_uniforms["ltc_1"] as GLUniform)["value"] = lights.state["rectAreaLTC1"];
                (m_uniforms["ltc_2"] as GLUniform)["value"] = lights.state["rectAreaLTC2"];
                (m_uniforms["pointLights"] as GLUniform)["value"] = lights.state["point"];
                (m_uniforms["pointLightShadows"] as GLUniform)["value"] = lights.state["pointShadow"];
                (m_uniforms["hemisphereLights"] as GLUniform)["value"] = lights.state["hemi"];

                (m_uniforms["directionalShadowMap"] as GLUniform)["value"] = lights.state["directionalShadowMap"];
                (m_uniforms["directionalShadowMatrix"] as GLUniform)["value"] = lights.state["directionalShadowMatrix"];
                (m_uniforms["spotShadowMap"] as GLUniform)["value"] = lights.state["spotShadowMap"];
                (m_uniforms["spotShadowMatrix"] as GLUniform)["value"] = lights.state["spotShadowMatrix"];
                (m_uniforms["pointShadowMap"] as GLUniform)["value"] = lights.state["pointShadowMap"];
                (m_uniforms["pointShadowMatrix"] as GLUniform)["value"] = lights.state["pointShadowMatrix"];
            }
            var refreshProgram = false;
            var refreshMaterial = false;
            var refreshLights = false;

            if (state.UseProgram(program.program))
            {
                refreshProgram = true;
                refreshMaterial = true;
                refreshLights = true;
            }

            if (material.Id != _currentMaterialId)
            {
                _currentMaterialId = material.Id;
                refreshMaterial = true;
            }

            // When resizeing, it always need to apply camera ProjectionMatrix
            p_uniforms.SetProjectionMatrix(camera.ProjectionMatrix);

            if (refreshProgram || (_currentCamera != null && !_currentCamera.Equals(camera)))
            {
                //if (program.NumMultiviewViews > 0)
                //{
                //    Multiview.UpdateCameraProjectionMatricesUniform(camera, p_uniforms);
                //}
                //else
                //{
                //    // if screen was resized, camera.ProjectionMatrix was updated and this value is same to uniform["projectionMatrix"],
                //    // so those values are alwasys same because they is pointed at same address
                //    // single.setValue will not this change the matrix.Elements value to glsl memory

                //    //p_uniforms.SetValue("projectionMatrix", camera.ProjectionMatrix);
                //}

                if (capabilities.logarithmicDepthBuffer)
                {
                    p_uniforms.SetValue("logDepthBufFC", 2.0f / (System.Math.Log(camera.Far + 1.0) / System.Math.Log(2)));
                }

                if (_currentCamera == null || !_currentCamera.Equals(camera))
                {
                    _currentCamera = camera;

                    refreshMaterial = true;
                    refreshLights = true;
                }

                if (material is ShaderMaterial || 
                    material is MeshPhongMaterial || 
                    material is MeshToonMaterial || 
                    material is MeshStandardMaterial || material.EnvMap != null)
                {
                    SingleUniform uCamPos = p_uniforms["cameraPosition"] as SingleUniform;

                    if (uCamPos != null)
                    {
                        uCamPos.SetValue(Vector3.Zero().SetFromMatrixPosition(camera.MatrixWorld));
                    }
                }

                if (material is MeshPhongMaterial ||
                    material is MeshToonMaterial ||
                    material is MeshLambertMaterial ||
                    material is MeshBasicMaterial ||
                    material is MeshStandardMaterial ||
                    material is ShaderMaterial)
                {
                    p_uniforms.SetValue("isOrthographic", camera is OrthographicCamera);

                }

                if (material is MeshPhongMaterial ||
                    material is MeshToonMaterial ||
				    material is MeshLambertMaterial ||
				    material is MeshBasicMaterial ||
				    material is MeshStandardMaterial ||
				    material is ShaderMaterial ||
				    material.Skinning ) 
                {

				    //if ( program.NumMultiviewViews > 0 ) {

					   // Multiview.UpdateCameraViewMatricesUniform( camera, p_uniforms );

				    //} else {

					    p_uniforms.SetValue("viewMatrix", camera.MatrixWorldInverse );

				    //}

			    }

            }

            //skinning uniforms must be set even if material didn't change auto-setting of texture unit for bone texture must go before other textures
            // not sure why, but otherwise weird things happen

            if (material.Skinning)
            {
                p_uniforms.SetOptional(object3D, "bindMatrix" );
			    p_uniforms.SetOptional(object3D, "bindMatrixInverse" );

			    var skeleton = (object3D as SkinnedMesh).Skeleton;

			    if ( skeleton!=null ) {

				    var bones = skeleton.Bones;

				    if ( capabilities.floatVertexTextures) {

					    if ( skeleton.BoneTexture !=null ) {

						    // layout (1 matrix = 4 pixels)
						    //      RGBA RGBA RGBA RGBA (=> column1, column2, column3, column4)
						    //  with  8x8  pixel texture max   16 bones * 4 pixels =  (8 * 8)
						    //       16x16 pixel texture max   64 bones * 4 pixels = (16 * 16)
						    //       32x32 pixel texture max  256 bones * 4 pixels = (32 * 32)
						    //       64x64 pixel texture max 1024 bones * 4 pixels = (64 * 64)


						    var size = (float)System.Math.Sqrt( bones.Length * 4 ); // 4 pixels needed for 1 matrix
						    size = MathUtils.CeilPowerOfTwo( size );
						    size = System.Math.Max( size, 4 );

						    var boneMatrices = new float[ (int)(size * size * 4) ]; // 4 floats per RGBA pixel
						    Array.Copy(skeleton.BoneMatrices, boneMatrices,skeleton.BoneMatrices.Length ); // copy current values

                            TypeConverter tc = TypeDescriptor.GetConverter(typeof(System.Drawing.Bitmap));
                            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)tc.ConvertFrom(boneMatrices);

						    var boneTexture = new DataTexture(bitmap, (int)size, (int)size, Constants.RGBAFormat, Constants.FloatType);
                            
       
						    skeleton.BoneMatrices = boneMatrices;
						    skeleton.BoneTexture = boneTexture;
						    skeleton.BoneTextureSize = (int)size;

					    }

					    p_uniforms.SetValue("boneTexture", skeleton.BoneTexture, textures );
					    p_uniforms.SetValue("boneTextureSize", skeleton.BoneTextureSize );

				    } else {

					    p_uniforms.SetOptional(skeleton, "boneMatrices" );

				    }

			    }
            }

            if (refreshMaterial || (materialProperties.ContainsKey("receiveShadow") && (bool)materialProperties["receiveShadow"]) != object3D.ReceiveShadow)
            {
                materialProperties["receiveShadow"] = object3D.ReceiveShadow;
			    p_uniforms.SetValue("receiveShadow", object3D.ReceiveShadow );
            }

            if (refreshMaterial)
            {

                p_uniforms.SetValue("toneMappingExposure", ToneMappingExposure);
                //p_uniforms.SetValue("toneMappingWhitePoint",ToneMappingWhitePoint );

                if (materialProperties.ContainsKey("needsLights") && (bool)materialProperties["needsLights"])
                {

                    // the current material requires lighting info

                    // note: all lighting uniforms are always set correctly
                    // they simply reference the renderer's state for their
                    // values
                    //
                    // use the current material's .needsUpdate flags to set
                    // the GL state when required

                    MarkUniformsLightsNeedsUpdate(m_uniforms, refreshLights);

                }

                // refresh uniforms common to several materials

                if (fog != null && material.Fog)
                {

                    materials.RefreshFogUniforms(m_uniforms, fog);

                }

                materials.RefreshMaterialUniforms(m_uniforms, material, _pixelRatio, this.Height);

                if (ShaderLib.UniformsLib.ContainsKey("ltc_1")) (m_uniforms["ltc_1"] as GLUniform)["value"] = ShaderLib.UniformsLib["LTC_1"];
                if (ShaderLib.UniformsLib.ContainsKey("ltc_2")) (m_uniforms["ltc_2"] as GLUniform)["value"] = ShaderLib.UniformsLib["LTC_2"];

                //if (material is MeshLambertMaterial)
                //    Debug.WriteLine(material.type);
                GLUniforms.Upload((List<GLUniform>)materialProperties["uniformsList"], m_uniforms, textures);
            }
            if (material is ShaderMaterial && (material as ShaderMaterial).UniformsNeedUpdate == true)
            {

                GLUniforms.Upload((List<GLUniform>)materialProperties["uniformsList"], m_uniforms, textures);
                (material as ShaderMaterial).UniformsNeedUpdate = false;

            }

            if (material is SpriteMaterial)
            {
                p_uniforms.SetValue("center", (object3D as Sprite).Center);
            }

                

            p_uniforms.SetValue("modelViewMatrix", object3D.ModelViewMatrix);
            p_uniforms.SetValue("normalMatrix", object3D.NormalMatrix);
            p_uniforms.SetValue("modelMatrix", object3D.MatrixWorld);

            return program;
        }

       
        private void MarkUniformsLightsNeedsUpdate(GLUniforms uniforms, object value)
        {
            (uniforms["ambientLightColor"] as Hashtable)["needsUpdate"] = value;
            (uniforms["lightProbe"] as Hashtable)["needsUpdate"] = value;

            (uniforms["directionalLights"] as Hashtable)["needsUpdate"] = value;
            (uniforms["directionalLightShadows"] as Hashtable)["needsUpdate"] = value;
            (uniforms["pointLights"] as Hashtable)["needsUpdate"] = value;
            (uniforms["pointLightShadows"] as Hashtable)["needsUpdate"] = value;
            (uniforms["spotLights"] as Hashtable)["needsUpdate"] = value;
            (uniforms["spotLightShadows"] as Hashtable)["needsUpdate"] = value;
            (uniforms["rectAreaLights"] as Hashtable)["needsUpdate"] = value;
            (uniforms["hemisphereLights"] as Hashtable)["needsUpdate"] = value;
        }

        private bool MaterialNeedsLights(Material material)
        {
            return material is MeshLambertMaterial || material is MeshToonMaterial || material is MeshPhongMaterial || material is MeshStandardMaterial || material is ShadowMaterial
                || (material is ShaderMaterial && (material as ShaderMaterial).Lights == true);
        }


        public int GetActiveCubeFace()
        {
            return _currentActiveCubeFace;
        }

        public int GetActiveMipmapLevel()
        {
            return _currentActiveMipmapLevel;
        }
        public override void Dispose()
        {
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.Disable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.StencilTest);
            GL.Disable(EnableCap.SampleAlphaToCoverage);

            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
            GL.BlendFuncSeparate(BlendingFactorSrc.One, BlendingFactorDest.Zero, BlendingFactorSrc.One, BlendingFactorDest.Zero);

            GL.ColorMask(true, true, true, true);
            GL.ClearColor(0, 0, 0, 0);

            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Less);
            GL.ClearDepth(1);

            GL.StencilMask(0xffffffff);
            GL.StencilFunc(StencilFunction.Always, 0, 0xffffffff);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            GL.ClearStencil(0);

            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);

            GL.PolygonOffset(0, 0);

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (capabilities.IsGL2 == true)
            {

                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);

            }

            GL.UseProgram(0);

            GL.LineWidth(1);

            GL.Scissor(0, 0, Width, Height);
            GL.Viewport(0, 0, Width, Height);


            base.Dispose();
        }

    }
}
