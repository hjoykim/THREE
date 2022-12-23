using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace THREE
{
    public class DragControls : IDisposable
    {
        Plane _plane = new Plane();
        Raycaster _raycaster = new Raycaster();       
        Vector3 _offset = new Vector3();
        Vector3 _intersection = new Vector3();
        Vector3 _worldPosition = new Vector3();
        Matrix4 _inverseMatrix = new Matrix4();

        Object3D _selected = null;
        Object3D _hovered = null;

        Vector2 mouse = new Vector2();

        List<Intersection> _intersections = new List<Intersection>();
        public List<Object3D> objects;

        Control glControl;
        Camera camera;

        public bool Enabled = true;
        public bool TransformGroup = false;

        public Action<Object3D> HoverOff;
        public Action<Object3D> HoverOn;
        public Action<Object3D> DragStart;
        public Action<Object3D> DragEnd;
        public Action<Object3D> Drag;

        public event EventHandler<EventArgs> Disposed;

        public DragControls(Control glControl,List<Object3D> objects,Camera camera)
        {
            this.glControl = glControl;
            this.objects = objects;
            this.camera = camera;

            Activate();
        }
        
        private void Activate()
        {
            this.glControl.MouseMove += OnPointerMove;
            this.glControl.MouseDown += OnPointerDown;
            this.glControl.MouseUp += OnPointerCancel;
            this.glControl.MouseLeave += OnPointerLeave;
        }

        public List<Object3D> GetObjects()
        {
            return objects;
        }
        private void OnPointerLeave(object sender, EventArgs e)
        {
            if (!this.Enabled) return;

            if (_selected != null)
            {
                if (DragEnd != null)
                {
                    DragEnd(_selected);
                }

                _selected = null;

            }

            Cursor.Current = _hovered != null ? Cursors.Arrow : Cursors.Default;
        }

        private void OnPointerCancel(object sender, MouseEventArgs e)
        {
            if (!this.Enabled) return;

            if (_selected != null)
            {
                if (DragEnd != null)
                {
                    DragEnd(_selected);
                }

                _selected = null;

            }

            Cursor.Current = _hovered != null ? Cursors.Arrow : Cursors.Default;
        }

        private void OnPointerDown(object sender, MouseEventArgs e)
        {          
            mouse.X = e.X * 1.0f / (sender as Control).Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / (sender as Control).Height * 2 + 1.0f;

            _intersections.Clear();
            _raycaster.SetFromCamera(mouse, camera);
            _raycaster.IntersectObjects(objects, true, _intersections);

            if (_intersections.Count > 0)
            {

                _selected = (TransformGroup == true) ? objects[0] : _intersections[0].object3D;

                _plane.SetFromNormalAndCoplanarPoint(camera.GetWorldDirection(_plane.Normal), _worldPosition.SetFromMatrixPosition(_selected.MatrixWorld));

                if (_raycaster.ray.IntersectPlane(_plane, _intersection) != null)
                {

                    _inverseMatrix.Copy(_selected.Parent.MatrixWorld).Invert();
                    _offset.Copy(_intersection).Sub(_worldPosition.SetFromMatrixPosition(_selected.MatrixWorld));

                }

                if (DragStart != null)
                {
                    DragStart(_selected);
                }
            }

        }

        private void OnPointerMove(object sender, MouseEventArgs e)
        {
            if (Enabled == false) return;

            mouse.X = e.X * 1.0f  / (sender as Control).Width * 2 - 1.0f;
            mouse.Y = -e.Y * 1.0f / (sender as Control).Height * 2 + 1.0f;

            _raycaster.SetFromCamera(mouse, camera);

            if (_selected != null)
            {

                if (_raycaster.ray.IntersectPlane(_plane, _intersection) != null)
                {

                    _selected.Position.Copy(_intersection.Sub(_offset).ApplyMatrix4(_inverseMatrix));
                    
                }
                if (Drag != null)
                    Drag(_selected);
                return;
            }           

            _intersections.Clear();
            _raycaster.SetFromCamera(mouse, camera);
            _raycaster.IntersectObjects(objects, true, _intersections);

            if (_intersections.Count > 0)
            {

                var object3d = _intersections[0].object3D;

                _plane.SetFromNormalAndCoplanarPoint(camera.GetWorldDirection(_plane.Normal), _worldPosition.SetFromMatrixPosition(object3d.MatrixWorld));

                if (_hovered != null && !object3d.Equals(_hovered))
                {
                    if (HoverOff != null)
                    {
                        HoverOff(_hovered);
                    }

                    _hovered = null;

                }

                if (!object3d.Equals(_hovered))
                {
                    if (HoverOn != null)
                    {
                        HoverOn(object3d);
                    }                   

                    _hovered = object3d;
                }

            }
            else
            {

                if (_hovered != null)
                {
                    if (HoverOff != null)
                    {
                        HoverOff(_hovered);
                    }                    
                    _hovered = null;
                }
            }
        }   

        private void Deactivate()
        {
            this.glControl.MouseMove -= OnPointerMove;
            this.glControl.MouseDown -= OnPointerDown;
            this.glControl.MouseUp -= OnPointerCancel;
            this.glControl.MouseLeave -= OnPointerLeave;
        }

        #region Dispose      

        public virtual void Dispose()
        {
            Deactivate();
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
    }
}
