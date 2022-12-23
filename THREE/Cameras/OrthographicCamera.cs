using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE
{
    public class OrthographicCamera : Camera,ICloneable
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public OrthographicCamera(float? left=null, float? right=null, float? top=null, float? bottom=null, float? near = null, float? far = null)
        {
            this.type = "OrthographicCamera";

            this.Zoom = 1;

            this.Left = left != null ? (float)left : -1;
            this.CameraRight = right != null ? (float)right : 1;
            this.Top = top != null ? (float)top : 1;
            this.Bottom = bottom != null ? (float)bottom : -1;

            this.Near = near != null ? (float)near : 0.1f;
            this.Far = far != null ? (float)far : 2000;

            this.UpdateProjectionMatrix();
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        protected OrthographicCamera(OrthographicCamera other)
            : base(other)
        {
            this.Zoom = other.Zoom;

            this.Left = other.Left;
            this.CameraRight = other.CameraRight;
            this.Top = other.Top;
            this.Bottom = other.Bottom;

            this.Near = other.Near;
            this.Far = other.Far;
        }

        #endregion

        #region Public Properties


        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// </summary>
        public override void UpdateProjectionMatrix()
        {
            var dx = (this.CameraRight - this.Left) / (2 * this.Zoom);
            var dy = (this.Top - this.Bottom) / (2 * this.Zoom);
            var cx = (this.CameraRight + this.Left) / 2;
            var cy = (this.Top + this.Bottom) / 2;

            var left = cx - dx;
            var right = cx + dx;
            var top = cy + dy;
            var bottom = cy - dy;

            if (this.View.Enabled ) {

			    //var zoomW = this.Zoom / ( this.View.Width / this.View.FullWidth );
			    //var zoomH = this.Zoom / ( this.View.Height / this.View.FullHeight );

			    var scaleW = ( this.CameraRight - this.Left ) / this.View.FullWidth / this.Zoom;
			    var scaleH = ( this.Top - this.Bottom ) / this.View.FullHeight / this.Zoom;

			    left += scaleW *  this.View.OffsetX ;
			    right = left + scaleW * this.View.Width;
			    top -= scaleH * this.View.OffsetY;
			    bottom = top - scaleH *this.View.Height;

		    }

            this.ProjectionMatrix = Matrix4.Identity().MakeOrthographic(left,right,top,bottom, this.Near, this.Far);
            
            this.ProjectionMatrixInverse.GetInverse(this.ProjectionMatrix);
        }

        public override object Clone()
        {
            return new OrthographicCamera(this);
        }
        #endregion
    }
}
