using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class CameraHelper : LineSegments
    {
        private List<float> Vertices = new List<float>();
        
        private List<float> Colors = new List<float>();
        
        private Hashtable PointMap = new Hashtable();

        private Camera Camera;

        private Vector3 _vector = Vector3.Zero();

        public CameraHelper(Camera camera) 
        {
            this.Geometry = new BufferGeometry();
            this.Material = new LineBasicMaterial() { Color = Color.Hex(0xffffff), VertexColors = Constants.FaceColors>0?true:false ,ToneMapped=false};

            Color colorFrustum = Color.Hex(0xffaa00);
            Color colorCone = Color.Hex(0xff0000);
            Color colorUp = Color.Hex(0x00aaff);
            Color colorTarget = Color.Hex(0xffffff);
            Color colorCross = Color.Hex(0x333333);

            // near

	        AddLine( "n1", "n2", colorFrustum );
	        AddLine( "n2", "n4", colorFrustum );
	        AddLine( "n4", "n3", colorFrustum );
	        AddLine( "n3", "n1", colorFrustum );

	        // far

	        AddLine( "f1", "f2", colorFrustum );
	        AddLine( "f2", "f4", colorFrustum );
	        AddLine( "f4", "f3", colorFrustum );
	        AddLine( "f3", "f1", colorFrustum );

	        // sides

	        AddLine( "n1", "f1", colorFrustum );
	        AddLine( "n2", "f2", colorFrustum );
	        AddLine( "n3", "f3", colorFrustum );
	        AddLine( "n4", "f4", colorFrustum );

	        // cone

	        AddLine( "p", "n1", colorCone );
	        AddLine( "p", "n2", colorCone );
	        AddLine( "p", "n3", colorCone );
	        AddLine( "p", "n4", colorCone );

	        // up

	        AddLine( "u1", "u2", colorUp );
	        AddLine( "u2", "u3", colorUp );
	        AddLine( "u3", "u1", colorUp );

	        // target

	        AddLine( "c", "t", colorTarget );
	        AddLine( "p", "c", colorCross );

	        // cross

	        AddLine( "cn1", "cn2", colorCross );
	        AddLine( "cn3", "cn4", colorCross );

	        AddLine( "cf1", "cf2", colorCross );
	        AddLine( "cf3", "cf4", colorCross );

            (Geometry as BufferGeometry).SetAttribute("position", new BufferAttribute<float>(Vertices.ToArray(), 3));
            (Geometry as BufferGeometry).SetAttribute("color", new BufferAttribute<float>(Colors.ToArray(), 3));

            this.Camera = camera;

            this.Camera.UpdateProjectionMatrix();

            this.Matrix = camera.MatrixWorld;

            this.MatrixAutoUpdate = false;

            this.Update();
        }

        private void AddLine(string a, string b, Color color)
        {
            AddPoint(a, color);
            AddPoint(b, color);
        }

        private void AddPoint(string id, Color color)
        {
            Vertices.Add<float>(0, 0, 0);
            Colors.Add<float>(color.R, color.G, color.B);

            if (!PointMap.ContainsKey(id))
            {
                PointMap.Add(id, new List<int>());
            }
            (PointMap[id] as List<int>).Add(Vertices.Count / 3 - 1);
        }

        public void Update()
        {
            var geometry = this.Geometry;
	        var pointMap = this.PointMap;

	        int w = 1, h = 1;

            Camera _camera = new Camera();
	        // we need just camera projection matrix inverse
	        // world matrix must be identity

	        _camera.ProjectionMatrixInverse.Copy( this.Camera.ProjectionMatrixInverse );

	        // center / target

	        SetPoint( "c", pointMap, geometry, _camera, 0, 0, - 1 );
	        SetPoint( "t", pointMap, geometry, _camera, 0, 0, 1 );

	        // near

	        SetPoint( "n1", pointMap, geometry, _camera, - w, - h, - 1 );
	        SetPoint( "n2", pointMap, geometry, _camera, w, - h, - 1 );
	        SetPoint( "n3", pointMap, geometry, _camera, - w, h, - 1 );
	        SetPoint( "n4", pointMap, geometry, _camera, w, h, - 1 );

	        // far

	        SetPoint( "f1", pointMap, geometry, _camera, - w, - h, 1 );
	        SetPoint( "f2", pointMap, geometry, _camera, w, - h, 1 );
	        SetPoint( "f3", pointMap, geometry, _camera, - w, h, 1 );
	        SetPoint( "f4", pointMap, geometry, _camera, w, h, 1 );

	        // up

	        SetPoint( "u1", pointMap, geometry, _camera, w * 0.7f, h * 1.1f, - 1 );
	        SetPoint( "u2", pointMap, geometry, _camera, - w * 0.7f, h * 1.1f, - 1 );
	        SetPoint( "u3", pointMap, geometry, _camera, 0, h * 2, - 1 );

	        // cross

	        SetPoint( "cf1", pointMap, geometry, _camera, - w, 0, 1 );
	        SetPoint( "cf2", pointMap, geometry, _camera, w, 0, 1 );
	        SetPoint( "cf3", pointMap, geometry, _camera, 0, - h, 1 );
	        SetPoint( "cf4", pointMap, geometry, _camera, 0, h, 1 );

	        SetPoint( "cn1", pointMap, geometry, _camera, - w, 0, - 1 );
	        SetPoint( "cn2", pointMap, geometry, _camera, w, 0, - 1 );
	        SetPoint( "cn3", pointMap, geometry, _camera, 0, - h, - 1 );
	        SetPoint( "cn4", pointMap, geometry, _camera, 0, h, - 1 );

	        BufferAttribute<float> attribute =(geometry as BufferGeometry).GetAttribute<float>("position") as BufferAttribute<float>;
            attribute.NeedsUpdate = true;
        }

        public void SetPoint(string point, Hashtable pointMap, Geometry geometry, Camera camera, float x, float y, float z)
        {
            _vector.Set( x, y, z ).UnProject( camera );

	        var points = (List<int>)pointMap[ point ];

	        if ( points != null ) {

                BufferAttribute<float> position =(geometry as BufferGeometry).GetAttribute<float>("position") as BufferAttribute<float>;

		        for ( int i = 0; i< points.Count;i ++ ) {

			        position.setXYZ( points[ i ], _vector.X, _vector.Y, _vector.Z );

		        }
	        }
        }
    }
}
