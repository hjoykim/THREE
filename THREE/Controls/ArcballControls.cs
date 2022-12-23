using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using THREE;

namespace THREE
{
	public class ArcballControls : IDisposable
	{
		#region member
		public enum STATE
		{
			IDLE,
			ROTATE,
			PAN,
			SCALE,
			FOV,
			FOCUS,
			ZROTATE,
			TOUCH_MULTI,
			ANIMATION_FOCUS,
			ANIMATION_ROTATE,
			NONE
		}

		public enum INPUT
		{
			NONE,
			ONE_FINGER,
			ONE_FINGER_SWITCHED,
			TWO_FINGER,
			MULT_FINGER,
			CURSOR
		}

		public struct Center
		{
			public float x;
			public float y;
		}
		Center _center;

		public class Transformation
		{
			public Matrix4 Camera = new Matrix4();
			public Matrix4 gizmos = new Matrix4();
		}

		Transformation _transformation = new Transformation();

		struct MouseAction
		{
			public string operation;
			public string mouse;
			public string key;
			public STATE state;
		}
		
		public Action ChangeEvent;
		public Action StartEvent;
		public Action EndEvent;

		Raycaster _raycaster = new Raycaster();
		Vector3 _offset = new Vector3();

		Matrix4 _gizmoMatrixStateTemp = new Matrix4();
		Matrix4 _cameraMatrixStateTemp = new Matrix4();
		Vector3 _scalePointTemp = new Vector3();

		Control glControl;
		Camera camera;
		Scene scene;

		Vector3 target = new Vector3();
		Vector3 _currentTarget = new Vector3();
		float radiusFactor = 0.67f;

		public List<string> OperationInput = new List<string>() { "PAN", "ROTATE", "ZOOM", "FOV" };
		public List<string> MouseInput = new List<string>() { "LEFT", "MIDDLE", "RIGHT", "WHEEL" };
		public List<string> KeyInput = new List<string>() { "CTRL", "SHIFT", null };

		List<MouseAction> mouseActions = new List<MouseAction>();
		string _mouseOp = null;
		//global vectors and matrices that are used in some operations to avoid creating new objects every time (e.g. every time cursor moves)
		Vector2 _v2_1 = new Vector2();
		Vector3 _v3_1 = new Vector3();
		Vector3 _v3_2 = new Vector3();

		Matrix4 _m4_1 = new Matrix4();
		Matrix4 _m4_2 = new Matrix4();

		Quaternion _quat = new Quaternion();

		//transformation matrices
		Matrix4 _translationMatrix = new Matrix4(); //matrix for translation operation
		Matrix4 _rotationMatrix = new Matrix4(); //matrix for rotation operation
		Matrix4 _scaleMatrix = new Matrix4(); //matrix for scaling operation

		Vector3 _rotationAxis = new Vector3(); //axis for rotate operation


		//camera state
		Matrix4 _cameraMatrixState = new Matrix4();
		Matrix4 _cameraProjectionState = new Matrix4();

		float _fovState = 1;
		Vector3 _upState = new Vector3();
		float _zoomState = 1;
		float _nearPos = 0;
		float _farPos = 0;

		Matrix4 _gizmoMatrixState = new Matrix4();

		//initial values
		Vector3 _up0 = new Vector3();
		float _zoom0 = 1;
		float _fov0 = 0;
		float _initialNear = 0;
		float _nearPos0 = 0;
		float _initialFar = 0;
		float _farPos0 = 0;
		Matrix4 _cameraMatrixState0 = new Matrix4();
		Matrix4 _gizmoMatrixState0 = new Matrix4();

		//pointers array
		string _button = "";
		List<MouseEventArgs> _touchStart = new List<MouseEventArgs>();
		List<MouseEventArgs> _touchCurrent = new List<MouseEventArgs>();
		INPUT _input = INPUT.NONE;

		//two fingers touch interaction
		float _switchSensibility = 32;  //minimum movement to be performed to fire single pan start after the second finger has been released
		float _startFingerDistance = 0; //distance between two fingers
		float _currentFingerDistance = 0;
		float _startFingerRotation = 0; //amount of rotation performed with two fingers
		float _currentFingerRotation = 0;

		//double tap
		float _devPxRatio = 1;
		bool _downValid = true;
		float _nclicks = 0;

		List<MouseEventArgs> _downEvents = new List<MouseEventArgs>();
		List<long> _downEventsTime = new List<long>();
		long _downStart = 0;    //pointerDown time
		float _clickStart = 0;  //first click time
		float _maxDownTime = 250;
		float _maxInterval = 300;
		float _posThreshold = 24;
		float _movementThreshold = 24;

		//cursor positions
		Vector3 _currentCursorPosition = new Vector3();
		Vector3 _startCursorPosition = new Vector3();

		//grid
		GridHelper _grid = null; //grid to be visualized during pan operation
		Vector3 _gridPosition = new Vector3();

		//gizmos
		public Group _gizmos = new Group();
		int _curvePts = 128;


		//animations
		float _timeStart = -1; //initial time
		int _animationId = -1;

		//focus animation
		int focusAnimationTime = 500; //duration of focus animation in ms

		//rotate animation
		long _timePrev = 0; //time at which previous rotate operation has been detected
		long _timeCurrent = 0; //time at which current rotate operation has been detected
		float _anglePrev = 0; //angle of previous rotation
		float _angleCurrent = 0; //angle of current rotation
		Vector3 _cursorPosPrev = new Vector3(); //cursor position when previous rotate operation has been detected
		Vector3 _cursorPosCurr = new Vector3();//cursor position when current rotate operation has been detected
		float _wPrev = 0; //angular velocity of the previous rotate operation
		float _wCurr = 0; //angular velocity of the current rotate operation


		//parameters
		public bool AdjustNearFar = false;
		public float ScaleFactor = 1.1f;   //zoom/distance multiplier
		float dampingFactor = 25;
		float wMax = 20;    //maximum angular velocity allowed
		bool enableAnimations = true; //if animations should be performed
		public bool EnableGrid = false; //if grid should be showed during pan operation
		public bool CursorZoom = false;    //if wheel zoom should be cursor centered
		float minFov = 5;
		float maxFov = 90;

		public bool Enabled = true;
		public bool EnablePan = true;
		public bool EnableRotate = true;
		public bool EnableZoom = true;
		public bool EnableGizmos = true;

		public float MinDistance = 0;
		public float MaxDistance = float.PositiveInfinity;
		public float MinZoom = 0;
		public float MaxZoom = float.PositiveInfinity;

		//trackball parameters
		float _tbRadius = 1;

		protected readonly Stopwatch stopWatch = new Stopwatch();
		//FSA
		STATE _state = STATE.IDLE;

		#endregion
		public ArcballControls(Control glControl, Camera camera, Scene scene = null)
		{
			this.glControl = glControl;
			this.scene = scene;

			_center.x = 0;
			_center.y = 0;

			SetCamera(camera);

			if(this.scene!=null)
            {
				this.scene.Add(_gizmos);
            }
			InitializeMouseActions();

			glControl.MouseWheel += OnWheel;
			glControl.MouseDown += OnPointerDown;
			glControl.MouseUp += OnPointerCancel;
			glControl.SizeChanged += Control_SizeChanged;

			glControl.KeyDown += OnKeyDown;
			glControl.KeyUp += OnKeyUp;
			stopWatch.Start();

		}
		/**
		 * Set default mouse actions
		 */
		private void InitializeMouseActions()
		{

			this.SetMouseAction("PAN", "LEFT", "CTRL");
			this.SetMouseAction("PAN", "RIGHT");

			this.SetMouseAction("ROTATE", "LEFT");

			this.SetMouseAction("ZOOM", "WHEEL");
			this.SetMouseAction("ZOOM", "MIDDLE");

			this.SetMouseAction("FOV", "WHEEL", "SHIFT");
			this.SetMouseAction("FOV", "MIDDLE", "SHIFT");

		}


		/**
		 * Set a new mouse action by specifying the operation to be performed and a mouse/key combination. In case of conflict, replaces the existing one
		 * @param {String} operation The operation to be performed ('PAN', 'ROTATE', 'ZOOM', 'FOV)
		 * @param {*} mouse A mouse button (0, 1, 2) or 'WHEEL' for wheel notches
		 * @param {*} key The keyboard modifier ('CTRL', 'SHIFT') or null if key is not needed
		 * @returns {Boolean} True if the mouse action has been successfully added, false otherwise
		 */
		private bool SetMouseAction(string operation, string mouse, string key = null)
		{


			STATE state = STATE.NONE;

			if (!OperationInput.Contains(operation) || !MouseInput.Contains(mouse) || !KeyInput.Contains(key))
			{

				//invalid parameters
				return false;

			}

			if (mouse == "WHEEL")
			{

				if (operation != "ZOOM" && operation != "FOV")
				{

					//cannot associate 2D operation to 1D input
					return false;

				}

			}

			switch (operation)
			{

				case "PAN":

					state = STATE.PAN;
					break;

				case "ROTATE":

					state = STATE.ROTATE;
					break;

				case "ZOOM":

					state = STATE.SCALE;
					break;

				case "FOV":

					state = STATE.FOV;
					break;

			}

			MouseAction action = new MouseAction()
			{

				operation = operation,
				mouse = mouse,
				key = key,
				state = state

			};

			for (int i = 0; i < this.mouseActions.Count; i++)
			{

				if (this.mouseActions[i].mouse == action.mouse && this.mouseActions[i].key == action.key)
				{

					this.mouseActions.Insert(i, action);
					return true;

				}

			}

			this.mouseActions.Add(action);
			return true;

		}
		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
				this.controlKey = false;
			if (e.KeyCode == Keys.ShiftKey)
				this.shiftKey = true;
		}
		private bool controlKey = false;
		private bool shiftKey = false;

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ControlKey)
				this.controlKey = true;
			if (e.KeyCode == Keys.ShiftKey)
				this.shiftKey = true;

		}
		#region Dispose      
		public event EventHandler<EventArgs> Disposed;
		public virtual void Dispose()
		{
			glControl.MouseDown -= OnPointerDown;
			glControl.MouseUp -= OnPointerCancel;
			glControl.MouseUp -= OnPointerUp;
			glControl.MouseMove -= OnPointerMove;
			glControl.SizeChanged -= Control_SizeChanged;
			glControl.MouseWheel -= OnWheel;
			DisposeGrid();
			stopWatch.Stop();
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
				this.disposed = true;
			}
		}
		#endregion



		private void SetCenter(float x, float y)
		{
			_center.x = x;
			_center.y = y;

		}

		private void OnPointerCancel(object sender, MouseEventArgs e)
		{
			this._touchStart.Clear();
			this._touchCurrent.Clear();
			this._input = INPUT.NONE;
		}



		private void DrawGrid()
		{
			if (this.scene != null)
			{

				Color color = Color.Hex(0x888888);
				var multiplier = 3;
				float size, divisions, maxLength, tick;

				if (this.camera is OrthographicCamera)
				{

					var width = this.camera.CameraRight - this.camera.Left;
					var height = this.camera.Bottom - this.camera.Top;

					maxLength = (float)System.Math.Max(width, height);
					tick = maxLength / 20;

					size = maxLength / this.camera.Zoom * multiplier;
					divisions = size / tick * this.camera.Zoom;

				}
				else //if (this.camera is PerspectiveCamera)
				{

					var distance = this.camera.Position.DistanceTo(this._gizmos.Position);
					var halfFovV = MathUtils.DEG2RAD * this.camera.Fov * 0.5f;
					var halfFovH = (float)System.Math.Atan((this.camera.Aspect) * System.Math.Tan(halfFovV));

					maxLength = (float)System.Math.Tan(System.Math.Max(halfFovV, halfFovH)) * distance * 2;
					tick = maxLength / 20;

					size = maxLength * multiplier;
					divisions = size / tick;

				}

				if (this._grid == null)
				{

					this._grid = new GridHelper((int)size, (int)divisions, 0x888888, 0x888888);
					this._grid.Position.Copy(this._gizmos.Position);
					this._gridPosition.Copy(this._grid.Position);
					this._grid.Quaternion.Copy(this.camera.Quaternion);
					this._grid.RotateX((float)System.Math.PI * 0.5f);

					this.scene.Add(this._grid);

				}

			}
		}

		/**
		 * Update a PointerEvent inside current pointerevents array
		 * @param {PointerEvent} event
		 */
		private void UpdateTouchEvent(MouseEventArgs e)
		{
			for (int i = 0; i < this._touchCurrent.Count; i++)
			{

				if (this._touchCurrent[i].Equals(e))
				{

					this._touchCurrent.Insert(i, e);
					break;
				}
			}
		}
		private void DisposeGrid()
		{
			if (this._grid != null && this.scene != null)
			{

				this.scene.Remove(this._grid);
				this._grid = null;

			}
		}
		private void Control_SizeChanged(object sender, EventArgs e)
		{
			var scale = (this._gizmos.Scale.X + this._gizmos.Scale.Y + this._gizmos.Scale.Z) / 3;
			this._tbRadius = this.CalculateTbRadius(this.camera);

			var newRadius = this._tbRadius / scale;
			var curve = new EllipseCurve(0, 0, newRadius, newRadius);
			var points = curve.GetPoints(this._curvePts);
			var curveGeometry = new BufferGeometry().SetFromPoints(points);


			foreach (var gizmo in this._gizmos.Children)
			{
				gizmo.Geometry = curveGeometry;
				//this._gizmos.children[gizmo].geometry = curveGeometry;

			}

			if (ChangeEvent != null)
				ChangeEvent();

			//this.dispatchEvent(_changeEvent);

		}

		/**
		 * Calculate the distance between two pointers
		 * @param {PointerEvent} p0 The first pointer
		 * @param {PointerEvent} p1 The second pointer
		 * @returns {number} The distance between the two pointers
		 */
		private float CalculatePointersDistance(MouseEventArgs p0, MouseEventArgs p1)
		{
			return (float)System.Math.Sqrt(System.Math.Pow(p1.X - p0.X, 2) + System.Math.Pow(p1.Y - p0.Y, 2));

		}






		/**
         * Set gizmos visibility
         * @param {Boolean} value Value of gizmos visibility
         */
		public void SetGizmosVisible(bool value)
		{
			_gizmos.Visible = value;

			if (ChangeEvent != null)
				ChangeEvent();
		}

		/**
		 * Set gizmos radius factor and redraws gizmos
		 * @param {Float} value Value of radius factor
		 */
		public void SetTbRadius(float value)
		{

			this.radiusFactor = value;
			this._tbRadius = this.CalculateTbRadius(this.camera);

			var curve = new EllipseCurve(0, 0, this._tbRadius, this._tbRadius);
			var points = curve.GetPoints(this._curvePts);
			var curveGeometry = new BufferGeometry().SetFromPoints(points);


			foreach (var gizmo in this._gizmos.Children)
			{

				gizmo.Geometry = curveGeometry;

			}

			if (ChangeEvent != null)
				ChangeEvent();

		}
		public void SetCamera(Camera camera)
		{
			camera.LookAt(this.target);
			camera.UpdateMatrix();

			//setting state
			if (camera is PerspectiveCamera)
			{

				this._fov0 = camera.Fov;
				this._fovState = camera.Fov;

			}

			this._cameraMatrixState0.Copy(camera.Matrix);
			this._cameraMatrixState.Copy(this._cameraMatrixState0);
			this._cameraProjectionState.Copy(camera.ProjectionMatrix);
			this._zoom0 = camera.Zoom;
			this._zoomState = this._zoom0;

			this._initialNear = camera.Near;
			this._nearPos0 = camera.Position.DistanceTo(this.target) - camera.Near;
			this._nearPos = this._initialNear;

			this._initialFar = camera.Far;
			this._farPos0 = camera.Position.DistanceTo(this.target) - camera.Far;
			this._farPos = this._initialFar;

			this._up0.Copy(camera.Up);
			this._upState.Copy(camera.Up);

			this.camera = camera;
			this.camera.UpdateProjectionMatrix();

			//making gizmos
			this._tbRadius = this.CalculateTbRadius(camera);
			this.MakeGizmos(this.target, this._tbRadius);
		}

		/**
		 * Creates the rotation gizmos matching trackball center and radius
		 * @param {Vector3} tbCenter The trackball center
		 * @param {number} tbRadius The trackball radius
		 */
		private void MakeGizmos(Vector3 tbCenter, float tbRadius)
		{
			var curve = new EllipseCurve(0, 0, tbRadius, tbRadius);
			var points = curve.GetPoints(this._curvePts);

			//geometry
			var curveGeometry = new BufferGeometry().SetFromPoints(points);

			//material
			var curveMaterialX = new LineBasicMaterial() { Color = Color.Hex(0xff8080), Fog = true, Transparent = true, Opacity = 0.6f };
			var curveMaterialY = new LineBasicMaterial() { Color = Color.Hex(0x80ff80), Fog = true, Transparent = true, Opacity = 0.6f };
			var curveMaterialZ = new LineBasicMaterial() { Color = Color.Hex(0x8080ff), Fog = true, Transparent = true, Opacity = 0.6f };

			//line
			var gizmoX = new Line(curveGeometry, curveMaterialX);
			var gizmoY = new Line(curveGeometry, curveMaterialY);
			var gizmoZ = new Line(curveGeometry, curveMaterialZ);

			var rotation = (float)System.Math.PI * 0.5f;
			gizmoX.Rotation.X = rotation;
			gizmoY.Rotation.Y = rotation;


			;//setting state
			this._gizmoMatrixState0 = Matrix4.Identity().SetPosition(tbCenter);
			this._gizmoMatrixState.Copy(this._gizmoMatrixState0);

			if (this.camera.Zoom != 1)
			{

				//adapt gizmos size to camera zoom
				var size = 1 / this.camera.Zoom;
				this._scaleMatrix.MakeScale(size, size, size);
				this._translationMatrix.MakeTranslation(-tbCenter.X, -tbCenter.Y, -tbCenter.Z);

				this._gizmoMatrixState.PreMultiply(this._translationMatrix).PreMultiply(this._scaleMatrix);
				this._translationMatrix.MakeTranslation(tbCenter.X, tbCenter.Y, tbCenter.Z);
				this._gizmoMatrixState.PreMultiply(this._translationMatrix);

			}

			this._gizmoMatrixState.Decompose(this._gizmos.Position, this._gizmos.Quaternion, this._gizmos.Scale);

			this._gizmos.Clear();

			this._gizmos.Add(gizmoX);
			this._gizmos.Add(gizmoY);
			this._gizmos.Add(gizmoZ);
		}
		/**
		 * Perform animation for focus operation
		 * @param {Number} time Instant in which this function is called as performance.now()
		 * @param {Vector3} point Point of interest for focus operation
		 * @param {Matrix4} cameraMatrix Camera matrix
		 * @param {Matrix4} gizmoMatrix Gizmos matrix
		 */
		public void OnnFocusAnim(float time, Vector3 point, Matrix4 cameraMatrix, Matrix4 gizmoMatrix)
		{

			if (this._timeStart == -1)
			{

				//animation start
				this._timeStart = time;

			}

			if (this._state == STATE.ANIMATION_FOCUS)
			{

				var deltaTime = time - this._timeStart;
				var animTime = deltaTime / this.focusAnimationTime;

				this._gizmoMatrixState.Copy(gizmoMatrix);

				if (animTime >= 1)
				{

					//animation end

					this._gizmoMatrixState.Decompose(this._gizmos.Position, this._gizmos.Quaternion, this._gizmos.Scale);

					this.Focus(point, this.ScaleFactor);

					this._timeStart = -1;
					this.UpdateTbState(STATE.IDLE, false);
					this.ActivateGizmos(false);

					if (ChangeEvent != null)
						ChangeEvent();

				}
				else
				{

					var amount = this.EaseOutCubic(animTime);
					var size = ((1 - amount) + (this.ScaleFactor * amount));

					this._gizmoMatrixState.Decompose(this._gizmos.Position, this._gizmos.Quaternion, this._gizmos.Scale);
					this.Focus(point, size, amount);

					if (ChangeEvent != null)
						ChangeEvent();
					//const self = this;
					//this._animationId = window.requestAnimationFrame(function(t) {

					//	self.onFocusAnim(t, point, cameraMatrix, gizmoMatrix.clone());

					//} );

				}

			}
			else
			{

				//interrupt animation

				this._animationId = -1;
				this._timeStart = -1;

			}

		}
		/**
		 * Perform animation for rotation operation
		 * @param {Number} time Instant in which this function is called as performance.now()
		 * @param {Vector3} rotationAxis Rotation axis
		 * @param {number} w0 Initial angular velocity
		 */
		public void OnRotationAnim(float time, Vector3 rotationAxis, float w0)
		{
			if (this._timeStart == -1)
			{

				//animation start
				this._anglePrev = 0;
				this._angleCurrent = 0;
				this._timeStart = time;

			}

			if (this._state == STATE.ANIMATION_ROTATE)
			{

				//w = w0 + alpha * t
				var deltaTime = (time - this._timeStart) / 1000;
				var w = w0 + ((-this.dampingFactor) * deltaTime);

				if (w > 0)
				{

					//tetha = 0.5 * alpha * t^2 + w0 * t + tetha0
					this._angleCurrent = 0.5f * (-this.dampingFactor) * (float)System.Math.Pow(deltaTime, 2) + w0 * deltaTime + 0;
					this.ApplyTransformMatrix(this.Rotate(rotationAxis, this._angleCurrent));

					if (ChangeEvent != null)
						ChangeEvent();

					//const self = this;
					//this._animationId = window.requestAnimationFrame(function(t) {

					//	self.onRotationAnim(t, rotationAxis, w0);

					//} );

				}
				else
				{

					this._animationId = -1;
					this._timeStart = -1;

					this.UpdateTbState(STATE.IDLE, false);
					this.ActivateGizmos(false);

					if (ChangeEvent != null)
						ChangeEvent();


				}

			}
			else
			{

				//interrupt animation

				this._animationId = -1;
				this._timeStart = -1;

				if (this._state != STATE.ROTATE)
				{

					this.ActivateGizmos(false);
					if (ChangeEvent != null)
						ChangeEvent();

				}

			}
		}
		/**
		 * Rotate the camera around an axis passing by trackball's center
		 * @param {Vector3} axis Rotation axis
		 * @param {number} angle Angle in radians
		 * @returns {Object} Object with 'camera' field containing transformation matrix resulting from the operation to be applied to the camera
		 */
		private Transformation Rotate(Vector3 axis, float angle)
		{
			var point = this._gizmos.Position; //rotation center
			this._translationMatrix.MakeTranslation(-point.X, -point.Y, -point.Z);
			this._rotationMatrix.MakeRotationAxis(axis, -angle);

			//rotate camera
			this._m4_1.MakeTranslation(point.X, point.Y, point.Z);
			this._m4_1.Multiply(this._rotationMatrix);
			this._m4_1.Multiply(this._translationMatrix);

			this.SetTransformationMatrices(this._m4_1);

			return _transformation;
		}

		/**
		 * Compute the easing out cubic function for ease out effect in animation
		 * @param {Number} t The absolute progress of the animation in the bound of 0 (beginning of the) and 1 (ending of animation)
		 * @returns {Number} Result of easing out cubic at time t
		 */
		private float EaseOutCubic(float t)
		{
			return 1 - (float)System.Math.Pow(1 - t, 3);
		}

		/**
		 * Make rotation gizmos more or less visible
		 * @param {Boolean} isActive If true, make gizmos more visible
		 */
		private void ActivateGizmos(bool isActive)
		{
			var gizmoX = this._gizmos.Children[0];
			var gizmoY = this._gizmos.Children[1];
			var gizmoZ = this._gizmos.Children[2];

			if (isActive)
			{

				gizmoX.Material.Opacity = 1;
				gizmoY.Material.Opacity = 1;
				gizmoZ.Material.Opacity = 1;

			}
			else
			{

				gizmoX.Material.Opacity = 0.6f;
				gizmoY.Material.Opacity = 0.6f;
				gizmoZ.Material.Opacity = 0.6f;

			}
		}

		/**
		 * Update the trackball FSA
		 * @param {STATE} newState New state of the FSA
		 * @param {Boolean} updateMatrices If matriices state should be updated
		 */
		private void UpdateTbState(STATE newState, bool updateMatrices)
		{
			this._state = newState;
			if (updateMatrices)
			{

				this.UpdateMatrixState();

			}
		}
		/**
		 * Update camera and gizmos state
		 */
		private void UpdateMatrixState()
		{
			//update camera and gizmos state
			this._cameraMatrixState.Copy(this.camera.Matrix);
			this._gizmoMatrixState.Copy(this._gizmos.Matrix);

			if (this.camera is OrthographicCamera)
			{

				this._cameraProjectionState.Copy(this.camera.ProjectionMatrix);
				this.camera.UpdateProjectionMatrix();
				this._zoomState = this.camera.Zoom;

			}
			else if (this.camera is PerspectiveCamera)
			{

				this._fovState = this.camera.Fov;

			}
		}

		/**
		 * Focus operation consist of positioning the point of interest in front of the camera and a slightly zoom in
		 * @param {Vector3} point The point of interest
		 * @param {Number} size Scale factor
		 * @param {Number} amount Amount of operation to be completed (used for focus animations, default is complete full operation)
		 */
		private void Focus(Vector3 point, float size, float amount = 1)
		{
			//move center of camera (along with gizmos) towards point of interest
			_offset.Copy(point).Sub(this._gizmos.Position).MultiplyScalar(amount);
			this._translationMatrix.MakeTranslation(_offset.X, _offset.Y, _offset.Z);

			_gizmoMatrixStateTemp.Copy(this._gizmoMatrixState);
			this._gizmoMatrixState.PreMultiply(this._translationMatrix);
			this._gizmoMatrixState.Decompose(this._gizmos.Position, this._gizmos.Quaternion, this._gizmos.Scale);

			_cameraMatrixStateTemp.Copy(this._cameraMatrixState);
			this._cameraMatrixState.PreMultiply(this._translationMatrix);
			this._cameraMatrixState.Decompose(this.camera.Position, this.camera.Quaternion, this.camera.Scale);

			//apply zoom
			if (this.EnableZoom)
			{

				this.ApplyTransformMatrix(this.Scale(size, this._gizmos.Position));

			}

			this._gizmoMatrixState.Copy(_gizmoMatrixStateTemp);
			this._cameraMatrixState.Copy(_cameraMatrixStateTemp);
		}

		private Transformation Scale(float size, Vector3 point, bool scaleGizmos = true)
		{
			_scalePointTemp.Copy(point);
			float sizeInverse = 1 / size;

			if (this.camera is OrthographicCamera)
			{

				//camera zoom
				this.camera.Zoom = this._zoomState;
				this.camera.Zoom *= size;

				//check min and max zoom
				if (this.camera.Zoom > this.MaxZoom)
				{

					this.camera.Zoom = this.MaxZoom;
					sizeInverse = this._zoomState / this.MaxZoom;

				}
				else if (this.camera.Zoom < this.MinZoom)
				{

					this.camera.Zoom = this.MinZoom;
					sizeInverse = this._zoomState / this.MinZoom;

				}

				this.camera.UpdateProjectionMatrix();

				this._v3_1.SetFromMatrixPosition(this._gizmoMatrixState);   //gizmos position

				//scale gizmos so they appear in the same spot having the same dimension
				this._scaleMatrix.MakeScale(sizeInverse, sizeInverse, sizeInverse);
				this._translationMatrix.MakeTranslation(-this._v3_1.X, -this._v3_1.Y, -this._v3_1.Z);

				this._m4_2.MakeTranslation(this._v3_1.X, this._v3_1.Y, this._v3_1.Z).Multiply(this._scaleMatrix);
				this._m4_2.Multiply(this._translationMatrix);


				//move camera and gizmos to obtain pinch effect
				_scalePointTemp.Sub(this._v3_1);

				var amount = _scalePointTemp.Clone().MultiplyScalar(sizeInverse);
				_scalePointTemp.Sub(amount);

				this._m4_1.MakeTranslation(_scalePointTemp.X, _scalePointTemp.Y, _scalePointTemp.Z);
				this._m4_2.PreMultiply(this._m4_1);

				this.SetTransformationMatrices(this._m4_1, this._m4_2);

				return _transformation;

			}
			else if (this.camera is PerspectiveCamera)
			{

				this._v3_1.SetFromMatrixPosition(this._cameraMatrixState);
				this._v3_2.SetFromMatrixPosition(this._gizmoMatrixState);

				//move camera
				var distance = this._v3_1.DistanceTo(_scalePointTemp);
				var amount = distance - (distance * sizeInverse);

				//check min and max distance
				var newDistance = distance - amount;
				if (newDistance < this.MinDistance)
				{

					sizeInverse = this.MinDistance / distance;
					amount = distance - (distance * sizeInverse);

				}
				else if (newDistance > this.MaxDistance)
				{

					sizeInverse = this.MaxDistance / distance;
					amount = distance - (distance * sizeInverse);

				}

				_offset.Copy(_scalePointTemp).Sub(this._v3_1).Normalize().MultiplyScalar(amount);

				this._m4_1.MakeTranslation(_offset.X, _offset.Y, _offset.Z);


				if (scaleGizmos)
				{

					//scale gizmos so they appear in the same spot having the same dimension
					var pos = this._v3_2;

					distance = pos.DistanceTo(_scalePointTemp);
					amount = distance - (distance * sizeInverse);
					_offset.Copy(_scalePointTemp).Sub(this._v3_2).Normalize().MultiplyScalar(amount);

					this._translationMatrix.MakeTranslation(pos.X, pos.Y, pos.Z);
					this._scaleMatrix.MakeScale(sizeInverse, sizeInverse, sizeInverse);

					this._m4_2.MakeTranslation(_offset.X, _offset.Y, _offset.Z).Multiply(this._translationMatrix);
					this._m4_2.Multiply(this._scaleMatrix);

					this._translationMatrix.MakeTranslation(-pos.X, -pos.Y, -pos.Z);

					this._m4_2.Multiply(this._translationMatrix);
					this.SetTransformationMatrices(this._m4_1, this._m4_2);


				}
				else
				{

					this.SetTransformationMatrices(this._m4_1);

				}

				return _transformation;

			}
			else
			{
				return null;
			}
		}
		/**
		 * Set camera fov
		 * @param {Number} value fov to be setted
		 */
		public void SetFov(float value)
		{

			if (this.camera is PerspectiveCamera)
			{

				this.camera.Fov = MathUtils.Clamp(value, this.minFov, this.maxFov);
				this.camera.UpdateProjectionMatrix();

			}

		}
		/**
		 * Set values in transformation object
		 * @param {Matrix4} camera Transformation to be applied to the camera
		 * @param {Matrix4} gizmos Transformation to be applied to gizmos
		 */
		private void SetTransformationMatrices(Matrix4 camera = null, Matrix4 gizmos = null)
		{
			if (camera != null)
			{

				if (_transformation.Camera != null)
				{

					_transformation.Camera.Copy(camera);

				}
				else
				{

					_transformation.Camera = (Matrix4)camera.Clone();

				}

			}
			else
			{

				_transformation.Camera = null;

			}

			if (gizmos != null)
			{

				if (_transformation.gizmos != null)
				{

					_transformation.gizmos.Copy(gizmos);

				}
				else
				{

					_transformation.gizmos = (Matrix4)gizmos.Clone();

				}

			}
			else
			{

				_transformation.gizmos = null;

			}
		}

		/**
		 * Rotate camera around its direction axis passing by a given point by a given angle
		 * @param {Vector3} point The point where the rotation axis is passing trough
		 * @param {Number} angle Angle in radians
		 * @returns The computed transormation matix
		 */
		public Transformation ZRotate(Vector3 point, float angle)
		{

			this._rotationMatrix.MakeRotationAxis(this._rotationAxis, angle);
			this._translationMatrix.MakeTranslation(-point.X, -point.Y, -point.Z);

			this._m4_1.MakeTranslation(point.X, point.Y, point.Z);
			this._m4_1.Multiply(this._rotationMatrix);
			this._m4_1.Multiply(this._translationMatrix);

			this._v3_1.SetFromMatrixPosition(this._gizmoMatrixState).Sub(point);    //vector from rotation center to gizmos position
			this._v3_2.Copy(this._v3_1).ApplyAxisAngle(this._rotationAxis, angle);  //apply rotation
			this._v3_2.Sub(this._v3_1);

			this._m4_2.MakeTranslation(this._v3_2.X, this._v3_2.Y, this._v3_2.Z);

			this.SetTransformationMatrices(this._m4_1, this._m4_2);
			return _transformation;

		}

		public Raycaster GetRaycaster()
		{
			return _raycaster;
		}

		/**
		* Unproject the cursor on the 3D object surface
		* @param {Vector2} cursor Cursor coordinates in NDC
		* @param {Camera} camera Virtual camera
		* @returns {Vector3} The point of intersection with the model, if exist, null otherwise
		*/
		public Vector3 UnprojectOnObj(Vector2 cursor, Camera camera)
		{

			_raycaster.near = camera.Near;
			_raycaster.far = camera.Far;
			_raycaster.SetFromCamera(cursor, camera);

			var intersect = _raycaster.IntersectObjects(this.scene.Children, true);

			for (int i = 0; i < intersect.Count; i++)
			{

				if (intersect[i].object3D.Uuid != this._gizmos.Uuid && intersect[i].face != null)
				{

					return intersect[i].point.Clone();

				}

			}

			return null;

		}
		
		/**
		* Unproject the cursor on the trackball surface
		* @param {Camera} camera The virtual camera
		* @param {Number} cursorX Cursor horizontal coordinate on screen
		* @param {Number} cursorY Cursor vertical coordinate on screen
		* @param {HTMLElement} canvas The canvas where the renderer draws its output
		* @param {number} tbRadius The trackball radius
		* @returns {Vector3} The unprojected point on the trackball surface
		*/
		private Vector3 UnprojectOnTbSurface(Camera camera, float cursorX, float cursorY, float tbRadius)
		{

			if (camera is OrthographicCamera)
			{

				this._v2_1.Copy(this.GetCursorPosition(cursorX, cursorY));
				this._v3_1.Set(this._v2_1.X, this._v2_1.Y, 0);

				var x2 = (float)System.Math.Pow(this._v2_1.X, 2);
				var y2 = (float)System.Math.Pow(this._v2_1.Y, 2);
				var r2 = (float)System.Math.Pow(this._tbRadius, 2);

				if (x2 + y2 <= r2 * 0.5)
				{

					//intersection with sphere
					this._v3_1.SetZ((float)System.Math.Sqrt(r2 - (x2 + y2)));

				}
				else
				{

					//intersection with hyperboloid
					this._v3_1.SetZ((r2 * 0.5f) / ((float)System.Math.Sqrt(x2 + y2)));

				}

				return this._v3_1;

			}
			else// if (camera.type == 'PerspectiveCamera' ) {
			{
				//unproject cursor on the near plane
				this._v2_1.Copy(this.GetCursorNDC(cursorX, cursorY));

				this._v3_1.Set(this._v2_1.X, this._v2_1.Y, -1);
				this._v3_1.ApplyMatrix4(camera.ProjectionMatrixInverse);

				var rayDir = this._v3_1.Clone().Normalize(); //unprojected ray direction
				var cameraGizmoDistance = camera.Position.DistanceTo(this._gizmos.Position);
				var radius2 = (float)System.Math.Pow(tbRadius, 2);

				//	  camera
				//		|\
				//		| \
				//		|  \
				//	h	|	\
				//		| 	 \
				//		| 	  \
				//	_ _ | _ _ _\ _ _  near plane
				//			l

				var h = this._v3_1.Z;
				var l = (float)System.Math.Sqrt(System.Math.Pow(this._v3_1.X, 2) + System.Math.Pow(this._v3_1.Y, 2));

				if (l == 0)
				{

					//ray aligned with camera
					rayDir.Set(this._v3_1.X, this._v3_1.Y, tbRadius);
					return rayDir;

				}

				var m = h / l;
				var q = cameraGizmoDistance;

				/*
				 * calculate intersection point between unprojected ray and trackball surface
				 *|y = m * x + q
				 *|x^2 + y^2 = r^2
				 *
				 * (m^2 + 1) * x^2 + (2 * m * q) * x + q^2 - r^2 = 0
				 */
				var a = (float)System.Math.Pow(m, 2) + 1;
				var b = 2 * m * q;
				var c = (float)System.Math.Pow(q, 2) - radius2;
				var delta = (float)System.Math.Pow(b, 2) - (4 * a * c);

				if (delta >= 0)
				{

					//intersection with sphere
					this._v2_1.SetX((-b - (float)System.Math.Sqrt(delta)) / (2 * a));
					this._v2_1.SetY(m * this._v2_1.X + q);

					var angle = MathUtils.RAD2DEG * this._v2_1.Angle();

					if (angle >= 45)
					{

						//if angle between intersection point and X' axis is >= 45°, return that point
						//otherwise, calculate intersection point with hyperboloid

						var rayLength = (float)System.Math.Sqrt(System.Math.Pow(this._v2_1.X, 2) + System.Math.Pow((cameraGizmoDistance - this._v2_1.Y), 2));
						rayDir.MultiplyScalar(rayLength);
						rayDir.Z += cameraGizmoDistance;
						return rayDir;

					}

				}

				//intersection with hyperboloid
				/*
				 *|y = m * x + q
				 *|y = (1 / x) * (r^2 / 2)
				 *
				 * m * x^2 + q * x - r^2 / 2 = 0
				 */

				a = m;
				b = q;
				c = -radius2 * 0.5f;
				delta = (float)System.Math.Pow(b, 2) - (4 * a * c);
				this._v2_1.SetX((-b - (float)System.Math.Sqrt(delta)) / (2 * a));
				this._v2_1.SetY(m * this._v2_1.X + q);

				var rayLength1 = (float)System.Math.Sqrt(System.Math.Pow(this._v2_1.X, 2) + System.Math.Pow((cameraGizmoDistance - this._v2_1.Y), 2));

				rayDir.MultiplyScalar(rayLength1);
				rayDir.Z += cameraGizmoDistance;
				return rayDir;

			}

		}


		/**
		 * Unproject the cursor on the plane passing through the center of the trackball orthogonal to the camera
		 * @param {Camera} camera The virtual camera
		 * @param {Number} cursorX Cursor horizontal coordinate on screen
		 * @param {Number} cursorY Cursor vertical coordinate on screen
		 * @param {HTMLElement} canvas The canvas where the renderer draws its output
		 * @param {Boolean} initialDistance If initial distance between camera and gizmos should be used for calculations instead of current (Perspective only)
		 * @returns {Vector3} The unprojected point on the trackball plane
		 */
		public Vector3 UnprojectOnTbPlane(Camera camera, float cursorX, float cursorY, bool initialDistance = false)
		{
			if (camera is OrthographicCamera)
			{

				this._v2_1.Copy(this.GetCursorPosition(cursorX, cursorY));
				this._v3_1.Set(this._v2_1.X, this._v2_1.Y, 0);

				return this._v3_1.Clone();

			}
			else if (camera is PerspectiveCamera)
			{

				this._v2_1.Copy(this.GetCursorNDC(cursorX, cursorY));

				//unproject cursor on the near plane
				this._v3_1.Set(this._v2_1.X, this._v2_1.Y, -1);
				this._v3_1.ApplyMatrix4(camera.ProjectionMatrixInverse);

				var rayDir = this._v3_1.Clone().Normalize(); //unprojected ray direction

				//	  camera
				//		|\
				//		| \
				//		|  \
				//	h	|	\
				//		| 	 \
				//		| 	  \
				//	_ _ | _ _ _\ _ _  near plane
				//			l

				var h = this._v3_1.Z;
				var l = (float)System.Math.Sqrt(System.Math.Pow(this._v3_1.X, 2) + System.Math.Pow(this._v3_1.Y, 2));
				float cameraGizmoDistance;

				if (initialDistance)
				{

					cameraGizmoDistance = this._v3_1.SetFromMatrixPosition(this._cameraMatrixState0).DistanceTo(this._v3_2.SetFromMatrixPosition(this._gizmoMatrixState0));

				}
				else
				{

					cameraGizmoDistance = camera.Position.DistanceTo(this._gizmos.Position);

				}

				/*
				 * calculate intersection point between unprojected ray and the plane
				 *|y = mx + q
				 *|y = 0
				 *
				 * x = -q/m
				*/
				if (l == 0)
				{

					//ray aligned with camera
					rayDir.Set(0, 0, 0);
					return rayDir;

				}

				var m = h / l;
				var q = cameraGizmoDistance;
				var x = -q / m;

				var rayLength = (float)System.Math.Sqrt(System.Math.Pow(q, 2) + System.Math.Pow(x, 2));
				rayDir.MultiplyScalar(rayLength);
				rayDir.Z = 0;
				return rayDir;

			}

			return null;

		}

		/**
		 * Calculate the cursor position inside the canvas x/y coordinates with the origin being in the center of the canvas
		 * @param {Number} x Cursor horizontal coordinate within the canvas
		 * @param {Number} y Cursor vertical coordinate within the canvas
		 * @param {HTMLElement} canvas The canvas where the renderer draws its output
		 * @returns {Vector2} Cursor position inside the canvas
		 */
		private Vector2 GetCursorPosition(float cursorX, float cursorY)
		{
			this._v2_1.Copy(this.GetCursorNDC(cursorX, cursorY));
			this._v2_1.X *= (this.camera.CameraRight - this.camera.Left) * 0.5f;
			this._v2_1.Y *= (this.camera.Top - this.camera.Bottom) * 0.5f;
			return this._v2_1.Clone();
		}

		/**
		 * Calculate the cursor position in NDC
		 * @param {number} x Cursor horizontal coordinate within the canvas
		 * @param {number} y Cursor vertical coordinate within the canvas
		 * @param {HTMLElement} canvas The canvas where the renderer draws its output
		 * @returns {Vector2} Cursor normalized position inside the canvas
		 */
		private Vector2 GetCursorNDC(float cursorX, float cursorY)
		{
			var canvasRect = glControl.ClientRectangle;
			this._v2_1.SetX(((cursorX - canvasRect.Left) / canvasRect.Width) * 2 - 1);
			this._v2_1.SetY(((canvasRect.Bottom - cursorY) / canvasRect.Height) * 2 - 1);
			return this._v2_1.Clone();

		}

		/**
		* Apply a transformation matrix, to the camera and gizmos
		* @param {Object} transformation Object containing matrices to apply to camera and gizmos
		*/
		private void ApplyTransformMatrix(Transformation transformation)
		{
			if (transformation.Camera != null)
			{

				this._m4_1.Copy(this._cameraMatrixState).PreMultiply(transformation.Camera);
				this._m4_1.Decompose(this.camera.Position, this.camera.Quaternion, this.camera.Scale);
				this.camera.UpdateMatrix();

				//update camera up vector
				if (this._state == STATE.ROTATE || this._state == STATE.ZROTATE || this._state == STATE.ANIMATION_ROTATE)
				{

					this.camera.Up.Copy(this._upState).ApplyQuaternion(this.camera.Quaternion);

				}

			}

			if (transformation.gizmos != null)
			{

				this._m4_1.Copy(this._gizmoMatrixState).PreMultiply(transformation.gizmos);
				this._m4_1.Decompose(this._gizmos.Position, this._gizmos.Quaternion, this._gizmos.Scale);
				this._gizmos.UpdateMatrix();

			}

			if (this._state == STATE.SCALE || this._state == STATE.FOCUS || this._state == STATE.ANIMATION_FOCUS)
			{

				this._tbRadius = this.CalculateTbRadius(this.camera);

				if (this.AdjustNearFar)
				{

					var cameraDistance = this.camera.Position.DistanceTo(this._gizmos.Position);

					var bb = new Box3();
					bb.SetFromObject(this._gizmos);
					var sphere = new Sphere();
					bb.GetBoundingSphere(sphere);

					var adjustedNearPosition = (float)System.Math.Max(this._nearPos0, sphere.Radius + sphere.Center.Length());
					var regularNearPosition = cameraDistance - this._initialNear;

					var minNearPos = (float)System.Math.Min(adjustedNearPosition, regularNearPosition);
					this.camera.Near = cameraDistance - minNearPos;


					var adjustedFarPosition = (float)System.Math.Min(this._farPos0, -sphere.Radius + sphere.Center.Length());
					var regularFarPosition = cameraDistance - this._initialFar;

					var minFarPos = (float)System.Math.Min(adjustedFarPosition, regularFarPosition);
					this.camera.Far = cameraDistance - minFarPos;

					this.camera.UpdateProjectionMatrix();

				}
				else
				{

					bool update = false;

					if (this.camera.Near != this._initialNear)
					{

						this.camera.Near = this._initialNear;
						update = true;

					}

					if (this.camera.Far != this._initialFar)
					{

						this.camera.Far = this._initialFar;
						update = true;

					}

					if (update)
					{

						this.camera.UpdateProjectionMatrix();

					}

				}

			}
		}

		private float CalculateTbRadius(Camera camera)
		{
			var distance = camera.Position.DistanceTo(this._gizmos.Position);

			if (camera is PerspectiveCamera)
			{

				var halfFovV = MathUtils.DEG2RAD * camera.Fov * 0.5f; //vertical fov/2 in radians
				var halfFovH = System.Math.Atan((camera.Aspect) * System.Math.Tan(halfFovV)); //horizontal fov/2 in radians
				return (float)System.Math.Tan(System.Math.Min(halfFovV, halfFovH)) * distance * this.radiusFactor;

			}
			else if (camera is OrthographicCamera)
			{

				return (float)System.Math.Min(camera.Top, camera.CameraRight) * this.radiusFactor;

			}
			else
				return 0.0f;
		}

		public void Update()
		{
			var EPS = 0.000001f;

			if (this.target.Equals(this._currentTarget) == false)
			{

				this._gizmos.Position.Copy(this.target);    //for correct radius calculation
				this._tbRadius = this.CalculateTbRadius(this.camera);
				this.MakeGizmos(this.target, this._tbRadius);
				this._currentTarget.Copy(this.target);

			}

			//check min/max parameters
			if (this.camera is OrthographicCamera)
			{

				//check zoom
				if (this.camera.Zoom > this.MaxZoom || this.camera.Zoom < this.MinZoom)
				{

					var newZoom = MathUtils.Clamp(this.camera.Zoom, this.MinZoom, this.MaxZoom);
					this.ApplyTransformMatrix(this.Scale(newZoom / this.camera.Zoom, this._gizmos.Position, true));

				}

			}
			else if (this.camera is PerspectiveCamera)
			{

				//check distance
				var distance = this.camera.Position.DistanceTo(this._gizmos.Position);

				if (distance > this.MaxDistance + EPS || distance < this.MinDistance - EPS)
				{

					var newDistance = MathUtils.Clamp(distance, this.MinDistance, this.MaxDistance);
					this.ApplyTransformMatrix(this.Scale(newDistance / distance, this._gizmos.Position));
					this.UpdateMatrixState();

				}

				//check fov
				if (this.camera.Fov < this.minFov || this.camera.Fov > this.maxFov)
				{

					this.camera.Fov = MathUtils.Clamp(this.camera.Fov, this.minFov, this.maxFov);
					this.camera.UpdateProjectionMatrix();

				}

				var oldRadius = this._tbRadius;
				this._tbRadius = this.CalculateTbRadius(this.camera);

				if (oldRadius < this._tbRadius - EPS || oldRadius > this._tbRadius + EPS)
				{

					var scale = (this._gizmos.Scale.X + this._gizmos.Scale.Y + this._gizmos.Scale.Z) / 3;
					var newRadius = this._tbRadius / scale;
					var curve = new EllipseCurve(0, 0, newRadius, newRadius);
					var points = curve.GetPoints(this._curvePts);
					var curveGeometry = new BufferGeometry().SetFromPoints(points);

					foreach (var gizmo in this._gizmos.Children)
					{
						gizmo.Geometry = curveGeometry;
						//this._gizmos.children[gizmo].geometry = curveGeometry;

					}

				}

			}

			this.camera.LookAt(this._gizmos.Position);
		}

		private void OnSinglePanStart(MouseEventArgs e, string operation)
		{
			if (this.Enabled == false) return;

			if (StartEvent != null) StartEvent();

			SetCenter(e.X, e.Y);

			switch (operation)
			{
				case "PAN":
					if (!EnablePan)
					{
						return;
					}
					if (this._animationId != -1)
					{
						_animationId = -1;
						_timeStart = -1;

						ActivateGizmos(false);

						if (ChangeEvent != null) ChangeEvent();

					}

					UpdateTbState(STATE.PAN, true);
					this._startCursorPosition.Copy(this.UnprojectOnTbPlane(this.camera, _center.x, _center.y));
					if (this.EnableGrid)
					{

						this.DrawGrid();
						if (ChangeEvent != null) ChangeEvent();

					}
					break;
				case "ROTATE":
					if (!this.EnableRotate)
					{

						return;

					}

					if (this._animationId != -1)
					{

						this._animationId = -1;
						this._timeStart = -1;

					}

					this.UpdateTbState(STATE.ROTATE, true);
					this._startCursorPosition.Copy(this.UnprojectOnTbSurface(this.camera, _center.x, _center.y, this._tbRadius));
					this.ActivateGizmos(true);
					if (this.enableAnimations)
					{

						this._timePrev = this._timeCurrent = stopWatch.ElapsedMilliseconds;
						this._angleCurrent = this._anglePrev = 0;
						this._cursorPosPrev.Copy(this._startCursorPosition);
						this._cursorPosCurr.Copy(this._cursorPosPrev);
						this._wCurr = 0;
						this._wPrev = this._wCurr;

					}

					if (ChangeEvent != null) ChangeEvent();

					break;
				case "FOV":
					if (this.camera is OrthographicCamera || !this.EnableZoom)
					{

						return;

					}

					if (this._animationId != -1)
					{

						//cancelAnimationFrame(this._animationId);
						this._animationId = -1;
						this._timeStart = -1;

						this.ActivateGizmos(false);
						if (ChangeEvent != null) ChangeEvent();

					}

					this.UpdateTbState(STATE.FOV, true);
					this._startCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);
					this._currentCursorPosition.Copy(this._startCursorPosition);
					break;
				case "ZOOM":
					if (!this.EnableZoom)
					{

						return;

					}

					if (this._animationId != -1)
					{

						//cancelAnimationFrame(this._animationId);
						this._animationId = -1;
						this._timeStart = -1;

						this.ActivateGizmos(false);
						if (ChangeEvent != null) ChangeEvent();

					}

					this.UpdateTbState(STATE.SCALE, true);
					this._startCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);
					this._currentCursorPosition.Copy(this._startCursorPosition);
					break;
			}
		}

		private void OnSinglePanMove(MouseEventArgs e, STATE opState)
		{
			if (this.Enabled)
			{

				var restart = opState != this._state;
				this.SetCenter(e.X, e.Y);

				switch (opState)
				{

					case STATE.PAN:

						if (this.EnablePan)
						{

							if (restart)
							{

								//switch to pan operation
								if (EndEvent != null) EndEvent();
								if (StartEvent != null) StartEvent();

								this.UpdateTbState(opState, true);
								this._startCursorPosition.Copy(this.UnprojectOnTbPlane(this.camera, _center.x, _center.y));
								if (this.EnableGrid)
								{

									this.DrawGrid();

								}

								this.ActivateGizmos(false);

							}
							else
							{

								//continue with pan operation
								this._currentCursorPosition.Copy(this.UnprojectOnTbPlane(this.camera, _center.x, _center.y));
								this.ApplyTransformMatrix(this.Pan(this._startCursorPosition, this._currentCursorPosition));

							}

						}

						break;

					case STATE.ROTATE:

						if (this.EnableRotate)
						{

							if (restart)
							{

								//switch to rotate operation

								if (EndEvent != null) EndEvent();
								if (StartEvent != null) StartEvent();

								this.UpdateTbState(opState, true);
								this._startCursorPosition.Copy(this.UnprojectOnTbSurface(this.camera, _center.x, _center.y, this._tbRadius));

								if (this.EnableGrid)
								{

									this.DisposeGrid();

								}

								this.ActivateGizmos(true);

							}
							else
							{

								//continue with rotate operation
								this._currentCursorPosition.Copy(this.UnprojectOnTbSurface(this.camera, _center.x, _center.y, this._tbRadius));

								var distance = this._startCursorPosition.DistanceTo(this._currentCursorPosition);
								var angle = this._startCursorPosition.AngleTo(this._currentCursorPosition);
								var amount = (float)System.Math.Max(distance / this._tbRadius, angle); //effective rotation angle

								this.ApplyTransformMatrix(this.Rotate(this.CalculateRotationAxis(this._startCursorPosition, this._currentCursorPosition), amount));

								if (this.enableAnimations)
								{

									this._timePrev = this._timeCurrent;
									this._timeCurrent = stopWatch.ElapsedMilliseconds;
									this._anglePrev = this._angleCurrent;
									this._angleCurrent = amount;
									this._cursorPosPrev.Copy(this._cursorPosCurr);
									this._cursorPosCurr.Copy(this._currentCursorPosition);
									this._wPrev = this._wCurr;
									this._wCurr = this.CalculateAngularSpeed(this._anglePrev, this._angleCurrent, this._timePrev, this._timeCurrent);

								}

							}

						}

						break;

					case STATE.SCALE:

						if (this.EnableZoom)
						{

							if (restart)
							{

								//switch to zoom operation

								if (EndEvent != null) EndEvent();
								if (StartEvent != null) StartEvent();

								this.UpdateTbState(opState, true);
								this._startCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);
								this._currentCursorPosition.Copy(this._startCursorPosition);

								if (this.EnableGrid)
								{

									this.DisposeGrid();

								}

								this.ActivateGizmos(false);

							}
							else
							{

								//continue with zoom operation
								var screenNotches = 8;    //how many wheel notches corresponds to a full screen pan
								this._currentCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);

								var movement = this._currentCursorPosition.Y - this._startCursorPosition.Y;

								float size = 1;

								if (movement < 0)
								{

									size = 1 / ((float)System.Math.Pow(this.ScaleFactor, -movement * screenNotches));

								}
								else if (movement > 0)
								{

									size = (float)System.Math.Pow(this.ScaleFactor, movement * screenNotches);

								}

								this._v3_1.SetFromMatrixPosition(this._gizmoMatrixState);

								this.ApplyTransformMatrix(this.Scale(size, this._v3_1));

							}

						}

						break;

					case STATE.FOV:

						if (this.EnableZoom && this.camera is PerspectiveCamera)
						{

							if (restart)
							{

								//switch to fov operation

								if (EndEvent != null) EndEvent();
								if (StartEvent != null) StartEvent();

								this.UpdateTbState(opState, true);
								this._startCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);
								this._currentCursorPosition.Copy(this._startCursorPosition);

								if (this.EnableGrid)
								{

									this.DisposeGrid();

								}

								this.ActivateGizmos(false);

							}
							else
							{

								//continue with fov operation
								var screenNotches = 8;    //how many wheel notches corresponds to a full screen pan
								this._currentCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);

								var movement = this._currentCursorPosition.Y - this._startCursorPosition.Y;

								float size = 1;

								if (movement < 0)
								{

									size = 1 / ((float)System.Math.Pow(this.ScaleFactor, -movement * screenNotches));

								}
								else if (movement > 0)
								{

									size = (float)System.Math.Pow(this.ScaleFactor, movement * screenNotches);

								}

								this._v3_1.SetFromMatrixPosition(this._cameraMatrixState);
								var x = this._v3_1.DistanceTo(this._gizmos.Position);
								var xNew = x / size; //distance between camera and gizmos if scale(size, scalepoint) would be performed

								//check min and max distance
								xNew = MathUtils.Clamp(xNew, this.MinDistance, this.MaxDistance);

								var y = x * (float)System.Math.Tan(MathUtils.DEG2RAD * this._fovState * 0.5f);

								//calculate new fov
								var newFov = MathUtils.RAD2DEG * ((float)System.Math.Atan(y / xNew) * 2);

								//check min and max fov
								newFov = MathUtils.Clamp(newFov, this.minFov, this.maxFov);

								var newDistance = y / (float)System.Math.Tan(MathUtils.DEG2RAD * (newFov / 2));
								size = x / newDistance;
								this._v3_2.SetFromMatrixPosition(this._gizmoMatrixState);

								this.SetFov(newFov);
								this.ApplyTransformMatrix(this.Scale(size, this._v3_2, false));

								//adjusting distance
								_offset.Copy(this._gizmos.Position).Sub(this.camera.Position).Normalize().MultiplyScalar(newDistance / x);
								this._m4_1.MakeTranslation(_offset.X, _offset.Y, _offset.Z);

							}

						}

						break;

				}

				if (ChangeEvent != null) ChangeEvent();

			}

		}

		private float CalculateAngularSpeed(float p0, float p1, long t0, long t1)
		{
			var s = p1 - p0;
			var t = (t1 - t0) / 1000;
			if (t == 0)
			{

				return 0;

			}

			return (float)(s / t);
		}

		/**
		 * Calculate the rotation axis as the vector perpendicular between two vectors
		 * @param {Vector3} vec1 The first vector
		 * @param {Vector3} vec2 The second vector
		 * @returns {Vector3} The normalized rotation axis
		 */
		private Vector3 CalculateRotationAxis(Vector3 vec1, Vector3 vec2)
		{
			this._rotationMatrix.ExtractRotation(this._cameraMatrixState);
			this._quat.SetFromRotationMatrix(this._rotationMatrix);

			this._rotationAxis.CrossVectors(vec1, vec2).ApplyQuaternion(this._quat);
			return this._rotationAxis.Normalize().Clone();
		}

		/**
		* Perform pan operation moving camera between two points
		* @param {Vector3} p0 Initial point
		* @param {Vector3} p1 Ending point
		* @param {Boolean} adjust If movement should be adjusted considering camera distance (Perspective only)
		*/
		private Transformation Pan(Vector3 p0, Vector3 p1, bool adjust = false)
		{
			var movement = p0.Clone().Sub(p1);

			if (this.camera is OrthographicCamera)
			{

				//adjust movement amount
				movement.MultiplyScalar(1 / this.camera.Zoom);

			}
			else if (this.camera is PerspectiveCamera && adjust)
			{

				//adjust movement amount
				this._v3_1.SetFromMatrixPosition(this._cameraMatrixState0); //camera's initial position
				this._v3_2.SetFromMatrixPosition(this._gizmoMatrixState0);  //gizmo's initial position
				var distanceFactor = this._v3_1.DistanceTo(this._v3_2) / this.camera.Position.DistanceTo(this._gizmos.Position);
				movement.MultiplyScalar(1 / distanceFactor);

			}

			this._v3_1.Set(movement.X, movement.Y, 0).ApplyQuaternion(this.camera.Quaternion);

			this._m4_1.MakeTranslation(this._v3_1.X, this._v3_1.Y, this._v3_1.Z);

			this.SetTransformationMatrices(this._m4_1, this._m4_1);
			return _transformation;
		}
		
		public void Reset()
        {
			this.camera.Zoom = this._zoom0;

			if (this.camera is PerspectiveCamera)
			{

				this.camera.Fov = this._fov0;

			}

			this.camera.Near = this._nearPos;
			this.camera.Far = this._farPos;
			this._cameraMatrixState.Copy(this._cameraMatrixState0);
			this._cameraMatrixState.Decompose(this.camera.Position, this.camera.Quaternion, this.camera.Scale);
			this.camera.Up.Copy(this._up0);

			this.camera.UpdateMatrix();
			this.camera.UpdateProjectionMatrix();

			this._gizmoMatrixState.Copy(this._gizmoMatrixState0);
			this._gizmoMatrixState0.Decompose(this._gizmos.Position, this._gizmos.Quaternion, this._gizmos.Scale);
			this._gizmos.UpdateMatrix();

			this._tbRadius = this.CalculateTbRadius(this.camera);
		
			this.MakeGizmos(this._gizmos.Position, this._tbRadius);

			this.camera.LookAt(this._gizmos.Position);

			this.UpdateTbState(STATE.IDLE, false);

			if(ChangeEvent!=null)
				ChangeEvent();
			
		}
		private void OnPointerMove(object sender, MouseEventArgs e)
		{
			if (this._input != INPUT.CURSOR)
			{

				switch (this._input)
				{
					case INPUT.ONE_FINGER:
						//singleMove
						this.UpdateTouchEvent(e);

						this.OnSinglePanMove(e, STATE.ROTATE);
						break;

					case INPUT.ONE_FINGER_SWITCHED:

						var movement = this.CalculatePointersDistance(this._touchCurrent[0], e) * this._devPxRatio;

						if (movement >= this._switchSensibility)
						{

							//singleMove
							this._input = INPUT.ONE_FINGER;
							this.UpdateTouchEvent(e);

							this.OnSinglePanStart(e, "ROTATE");

						}

						break;
					case INPUT.TWO_FINGER:

						//rotate/pan/pinchMove
						this.UpdateTouchEvent(e);

						this.OnRotateMove();
						this.OnPinchMove();
						this.OnDoublePanMove();

						break;
					case INPUT.MULT_FINGER:

						//multMove
						this.UpdateTouchEvent(e);
						this.onTriplePanMove(e);
						break;

				}
			}
			else if (this._input == INPUT.CURSOR)
			{

				string modifier = null;

				if (this.controlKey)
				{

					modifier = "CTRL";

				}
				else if (this.shiftKey)
				{

					modifier = "SHIFT";

				}

				var mouseOpState = this.GetOpStateFromAction(this._button, modifier);

				if (mouseOpState != null)
				{

					this.OnSinglePanMove(e, (STATE)mouseOpState);

				}

			}

			//checkDistance
			if (this._downValid)
			{

				float movement = 0;
				if(this._downEvents.Count>1)
					movement = this.CalculatePointersDistance(this._downEvents[this._downEvents.Count - 1], e) * this._devPxRatio;

				if (movement > this._movementThreshold)
				{

					this._downValid = false;

				}

			}


		}

		private void OnRotateMove()
		{
			if (this.Enabled && this.EnableRotate)
			{

				this.SetCenter((this._touchCurrent[0].X + this._touchCurrent[1].X) / 2, (this._touchCurrent[0].Y + this._touchCurrent[1].Y) / 2);

				Vector3 rotationPoint = new Vector3(); ;

				if (this._state != STATE.ZROTATE)
				{

					this.UpdateTbState(STATE.ZROTATE, true);
					this._startFingerRotation = this._currentFingerRotation;

				}

				//this._currentFingerRotation = event.rotation;
				this._currentFingerRotation = this.GetAngle(this._touchCurrent[1], this._touchCurrent[0]) + this.GetAngle(this._touchStart[1], this._touchStart[0]);

				if (!this.EnablePan)
				{

					rotationPoint = new Vector3().SetFromMatrixPosition(this._gizmoMatrixState);

				}
				else
				{

					this._v3_2.SetFromMatrixPosition(this._gizmoMatrixState);
					rotationPoint = this.UnprojectOnTbPlane(this.camera, _center.x, _center.y).ApplyQuaternion(this.camera.Quaternion).MultiplyScalar(1 / this.camera.Zoom).Add(this._v3_2);

				}

				var amount = MathUtils.DEG2RAD * (this._startFingerRotation - this._currentFingerRotation);

				this.ApplyTransformMatrix(this.ZRotate(rotationPoint, amount));
				if (ChangeEvent != null) ChangeEvent();

			}
		}
		/**
		 * Calculate the angle between two pointers
		 * @param {PointerEvent} p1
		 * @param {PointerEvent} p2
		 * @returns {Number} The angle between two pointers in degrees
		 */
		private float GetAngle(MouseEventArgs p1, MouseEventArgs p2)
		{
			return (float)(System.Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180 / System.Math.PI);
		}

		private void OnPinchMove()
		{
			if (this.Enabled && this.EnableZoom)
			{

				this.SetCenter((this._touchCurrent[0].X + this._touchCurrent[1].X) / 2, (this._touchCurrent[0].Y + this._touchCurrent[1].Y) / 2);
				var minDistance = 12; //minimum distance between fingers (in css pixels)

				if (this._state != STATE.SCALE)
				{

					this._startFingerDistance = this._currentFingerDistance;
					this.UpdateTbState(STATE.SCALE, true);

				}

				this._currentFingerDistance = (float)System.Math.Max(this.CalculatePointersDistance(this._touchCurrent[0], this._touchCurrent[1]), minDistance * this._devPxRatio);
				var amount = this._currentFingerDistance / this._startFingerDistance;

				Vector3 scalePoint = new Vector3(); ;

				if (!this.EnablePan)
				{

					scalePoint = this._gizmos.Position;

				}
				else
				{

					if (this.camera is OrthographicCamera)
					{

						scalePoint = this.UnprojectOnTbPlane(this.camera, _center.x, _center.y)
							.ApplyQuaternion(this.camera.Quaternion)
							.MultiplyScalar(1 / this.camera.Zoom)
							.Add(this._gizmos.Position);

					}
					else if (this.camera is PerspectiveCamera)
					{

						scalePoint = this.UnprojectOnTbPlane(this.camera, _center.x, _center.y)
							.ApplyQuaternion(this.camera.Quaternion)
							.Add(this._gizmos.Position);

					}

				}

				this.ApplyTransformMatrix(this.Scale(amount, scalePoint));

				if (ChangeEvent != null) ChangeEvent();

			}
		}

		private void OnDoublePanMove()
		{
			if (this.Enabled && this.EnablePan)
			{

				this.SetCenter((this._touchCurrent[0].X + this._touchCurrent[1].X) / 2, (this._touchCurrent[0].Y + this._touchCurrent[1].Y) / 2);

				if (this._state != STATE.PAN)
				{

					this.UpdateTbState(STATE.PAN, true);
					this._startCursorPosition.Copy(this._currentCursorPosition);

				}

				this._currentCursorPosition.Copy(this.UnprojectOnTbPlane(this.camera, _center.x, _center.y, true));
				this.ApplyTransformMatrix(this.Pan(this._startCursorPosition, this._currentCursorPosition, true));
				if (ChangeEvent != null) ChangeEvent();

			}
		}

		private void onTriplePanMove(MouseEventArgs e)
		{
			if (this.Enabled && this.EnableZoom)
			{

				//	  fov / 2
				//		|\
				//		| \
				//		|  \
				//	x	|	\
				//		| 	 \
				//		| 	  \
				//		| _ _ _\
				//			y

				//const center = event.center;
				var clientX = 0;
				var clientY = 0;
				var nFingers = this._touchCurrent.Count;

				for (int i = 0; i < nFingers; i++)
				{

					clientX += this._touchCurrent[i].X;
					clientY += this._touchCurrent[i].Y;

				}

				this.SetCenter(clientX / nFingers, clientY / nFingers);

				var screenNotches = 8;    //how many wheel notches corresponds to a full screen pan
				this._currentCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);

				var movement = this._currentCursorPosition.Y - this._startCursorPosition.Y;

				float size = 1;

				if (movement < 0)
				{

					size = 1 / ((float)System.Math.Pow(this.ScaleFactor, -movement * screenNotches));

				}
				else if (movement > 0)
				{

					size = (float)System.Math.Pow(this.ScaleFactor, movement * screenNotches);

				}

				this._v3_1.SetFromMatrixPosition(this._cameraMatrixState);
				var x = this._v3_1.DistanceTo(this._gizmos.Position);
				var xNew = x / size; //distance between camera and gizmos if scale(size, scalepoint) would be performed

				//check min and max distance
				xNew = MathUtils.Clamp(xNew, this.MinDistance, this.MaxDistance);

				var y = x * (float)System.Math.Tan(MathUtils.DEG2RAD * this._fovState * 0.5f);

				//calculate new fov
				var newFov = MathUtils.RAD2DEG * ((float)System.Math.Atan(y / xNew) * 2);

				//check min and max fov
				newFov = MathUtils.Clamp(newFov, this.minFov, this.maxFov);

				var newDistance = y / (float)System.Math.Tan(MathUtils.DEG2RAD * (newFov / 2));
				size = x / newDistance;
				this._v3_2.SetFromMatrixPosition(this._gizmoMatrixState);

				this.SetFov(newFov);
				this.ApplyTransformMatrix(this.Scale(size, this._v3_2, false));

				//adjusting distance
				_offset.Copy(this._gizmos.Position).Sub(this.camera.Position).Normalize().MultiplyScalar(newDistance / x);
				this._m4_1.MakeTranslation(_offset.X, _offset.Y, _offset.Z);

				if (ChangeEvent != null) ChangeEvent();

			}
		}

		private object GetOpStateFromAction(string mouse, string key)
		{
			MouseAction action;

			for (var i = 0; i < this.mouseActions.Count; i++)
			{

				action = this.mouseActions[i];
				if (action.mouse == mouse && action.key == key)
				{

					return action.state;

				}

			}

			if (key != null)
			{

				for (int i = 0; i < this.mouseActions.Count; i++)
				{

					action = this.mouseActions[i];
					if (action.mouse == mouse && action.key == null)
					{

						return action.state;

					}

				}

			}

			return null;
		}

		private void OnSinglePanEnd()
		{

		}
		private void OnPointerUp(object sender, MouseEventArgs e)
		{
			if (this._input != INPUT.CURSOR)
			{

				var nTouch = this._touchCurrent.Count;

				for (var i = 0; i < nTouch; i++)
				{

					if (this._touchCurrent[i].Equals(e))
					{

						this._touchCurrent.RemoveAt(i);//splice(i, 1);
						this._touchStart.RemoveAt(i);// splice(i, 1);
						break;

					}

				}

				switch (this._input)
				{

					case INPUT.ONE_FINGER:
					case INPUT.ONE_FINGER_SWITCHED:
						//singleEnd
						glControl.MouseMove -= OnPointerMove;
						glControl.MouseUp -= OnPointerUp;
						this._input = INPUT.NONE;
						this.OnSinglePanEnd();
						break;

					case INPUT.TWO_FINGER:

						//doubleEnd
						this.OnDoublePanEnd(e);
						this.OnPinchEnd(e);
						this.OnRotateEnd(e);

						//switching to singleStart
						this._input = INPUT.ONE_FINGER_SWITCHED;

						break;

					case INPUT.MULT_FINGER:

						if (this._touchCurrent.Count == 0)
						{

							glControl.MouseMove -= OnPointerMove;
							glControl.MouseUp -= OnPointerUp;

							//multCancel
							this._input = INPUT.NONE;
							this.OnTriplePanEnd();

						}

						break;

				}

			}
			else if (this._input == INPUT.CURSOR)
			{

				glControl.MouseMove -= OnPointerMove;
				glControl.MouseUp -= OnPointerUp;


				this._input = INPUT.NONE;
				this.OnSinglePanEnd();
				this._button = "NONE";

			}



			if (this._downValid)
			{
				float downTime = 0;
				if(this._downEventsTime.Count>0)
					downTime = stopWatch.ElapsedMilliseconds - this._downEventsTime[this._downEventsTime.Count - 1];

				if (downTime <= this._maxDownTime)
				{

					if (this._nclicks == 0)
					{

						//first valid click detected
						this._nclicks = 1;
						this._clickStart = stopWatch.ElapsedMilliseconds;

					}
					else
					{

						var clickInterval = stopWatch.ElapsedMilliseconds - this._clickStart;
						float movement = 0;
						if(this._downEvents.Count>2)
							movement = this.CalculatePointersDistance(this._downEvents[1], this._downEvents[0]) * this._devPxRatio;

						if (clickInterval <= this._maxInterval && movement <= this._posThreshold)
						{

							//second valid click detected
							//fire double tap and reset values
							this._nclicks = 0;
							this._downEvents.Clear();// splice(0, this._downEvents.length);
							//this.OnDoubleTap(e);

						}
						else
						{

							//new 'first click'
							this._nclicks = 1;
							this._downEvents.RemoveAt(0);
							this._clickStart = stopWatch.ElapsedMilliseconds;

						}

					}

				}
				else
				{

					this._downValid = false;
					this._nclicks = 0;
					this._downEvents.Clear();// splice(0, this._downEvents.length);

				}

			}
			else
			{

				this._nclicks = 0;
				this._downEvents.Clear();// splice(0, this._downEvents.length);

			}



		}

		private void OnDoubleTap(MouseEventArgs e)
		{
			if (this.Enabled && this.EnablePan && this.scene != null)
			{
				if (StartEvent != null) StartEvent();

				this.SetCenter(e.X, e.Y);
				var hitP = this.UnprojectOnObj(this.GetCursorNDC(_center.x, _center.y), this.camera);

				if (hitP != null && this.enableAnimations)
				{

					//const self = this;
					//if ( this._animationId != - 1 ) {

					//window.cancelAnimationFrame(this._animationId);

					//}

					//this._timeStart = - 1;
					//this._animationId = window.requestAnimationFrame(function (t ) {

					//	self.updateTbState(STATE.ANIMATION_FOCUS, true );
					//	self.onFocusAnim(t, hitP, self._cameraMatrixState, self._gizmoMatrixState);

					//} );

				}
				else if (hitP != null && !this.enableAnimations)
				{

					this.UpdateTbState(STATE.FOCUS, true);
					this.Focus(hitP, this.ScaleFactor);
					this.UpdateTbState(STATE.IDLE, false);
					if (ChangeEvent != null) ChangeEvent();

				}

			}
			if (EndEvent != null) EndEvent();
		}

		private void OnTriplePanEnd()
		{
			this.UpdateTbState(STATE.IDLE, false);
			if (EndEvent != null) EndEvent();
		}

		private void OnRotateEnd(MouseEventArgs e)
		{

			this.UpdateTbState(STATE.IDLE, false);
			this.ActivateGizmos(false);
			if (EndEvent != null) EndEvent();
		}

		private void OnPinchEnd(MouseEventArgs e)
		{
			this.UpdateTbState(STATE.IDLE, false);
			if (EndEvent != null) EndEvent();
		}

		private void OnDoublePanEnd(MouseEventArgs e)
		{
			this.UpdateTbState(STATE.IDLE, false);
			if (EndEvent != null) EndEvent();
		}

		private void OnPointerDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{

				this._downValid = true;

				this._downEvents.Add(e);

				this._downStart = stopWatch.ElapsedMilliseconds;

			}
			else
			{

				this._downValid = false;

			}
			if (this._input != INPUT.CURSOR && e.Button!=MouseButtons.Right&&e.Button!=MouseButtons.Middle)
			{
				this._touchStart.Add(e);
				this._touchCurrent.Add(e);

				switch (this._input)
				{
					case INPUT.NONE:
						// single start
						this._input = INPUT.ONE_FINGER;
						this.OnSinglePanStart(e, "ROTATE");
						glControl.MouseMove += OnPointerMove;
						glControl.MouseUp += OnPointerUp;
						break;
					case INPUT.ONE_FINGER:
					case INPUT.ONE_FINGER_SWITCHED:
						//doubleStart
						this._input = INPUT.TWO_FINGER;
						this.OnRotateStart();
						this.OnPinchStart();
						this.OnDoublePanStart();
						break;

					case INPUT.TWO_FINGER:
						//multipleStart
						this._input = INPUT.MULT_FINGER;
						this.OnTriplePanStart();
						break;
				}
			}
			else if (this._input == INPUT.NONE)
			{
				string modifier = null;

				if (this.controlKey)
				{

					modifier = "CTRL";

				}
				else if (this.shiftKey)
				{

					modifier = "SHIFT";

				}

				this._mouseOp = this.GetOpFromAction(GetMouseButtonToString(e), modifier);
				if (this._mouseOp != null)
				{
					glControl.MouseMove += OnPointerMove;
					glControl.MouseUp += OnPointerUp;

					//singleStart
					this._input = INPUT.CURSOR;
					this._button = GetMouseButtonToString(e);
					this.OnSinglePanStart(e, this._mouseOp);

				}
			}
		}

		private void OnRotateStart()
		{
			if (this.Enabled && this.EnableRotate)
			{
				if (StartEvent != null) StartEvent();

				this.UpdateTbState(STATE.ZROTATE, true);

				//this._startFingerRotation = event.rotation;

				this._startFingerRotation = this.GetAngle(this._touchCurrent[1], this._touchCurrent[0]) + this.GetAngle(this._touchStart[1], this._touchStart[0]);
				this._currentFingerRotation = this._startFingerRotation;

				this.camera.GetWorldDirection(this._rotationAxis); //rotation axis

				if (!this.EnablePan && !this.EnableZoom)
				{

					this.ActivateGizmos(true);

				}

			}
		}

		private void OnPinchStart()
		{
			if (this.Enabled && this.EnableZoom)
			{
				if (StartEvent != null) StartEvent();
				this.UpdateTbState(STATE.SCALE, true);

				this._startFingerDistance = this.CalculatePointersDistance(this._touchCurrent[0], this._touchCurrent[1]);
				this._currentFingerDistance = this._startFingerDistance;

				this.ActivateGizmos(false);

			}
		}

		private void OnDoublePanStart()
		{
			if (this.Enabled && this.EnablePan)
			{
				if (StartEvent != null) StartEvent();

				this.UpdateTbState(STATE.PAN, true);

				this.SetCenter((this._touchCurrent[0].X + this._touchCurrent[1].X) / 2, (this._touchCurrent[0].Y + this._touchCurrent[1].Y) / 2);
				this._startCursorPosition.Copy(this.UnprojectOnTbPlane(this.camera, _center.x, _center.y, true));
				this._currentCursorPosition.Copy(this._startCursorPosition);

				this.ActivateGizmos(false);

			}
		}

		private void OnTriplePanStart()
		{
			if (this.Enabled && this.EnableZoom)
			{
				if (StartEvent != null) StartEvent();

				this.UpdateTbState(STATE.SCALE, true);

				//const center = event.center;
				var clientX = 0;
				var clientY = 0;
				var nFingers = this._touchCurrent.Count;

				for (int i = 0; i < nFingers; i++)
				{

					clientX += this._touchCurrent[i].X;
					clientY += this._touchCurrent[i].Y;

				}

				this.SetCenter(clientX / nFingers, clientY / nFingers);

				this._startCursorPosition.SetY(this.GetCursorNDC(_center.x, _center.y).Y * 0.5f);
				this._currentCursorPosition.Copy(this._startCursorPosition);

			}
		}

		private string GetMouseButtonToString(MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					return "LEFT";
				case MouseButtons.Middle:
					return "MIDDLE";
				case MouseButtons.Right:
					return "RIGHT";
				default:
					return "NONE";

			}
		}
		private string GetOpFromAction(string mouse, string key)
		{

			MouseAction action;

			for (int i = 0; i < this.mouseActions.Count; i++)
			{

				action = this.mouseActions[i];
				if (action.mouse == mouse && action.key == key)
				{

					return action.operation;

				}

			}

			if (key != null)
			{

				for (int i = 0; i < this.mouseActions.Count; i++)
				{

					action = this.mouseActions[i];
					if (action.mouse == mouse && action.key == null)
					{

						return action.operation;

					}

				}

			}

			return null;
		}

		private void OnWheel(object sender, MouseEventArgs e)
		{
			if (this.Enabled && this.EnableZoom)
			{

				string modifier = null;

				if (controlKey)
				{

					modifier = "CTRL";

				}
				else if (shiftKey)
				{

					modifier = "SHIFT";

				}

				string mouseOp = this.GetOpFromAction("WHEEL", modifier);

				if (mouseOp != null)
				{

					if (StartEvent != null) StartEvent();

					float notchDeltaY = 125; //distance of one notch of mouse wheel
					float sgn = e.Delta / notchDeltaY;

					float size = 1;

					if (sgn > 0)
					{

						size = 1 / this.ScaleFactor;

					}
					else if (sgn < 0)
					{

						size = this.ScaleFactor;

					}

					switch (mouseOp)
					{

						case "ZOOM":

							this.UpdateTbState(STATE.SCALE, true);

							if (sgn > 0)
							{

								size = 1 / ((float)System.Math.Pow(this.ScaleFactor, sgn));

							}
							else if (sgn < 0)
							{

								size = (float)System.Math.Pow(this.ScaleFactor, -sgn);

							}

							if (this.CursorZoom && this.EnablePan)
							{

								Vector3 scalePoint = new Vector3();

								if (this.camera is OrthographicCamera)
								{

									scalePoint = this.UnprojectOnTbPlane(this.camera, e.X, e.Y).ApplyQuaternion(this.camera.Quaternion).MultiplyScalar(1 / this.camera.Zoom).Add(this._gizmos.Position);

								}
								else if (this.camera is PerspectiveCamera)
								{

									scalePoint = this.UnprojectOnTbPlane(this.camera, e.X, e.Y).ApplyQuaternion(this.camera.Quaternion).Add(this._gizmos.Position);

								}

								this.ApplyTransformMatrix(this.Scale(size, scalePoint));

							}
							else
							{

								this.ApplyTransformMatrix(this.Scale(size, this._gizmos.Position));

							}

							if (this._grid != null)
							{

								this.DisposeGrid();
								this.DrawGrid();

							}

							this.UpdateTbState(STATE.IDLE, false);

							if (ChangeEvent != null) ChangeEvent();
							if (EndEvent != null) EndEvent();

							break;

						case "FOV":

							if (this.camera is PerspectiveCamera)
							{

								this.UpdateTbState(STATE.FOV, true);


								//Vertigo effect

								//	  fov / 2
								//		|\
								//		| \
								//		|  \
								//	x	|	\
								//		| 	 \
								//		| 	  \
								//		| _ _ _\
								//			y

								//check for iOs shift shortcut
								if (e.Delta != 0)
								{

									sgn = e.Delta / notchDeltaY;

									size = 1;

									if (sgn > 0)
									{

										size = 1 / ((float)System.Math.Pow(this.ScaleFactor, sgn));

									}
									else if (sgn < 0)
									{

										size = (float)System.Math.Pow(this.ScaleFactor, -sgn);

									}

								}

								this._v3_1.SetFromMatrixPosition(this._cameraMatrixState);
								float x = this._v3_1.DistanceTo(this._gizmos.Position);
								var xNew = x / size;    //distance between camera and gizmos if scale(size, scalepoint) would be performed

								//check min and max distance
								xNew = MathUtils.Clamp(xNew, this.MinDistance, this.MaxDistance);

								var y = x * (float)System.Math.Tan(MathUtils.DEG2RAD * this.camera.Fov * 0.5f);

								//calculate new fov
								var newFov = MathUtils.RAD2DEG * (float)(System.Math.Atan(y / xNew) * 2);

								//check min and max fov
								if (newFov > this.maxFov)
								{

									newFov = this.maxFov;

								}
								else if (newFov < this.minFov)
								{

									newFov = this.minFov;

								}

								var newDistance = y / (float)System.Math.Tan(MathUtils.DEG2RAD * (newFov / 2));
								size = x / newDistance;

								this.SetFov(newFov);
								this.ApplyTransformMatrix(this.Scale(size, this._gizmos.Position, false));

							}

							if (this._grid != null)
							{

								this.DisposeGrid();
								this.DrawGrid();

							}

							this.UpdateTbState(STATE.IDLE, false);

							if (ChangeEvent != null) ChangeEvent();
							if (EndEvent != null) EndEvent();

							break;

					}

				}

			}
		}

	}
}
