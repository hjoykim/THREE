using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE;
using THREE.Cameras;
using THREE.Math;
using THREE.Scenes;
using THREE.Helpers;
using THREE.Geometries;
using THREE.Materials;
using THREE.Objects;
using OpenTK;
using THREE.Controls;
using THREE.Lights;
using THREE.Core;
using THREE.Loaders;
using Vector3 = THREE.Math.Vector3;
using System.Collections;
using Quaternion = THREE.Math.Quaternion;
using System.Windows.Forms;
using Vector2 = THREE.Math.Vector2;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace THREEExample.Three.Misc.Controls
{
    [Example("Controls-Transform-axis", ExampleCategory.Misc, "Controls")]
    public class TransformControlsAxisExample : Example
    {
        Scene scene;

        Camera camera;

        TrackballControls controls;

        TransformControls transformControl;

        public TransformControlsAxisExample() : base()
        {
            scene = new Scene();

        }
        private void InitRenderer()
        {
            this.renderer.SetClearColor(new Color().SetHex(0x000000));
            this.renderer.ShadowMap.Enabled = true;
            this.renderer.ShadowMap.type = Constants.PCFSoftShadowMap;

            renderer.Resize(glControl.Width, glControl.Height);
        }

        private void InitCamera()
        {
            camera = new PerspectiveCamera(50, glControl.AspectRatio, 0.01f, 30000); ;
            camera.Position.Set(1000, 500, 1000);
            camera.LookAt(0, 200, 0);
        }
        private void InitCameraController()
        {
            controls = new TrackballControls(this.glControl, camera);
            controls.StaticMoving = false;
            controls.RotateSpeed = 3.0f;
            controls.ZoomSpeed = 2;
            controls.PanSpeed = 2;
            controls.NoZoom = false;
            controls.NoPan = false;
            controls.NoRotate = false;
            controls.StaticMoving = true;
            controls.DynamicDampingFactor = 0.2f;
        }
        public virtual void InitLighting()
        {
            var light = new DirectionalLight(Color.Hex(0xffffff), 2);
            light.Position.Set(1, 1, 1);
            scene.Add(light);
        }
        public virtual void Init()
        {
            scene.Add(new GridHelper(1000, 10, 0x888888, 0x444444));



            var texture = TextureLoader.Load(@"../../../assets/textures/crate.gif");
            texture.Anisotropy = renderer.capabilities.GetMaxAnisotropy();

            var geometry = new BoxGeometry(200, 200, 200);
            var material = new MeshLambertMaterial();
            material.Transparent = true;
            material.Map = texture;
            material.DepthTest = false;
            material.DepthWrite = false;
            material.ToneMapped = false;
            var mesh = new Mesh(geometry, material);

            scene.Add(mesh);

            transformControl = new TransformControls(glControl,camera);
            transformControl.Attach(mesh);
            scene.Add(transformControl);

            //mesh = new Mesh(TranslateLineGeometry(), matRed.Clone()) { Position = new Vector3(0, 1e3f, 0), Scale = new Vector3(1e6f, 1, 1) };

            //scene.Add(mesh);

        }
        public override void Load(GLControl control)
        {
            base.Load(control);
            InitRenderer();

            InitCamera();

            InitCameraController();

            InitLighting();

            Init();

        }
        public override void Render()
        {

            transformControl.Update();

            controls.Update();

            this.renderer.Render(scene, camera);
        }
        public override void Resize(System.Drawing.Size clientSize)
        {
            base.Resize(clientSize);
            camera.Aspect = this.glControl.AspectRatio;
            camera.UpdateProjectionMatrix();
        }
    }
    class TransformControls : Object3D ,INotifyPropertyChanged
    { 
        #region Materials
        MeshBasicMaterial gizmoMaterial,
         matInvisible,
         matRed,
         matGreen,
         matBlue,
         matRedTransparent,
         matGreenTransparent,
         matBlueTransparent,
         matWhiteTransparent,
         matYellowTransparent,
         matYellow,
         matGray;
        LineBasicMaterial gizmoLineMaterial, matHelper;
        #endregion

        #region local variable hashtable
        Hashtable gizmoTranslate,
            pickerTranslate,
            helperTranslate,
            gizmoRotate,
            helperRotate,
            pickerRotate,
            gizmoScale,
            pickerScale,
            helperScale,
            gizmo,
            picker,
            helper;
           
        #endregion

       

        #region property
        private Camera _camera;
        public Camera camera
        {
            get { return _camera; }
            set
            {
                if (value != _camera)
                {
                    _camera = value;

                    OnPropertyChanged();
                }
            }
        }
        private Object3D _object3D;
        public Object3D object3D
        {
            get { return _object3D; }
            set
            {
                if (value != _object3D)
                {
                    _object3D = value;
                 
                    OnPropertyChanged();
                }
            }
        }
        private bool _enabled = true;
        public bool enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                  
                    OnPropertyChanged();
                }
            }
        }
        private string _axis;
        public string axis
        {
            get { return _axis; }
            set
            {
                if (value != _axis)
                {
                    _axis = value;

                    OnPropertyChanged();
                }
            }
        }
        private string _mode = "translate";
        public string mode
        {
            get { return _mode; }
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                  
                    OnPropertyChanged();
                }
            }
        }
        private float _translationSnap;
        public float translationSnap
        {
            get { return _translationSnap; }
            set
            {
                if (value != _translationSnap)
                {
                    _translationSnap = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private float _rotationSnap;
        public float rotationSnap
        {
            get { return _rotationSnap; }
            set
            {
                if (value != _rotationSnap)
                {
                    _rotationSnap = value;
                  
                    OnPropertyChanged();
                }
            }
        }

        private float _scaleSnap;
        public float scaleSnap
        {
            get { return _scaleSnap; }
            set
            {
                if (value != _scaleSnap)
                {
                    _scaleSnap = value;
                  
                    OnPropertyChanged();
                }
            }
        }
        private string _space = "world";
        public string space
        {
            get { return _space; }
            set
            {
                if (value != _space)
                {
                    _space = value;
                  
                    OnPropertyChanged();
                }
            }
        }
        private float _size = 1;
        public float size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    _size = value;
                  
                    OnPropertyChanged();
                }
            }
        }
        private bool _dragging = false;
        public bool dragging
        {
            get { return _dragging; }
            set
            {
                if (value != _dragging)
                {
                    _dragging = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private bool _showX = true;
        public bool showX
        {
            get { return _showX; }
            set
            {
                if (value != _showX)
                {
                    _showX = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private bool _showY = true;
        public bool showY
        {
            get { return _showY; }
            set
            {
                if (value != _showY)
                {
                    _showY = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private bool _showZ = true;
        public bool showZ
        {
            get { return _showZ; }
            set
            {
                if (value != _showZ)
                {
                    _showZ = value;
                   
                    OnPropertyChanged();
                }
            }
        }

        private Vector3 _worldPosition = new Vector3();
        public Vector3 worldPosition
        {
            get { return _worldPosition; }
            set
            {
                if (value != _worldPosition)
                {
                    _worldPosition = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _worldPositionStart = new Vector3();
        public Vector3 worldPositionStart
        {
            get { return _worldPositionStart; }
            set
            {
                if (value != _worldPositionStart)
                {
                    _worldPositionStart = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Quaternion _worldQuaternion = new Quaternion();
        public Quaternion worldQuaternion
        {
            get { return _worldQuaternion; }
            set
            {
                if (value != _worldQuaternion)
                {
                    _worldQuaternion = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Quaternion _worldQuaternionStart = new Quaternion();
        public Quaternion worldQuaternionStart
        {
            get { return _worldQuaternionStart; }
            set
            {
                if (value != _worldQuaternionStart)
                {
                    _worldQuaternionStart = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _cameraPosition = new Vector3();
        public Vector3 cameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                if (value != _cameraPosition)
                {
                    _cameraPosition = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Quaternion _cameraQuaternion = new Quaternion();
        public Quaternion cameraQuaternion
        {
            get { return _cameraQuaternion; }
            set
            {
                if (value != _cameraQuaternion)
                {
                    _cameraQuaternion = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _pointStart = new Vector3();
        public Vector3 pointStart
        {
            get { return _pointStart; }
            set
            {
                if (value != _pointStart)
                {
                    _pointStart = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _pointEnd = new Vector3();
        public Vector3 pointEnd
        {
            get { return _pointEnd; }
            set
            {
                if (value != _pointEnd)
                {
                    _pointEnd = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _rotationAxis = new Vector3();
        public Vector3 rotationAxis
        {
            get { return _rotationAxis; }
            set
            {
                if (value != _rotationAxis)
                {
                    _rotationAxis = value;
                  
                    OnPropertyChanged();
                }
            }
        }
        private float _rotationAngle = 0;
        public float rotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                if (value != _rotationAngle)
                {
                    _rotationAngle = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        private Vector3 _eye = new Vector3();
        public Vector3 eye
        {
            get { return _eye; }
            set
            {
                if (value != _eye)
                {
                    _eye = value;
                   
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region resuable variable
        Vector3 _tempVector = new Vector3();
        Vector3 _tempVector2 = new Vector3();
        Quaternion _tempQuaternion = new Quaternion();
        Hashtable _unit = new Hashtable() {
            {"X", new Vector3( 1, 0, 0 ) },
            {"Y", new Vector3( 0, 1, 0 ) },
            {"Z", new Vector3( 0, 0, 1 ) }
        };
        Vector3 _offset = new Vector3();
        Vector3 _startNorm = new Vector3();
        Vector3 _endNorm = new Vector3();
        Vector3 _cameraScale = new Vector3();


        Vector3 _parentPosition = new Vector3();
        Quaternion _parentQuaternion = new Quaternion();
        Quaternion _parentQuaternionInv = new Quaternion();
        Vector3 _parentScale = new Vector3();

        Vector3 _worldScaleStart = new Vector3();
        Quaternion _worldQuaternionInv = new Quaternion();
        Vector3 _worldScale = new Vector3();

        Vector3 _positionStart = new Vector3();
        Quaternion _quaternionStart = new Quaternion();
        Vector3 _scaleStart = new Vector3();

        Raycaster _raycaster = new Raycaster();
        Quaternion _identityQuaternion = new Quaternion();

        Vector2 mouse = new Vector2();
        #endregion

        #region mouse action
        public Action<string> _mouseDownEvent;
        public Action<string> _mouseUpEvent;
        public Action<object> _changeEvent;
        public Action<object> _objectChangeEvent;
        #endregion

        Control glControl;

        public TransformControls(Control glControl,Camera camera) : base()
        {
            this.glControl = glControl;
            this.camera = camera;
 
            #region init material
            gizmoMaterial = new MeshBasicMaterial()
            {
                DepthTest = false,
                DepthWrite = false,
                Fog = false,
                ToneMapped = false,
                Transparent = true

            };

            gizmoLineMaterial = new LineBasicMaterial()
            {
                DepthTest = false,
                DepthWrite = false,
                Fog = false,
                ToneMapped = false,
                Transparent = true

            };

            gizmo = new Hashtable();
            picker = new Hashtable();
            helper = new Hashtable();

            matInvisible = gizmoMaterial.Clone();
            matInvisible.Opacity = 0.15f;

            matHelper = gizmoLineMaterial.Clone();
            matHelper.Opacity = 0.5f;

            matRed = gizmoMaterial.Clone();
            matRed.Color = Color.Hex(0xff0000);

            matGreen = gizmoMaterial.Clone();
            matGreen.Color = Color.Hex(0x00ff00);

            matBlue = gizmoMaterial.Clone();
            matBlue.Color = Color.Hex(0x0000ff);

            matRedTransparent = gizmoMaterial.Clone();
            matRedTransparent.Color = Color.Hex(0xff0000);
            matRedTransparent.Opacity = 0.5f;

            matGreenTransparent = gizmoMaterial.Clone();
            matGreenTransparent.Color = Color.Hex(0x00ff00);
            matGreenTransparent.Opacity = 0.5f;

            matBlueTransparent = gizmoMaterial.Clone();
            matBlueTransparent.Color = Color.Hex(0x0000ff);
            matBlueTransparent.Opacity = 0.5f;

            matWhiteTransparent = gizmoMaterial.Clone();
            matWhiteTransparent.Color = Color.Hex(0xffffff);
            matWhiteTransparent.Opacity = 0.25f;

            matYellowTransparent = gizmoMaterial.Clone();
            matYellowTransparent.Color = Color.Hex(0xffff00);
            matYellowTransparent.Opacity = 0.25f;

            matYellow = gizmoMaterial.Clone();
            matYellow.Color = Color.Hex(0xffff00);

            matGray = gizmoMaterial.Clone();
            matGray.Color = Color.Hex(0x787878);
            #endregion

            #region translate
            gizmoTranslate = new Hashtable() {
                { "X",new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0,0.05f,0), matRed), new Vector3(0.5f, 0, 0), new Euler(0, 0,-(float)System.Math.PI / 2)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matRed), new Vector3( -0.5f, 0, 0), new Euler(0, 0,(float)System.Math.PI / 2)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0,0.25f,0), matRed), new Vector3(0, 0, 0), new Euler(0, 0, -(float)System.Math.PI / 2 ) }
                    }
                },
                {"Y", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matGreen), new Vector3(0, 0.5f, 0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matGreen), new Vector3(0, -0.5f, 0), new Euler((float)System.Math.PI, 0, 0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0) as Geometry, matGreen)}
                    }
                },
                {"Z", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matBlue), new Vector3(0, 0, 0.5f),new Euler((float)System.Math.PI/2,0,0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0, 0.04f, 0.1f, 12).Translate(0, 0.05f, 0), matBlue), new Vector3(0, 0, -0.5f), new Euler(-(float)System.Math.PI/2, 0, 0)},
                        new List<object>(){ new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0), matBlue),null, new Euler((float)System.Math.PI / 2, 0, 0) }
                    }

                },
                {"XYZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.1f,0),matWhiteTransparent.Clone()),new Vector3(0, 0, 0)}
                    }
                },
                {"XY",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.15f,0.15f,0.01f),matBlueTransparent.Clone()),new Vector3(0.15f, 0.15f, 0)}
                    }
                },
                {"YZ",new List<List<object>>()
                    {
                       new List<object>(){ new Mesh(new BoxGeometry(0.15f,0.15f,0.01f),matRedTransparent.Clone()),new Vector3(0, 0.15f, 0.15f),new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {"XZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.15f,0.15f,0.01f),matGreenTransparent.Clone()),new Vector3(0.15f, 0, 0.15f),new Euler(-(float)System.Math.PI/2,0,0)}
                    }
                }

            };
            pickerTranslate = new Hashtable() {
                { "X",new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0.3f, 0, 0), new Euler(0, 0,-(float)System.Math.PI / 2)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(-0.3f, 0, 0), new Euler(0, 0,(float)System.Math.PI / 2)}
                    }
                },
                {"Y", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,0.3f, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,-0.3f,0), new Euler(0, 0,(float)System.Math.PI)}
                    }
                },
                {"Z", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, 0.3f), new Euler((float)System.Math.PI / 2, 0, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, -0.3f), new Euler(-(float)System.Math.PI / 2, 0, 0)}
                    }

                },
                {"XYZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.2f,0),matInvisible)},
                    }
                },
                {"XY",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f,0.2f,0.01f),matInvisible),new Vector3(0.15f, 0.15f, 0)}
                    }
                },
                {"YZ",new List<List<object>>()
                    {
                       new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f), matInvisible),new Vector3(0, 0.15f, 0.15f),new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {"XZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f),matInvisible),new Vector3(0.15f, 0, 0.15f),new Euler(-(float)System.Math.PI/2,0,0)}
                    }
                }

            };
            helperTranslate = new Hashtable()
            {
                {
                    "START", new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.01f,2),matHelper),null,null,null,"helper"}
                    }
                },
                {
                    "END", new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new OctahedronGeometry(0.01f,2),matHelper),null,null,null,"helper"}
                    }
                },
                {
                    "DELTA", new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(TranslateHelperGeometry(),matHelper),null,null,null,"helper"}
                    }
                },
                {
                    "X", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(-1e3f,0,0),null,new Vector3(1e6f,1,1),"helper"}
                    }
                },
                {
                    "Y", new List<List<object>>()
                        {
                            new List<object>(){ new Line(TranslateLineGeometry(),matHelper.Clone()),new Vector3(0,-1e3f,0),new Euler(0,0,(float)System.Math.PI/2),new Vector3(1e6f,1,1),"helper"}
                        }
                },
                {
                    "Z", new List<List<object>>()
                        {
                            new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(0,0,-1e3f),new Euler(0,-(float)System.Math.PI/2,0),new Vector3(1e6f,1,1),"helper"}
                        }
                }
            };
            #endregion

            #region rotate
            gizmoRotate = new Hashtable()
            {
                {
                    "XYZE",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,1),matGray),null,new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {
                    "X",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,0.5f),matRed)}
                    }
                },
                {
                    "Y",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,0.5f),matGreen),null,new Euler(0,0,-(float)System.Math.PI/2)}
                    }
                },
                {
                    "Z",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.5f,0.5f),matBlue),null,new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {
                    "E",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(CircleGeometry(0.75f,1),matYellowTransparent),null,new Euler(0,(float)System.Math.PI/2,0)}
                    }
                }

            };
            helperRotate = new Hashtable()
            {
                {
                    "AXIS" ,new List<List<object>>()
                    {
                        new List<object>{new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(-1e3f,0,0),null,new Vector3(1e6f,1,1),"helper" }
                    }
                }
            };
            pickerRotate = new Hashtable()
            {
                {
                    "XYZE",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new SphereGeometry(0.25f,10,8),matInvisible)}
                    }
                },
                {
                    "X",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.5f,0.1f,4,24),matInvisible),new Vector3(0,0,0),new Euler(0,-(float)System.Math.PI/2,-(float)System.Math.PI/2)}
                    }
                },
                {
                    "Y",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.5f,0.1f,4,24),matInvisible),new Vector3(0,0,0),new Euler((float)System.Math.PI/2,0,0)}
                    }
                },
                {
                    "Z",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.5f,0.1f,4,24),matInvisible),new Vector3(0,0,0),new Euler(0,0,-(float)System.Math.PI/2)}
                    }
                },
                {
                    "E",new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new TorusGeometry(0.75f,0.1f,4,24),matInvisible)}
                    }
                },
            };
            #endregion

            #region scale
            gizmoScale = new Hashtable()
            {
                {
                    "X", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0,0.04f,0), matRed),new Vector3(0.5f,0,0),new Euler(0,0,-(float)System.Math.PI/2)},
                        new List<object>() { new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0,0.25f,0), matRed),new Vector3(0,0,0),new Euler(0,0,-(float)System.Math.PI/2)},
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matRed),new Vector3(-0.5f,0,0),new Euler(0,0,(float)System.Math.PI/2)},
                    }
                },
                {
                    "Y", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matGreen),new Vector3(0,0.5f,0)},
                        new List<object>() { new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0), matGreen)},
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matGreen),new Vector3(0,-0.5f,0),new Euler(0,0,(float)System.Math.PI)},
                    }
                },
                {
                    "Z", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matBlue),new Vector3(0,0,0.5f),new Euler((float)System.Math.PI/2,0,0)},
                        new List<object>() { new Mesh(new CylinderGeometry(0.0075f, 0.0075f, 0.5f, 3).Translate(0, 0.25f, 0), matBlue),new Vector3(0,0,0),new Euler((float)System.Math.PI/2,0,0)},
                        new List<object>() { new Mesh(new BoxGeometry(0.08f, 0.08f, 0.08f).Translate(0, 0.04f, 0), matBlue),new Vector3(0,0,-0.5f),new Euler(-(float)System.Math.PI/2,0,0)},
                    }
                },
                {
                    "XY", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.15f,0.15f,0.1f),matBlueTransparent),new Vector3(0.15f,0.15f,0) }
                    }
                },
                {
                    "YZ", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.15f,0.15f,0.1f),matBlueTransparent),new Vector3(0,0.15f,0.15f),new Euler(0,(float)System.Math.PI/2,0) }
                    }
                },
                {
                    "XZ", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.15f,0.15f,0.1f),matBlueTransparent),new Vector3(0.15f,0,0.15f),new Euler(-(float)System.Math.PI/2,0,0) }
                    }
                },
                {
                    "XYZ", new List<List<object>>()
                    {
                        new List<object>() { new Mesh(new BoxGeometry(0.1f,0.1f,0.1f),(MeshBasicMaterial)matWhiteTransparent.Clone()) }
                    }
                },
            };

            pickerScale = new Hashtable() {
                { "X",new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0.3f, 0, 0), new Euler(0, 0,-(float)System.Math.PI / 2)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(-0.3f, 0, 0), new Euler(0, 0,(float)System.Math.PI / 2)}
                    }
                },
                {"Y", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,0.3f, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0,-0.3f,0), new Euler(0, 0,(float)System.Math.PI)}
                    }
                },
                {"Z", new List<List<object>>()
                    {
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, 0.3f), new Euler((float)System.Math.PI / 2, 0, 0)},
                        new List<object>() {new Mesh(new CylinderGeometry(0.2f,0,0.6f,4), matInvisible), new Vector3(0, 0, -0.3f), new Euler(-(float)System.Math.PI / 2, 0, 0)}
                    }

                },
                {"XY",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f,0.2f,0.01f),matInvisible),new Vector3(0.15f, 0.15f, 0)}
                    }
                },
                {"YZ",new List<List<object>>()
                    {
                       new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f), matInvisible),new Vector3(0, 0.15f, 0.15f),new Euler(0,(float)System.Math.PI/2,0)}
                    }
                },
                {"XZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f, 0.2f, 0.01f),matInvisible),new Vector3(0.15f, 0, 0.15f),new Euler(-(float)System.Math.PI/2,0,0)}
                    }
                },
                {"XYZ",new List<List<object>>()
                    {
                        new List<object>(){ new Mesh(new BoxGeometry(0.2f,0.2f,0.2f),matInvisible),new Vector3(0,0,0)}
                    }
                }

            };

            helperScale = new Hashtable()
            {
                {
                    "X", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(-1e3f,0,0),null,new Vector3(1e6f,1,1),"helper"}
                    }
                },
                {
                    "Y", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(0,-1e3f,0),new Euler(0,0,(float)System.Math.PI/2),new Vector3(1e6f,1,1),"helper"}
                    }
                },
                {
                    "Z", new List<List<object>>()
                    {
                        new List<object>(){ new Line(TranslateLineGeometry(), matHelper.Clone()),new Vector3(0,0,-1e3f),new Euler(0,-(float)System.Math.PI/2,0),new Vector3(1e6f,1,1),"helper"}
                    }
                }
            };
            #endregion

            gizmo.Add("translate", SetupGizmo(gizmoTranslate));
            helper.Add("translate", SetupGizmo(helperTranslate));
            picker.Add("translate", SetupGizmo(pickerTranslate));



            //gizmo.Add("rotate", SetupGizmo(gizmoRotate));           
            //helper.Add("rotate", SetupGizmo(helperRotate));
            picker.Add("rotate", SetupGizmo(pickerRotate));

            //gizmo.Add("scale", SetupGizmo(gizmoScale));           
            //helper.Add("scale", SetupGizmo(helperScale));
            picker.Add("scale", SetupGizmo(pickerScale));

            Add(gizmo["translate"] as Object3D);
            Add(helper["translate"] as Object3D);
            Add(picker["translate"] as Object3D);

            //this.Add(gizmo["rotate"] as Object3D);           
            //this.Add(helper["rotate"] as Object3D);
            Add(picker["rotate"] as Object3D);

            //this.Add(gizmo["scale"] as Object3D);           
            //this.Add(helper["scale"] as Object3D);
            Add(picker["scale"] as Object3D);

            // Pickers should be hidden always
            (picker["translate"] as Object3D).Visible = false;
            (picker["rotate"] as Object3D).Visible = false;
            (picker["scale"] as Object3D).Visible = false;

            glControl.MouseMove += OnPointerHover;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void OnPointerHover(object sender, MouseEventArgs e)
        {
            //var pointer = GetPointer(sender,e);
            PointerHover(sender, e);
        }
        private void PointerHover(object sender, MouseEventArgs e)
        {

            if (this.object3D == null || this.dragging == true) return;
            //if (this.dragging == true) return;
            //camera.LookAt(this.Parent.Position);
            //camera.UpdateMatrixWorld();

            mouse.X = e.X * 1.0f / (sender as GLControl).Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / ((sender as GLControl).Height) * 2 + 1.0f;

            _raycaster.SetFromCamera(mouse, camera);

            var intersect = IntersectObjectWithRay(this.picker[this.mode] as Object3D, _raycaster);

            if (intersect != null)
            {

                this.axis = intersect.object3D.Name;
                //Debug.WriteLine("axis:" + axis);

            }
            else
            {

                this.axis = null;

            }
        }

        public override Object3D Attach(Object3D object3D)
        {
            this.object3D = object3D;
            this.Visible = true;

            return this;
        }
        private Intersection IntersectObjectWithRay(Object3D object3d, Raycaster raycaster, bool includeInvisible = false)
        {

            var allIntersections = raycaster.IntersectObject(object3d, true);

            for (int i = 0; i < allIntersections.Count; i++)
            {

                if (allIntersections[i].object3D.Visible || includeInvisible)
                {

                    return allIntersections[i];

                }

            }

            return null;

        }
        private Object3D SetupGizmo(Hashtable gizmoMap)
        {

            var gizmo = new Object3D();

            #region test cide
            //var objectX = new Line(TranslateLineGeometry(), matRed);
            //objectX.Name = "X";
            //objectX.Tag = "helper";
            //objectX.Position.Set(-1e3f, 0, 0);
            //objectX.Scale.Set(1e6f, 1, 1);
            //objectX.UpdateMatrix();


            //objectX.Geometry.ApplyMatrix4(objectX.Matrix);
            //objectX.RenderOrder = int.MaxValue;

            //objectX.Position.Set(0, 0, 0);
            //objectX.Rotation.Set(0, 0, 0);
            //objectX.Scale.Set(1, 1, 1);

            //gizmo.Add(objectX);

            //var objectX = new Line(TranslateLineGeometry(), matRed);
            //objectX.Name = "Y";
            //objectX.Tag = "helper";
            //objectX.Position.Set(0, -1e3f, 0);
            //objectX.Rotation.Set(0, 0, (float)System.Math.PI / 2);
            //objectX.UpdateMatrix();

            //objectX.Geometry.ApplyMatrix4(objectX.Matrix);
            //objectX.RenderOrder = int.MaxValue;

            //objectX.Position.Set(0, 0, 0);
            //objectX.Rotation.Set(0, 0, 0);
            //objectX.Scale.Set(1, 1, 1);

            //gizmo.Add(objectX);

            //return gizmo;
            #endregion

            foreach (string name in gizmoMap.Keys)
            {
                List<List<object>> list = (List<List<object>>)gizmoMap[name];
                for (int i = 0; i < list.Count; i++)
                {
                    Object3D object3D;
                    if (list[i][0] is Mesh) object3D = list[i][0] as Mesh;
                    else object3D = list[i][0] as Line;
                    //var position = list[1] as Vector3;
                    //const rotation = gizmoMap[name][i][2];
                    //const scale = gizmoMap[name][i][3];
                    if (list[i].Count > 4 && list[i][4] != null)
                        object3D.Tag = list[i][4] as string;

                    // name and tag properties are essential for picking and updating logic.
                    object3D.Name = name;
                    //object.tag = tag;

                    if (list[i].Count > 1 && list[i][1] != null) // position
                    {
                        var position = list[i][1] as Vector3;
                        object3D.Position.Copy(position);

                    }

                    if (list[i].Count > 2 && list[i][2] != null) // rotation
                    {
                        var rotation = list[i][2] as Euler;
                        object3D.Rotation.Set(rotation.X, rotation.Y, rotation.Z);

                    }

                    if (list[i].Count > 3 && list[i][3] != null) // scale
                    {
                        var scale = list[i][3] as Vector3;
                        object3D.Scale.Copy(scale);

                    }

                    object3D.UpdateMatrix();

                    //var tempGeometry = object3D.Geometry.Clone() as Geometry;
                    //tempGeometry.ApplyMatrix4(object3D.Matrix);
                    object3D.Geometry.ApplyMatrix4(object3D.Matrix);// = tempGeometry;
                    object3D.RenderOrder = int.MaxValue;
                    if (object3D.Tag == null || !(object3D.Tag as string).Equals("helper"))
                    {
                        object3D.Position.Set(0, 0, 0);
                        object3D.Rotation.Set(0, 0, 0);
                        object3D.Scale.Set(1, 1, 1);
                    }


                    gizmo.Add(object3D);

                }

            }

            return gizmo;

        }
        private TorusGeometry CircleGeometry(float radius, float arc)
        {
            var geometry = new TorusGeometry(radius, 0.0075f, 3, 64, arc * (float)System.Math.PI * 2);
            geometry.RotateY((float)System.Math.PI / 2);
            geometry.RotateX((float)System.Math.PI / 2);
            return geometry;
        }

        // Special geometry for transform helper. If scaled with position vector it spans from [0,0,0] to position
        private BufferGeometry TranslateHelperGeometry()
        {

            var geometry = new BufferGeometry();

            geometry.SetAttribute("position", new BufferAttribute<float>(new float[] { 0, 0, 0, 1, 1, 1 }, 3));

            return geometry;

        }
        private BufferGeometry TranslateLineGeometry()
        {
            var geometry = new BufferGeometry();
            geometry.SetAttribute("position", new BufferAttribute<float>(new float[] { -1, 0, 0, 1, 0, 0 }, 3));

            return geometry;
        }    


        public void Update()
        {
            if(this.object3D!=null)
            {
                object3D.UpdateMatrixWorld();

                if(object3D.Parent == null)
                {
                    Debug.Fail("TransformControls : The attached 3D object must be a part of the scene graph");
                }
                else
                {
                    object3D.Parent.MatrixWorld.Decompose(_parentPosition, _parentQuaternion, _parentScale);
                }
                this.object3D.MatrixWorld.Decompose(worldPosition, worldQuaternion, _worldScale);
                _parentQuaternionInv.Copy(_parentQuaternion).Invert();
                _worldQuaternionInv.Copy(worldQuaternion).Invert();
            }
            camera.UpdateMatrixWorld();
            camera.MatrixWorld.Decompose(cameraPosition, cameraQuaternion, _cameraScale);



            //Quaternion quaternion = space == "local" ? worldQuaternion : _identityQuaternion;
            Quaternion quaternion = worldQuaternion;
            List<Object3D> handles = new List<Object3D>();
            handles = handles.Concat((this.picker[mode] as Object3D).Children);
            handles = handles.Concat((this.gizmo[mode] as Object3D).Children);
            handles = handles.Concat((this.helper[mode] as Object3D).Children);
            for (int i = 0; i < handles.Count; i++)
            {
                Object3D handle = handles[i];

                //hide aligned to camera               

                float factor;
                if (camera is OrthographicCamera)
                {
                    factor = (camera.Top - camera.Bottom) / camera.Zoom;
                }
                else
                {
                    factor = worldPosition.DistanceTo(cameraPosition) * (float)(Math.Min(1.9f * System.Math.Tan(Math.PI * camera.Fov / 360) / camera.Zoom, 7));
                }
                handle.Visible = true;



                if (handle.Tag == null || !(handle.Tag as string).Equals("helper"))
                {
                    handle.Scale.Set(1, 1, 1).MultiplyScalar(factor * size / 4);
                    //handle.Rotation.Set(0, 0, 0);
                    handle.Position.Copy(worldPosition);
                }

                if(handle.Tag!=null && (handle.Tag as string).Equals("helper"))
                {
                    handle.Visible = false;
                    if(handle.Name.Equals("AXIS"))
                    {
                        if(axis.Equals("X"))
                        {

                        }
                        if (axis.Equals("Y"))
                        {

                        }
                        if (axis.Equals("Z"))
                        {

                        }
                        if (axis.Equals("XYZE"))
                        {

                        }
                        if (axis.Equals("E"))
                        {

                        }
                    }
                    else if(handle.Name.Equals("START"))
                    {

                    }
                    else if (handle.Name.Equals("END"))
                    {

                    }
                    else if (handle.Name.Equals("DELTA"))
                    {

                    }
                    else
                    {
                        //handle.Quaternion.Copy(quaternion);

                        if (dragging)
                        {

                            handle.Position.Copy(worldPositionStart);

                        }
                        else
                        {

                            handle.Position.Copy(worldPosition);

                        }

                        if (axis != null)
                        {

                            handle.Visible = axis.IndexOf(handle.Name) != -1;

                        }
                    }
                    // if updating helper, skip rest of the loop
                    continue;
                }

                // Align handlers to current local or world quaternion

                handle.Quaternion.Copy(quaternion);
            }
        }
    }
}
