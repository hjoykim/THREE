using System.Collections.Generic;

namespace THREE
{
    public class PointLightShadow : LightShadow
    {
        

        private List<Vector3> _cubeDirections = new List<Vector3> 
        { 
            new Vector3(1,0,0),
            new Vector3(-1,0,0),
            new Vector3(0,0,1),
            new Vector3(0,0,-1),
            new Vector3(0,1,0),
            new Vector3(0,-1,0)
        };

        private List<Vector3> _cubeUps = new List<Vector3> 
        { 
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,0,1),
            new Vector3(0,0,-1)
        };

        public PointLightShadow()
            : base(new PerspectiveCamera(90, 1, 0.5f, 500))
        {
              _frameExtents = new Vector2(4, 2);
              this._viewportCount = 6;

              _viewports = new List<Vector4>
                {
                    new Vector4(2,1,1,1),
                    new Vector4(0,1,1,1),
                    new Vector4(3,1,1,1),
                    new Vector4(1,1,1,1),
                    new Vector4(3,0,1,1),
                    new Vector4(1,0,1,1)
                };
        }

        public void UpdateMatrices(Light light,int? _viewportIndex=null)
        {
            int viewportIndex = 0;
            if (_viewportIndex == null) viewportIndex = 0;
            else viewportIndex = (int)_viewportIndex;

		    var camera = this.Camera;
			var shadowMatrix = this.Matrix;
			var lightPositionWorld = this._lightPositionWorld;
            var lookTarget = this._lookTarget;
			var projScreenMatrix = this._projScreenMatrix;

		    lightPositionWorld.SetFromMatrixPosition( light.MatrixWorld );
		    camera.Position.Copy( lightPositionWorld );

		    lookTarget.Copy( camera.Position );
		    lookTarget.Add( this._cubeDirections[ viewportIndex ] );
		    camera.Up.Copy( this._cubeUps[ viewportIndex ] );
		    camera.LookAt( lookTarget );
		    camera.UpdateMatrixWorld();

		    shadowMatrix.MakeTranslation( -lightPositionWorld.X, - lightPositionWorld.Y, -lightPositionWorld.Z);

		    projScreenMatrix.MultiplyMatrices( camera.ProjectionMatrix, camera.MatrixWorldInverse );
		    this._frustum.SetFromProjectionMatrix( projScreenMatrix );
        }
    }
}
