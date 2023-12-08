
using System;
using System.Collections.Generic;

namespace THREE
{


    public class SpotLightHelper : Object3D
    {
        private Vector3 _vector = Vector3.Zero();

        private Light Light;

        private Color? Color;

        private LineSegments Cone;

        public SpotLightHelper(Light light, Color? color=null) : base()
        {
            this.type = "SpotLightHelper";
            this.Light = light;

            this.Light.UpdateMatrixWorld();

            this.Matrix = light.MatrixWorld;

            this.MatrixAutoUpdate = false;

            this.Color = color;

            var geometry = new BufferGeometry();

            var positions = new List<float>()
            {
                0, 0, 0, 	0, 0, 1,
		        0, 0, 0, 	1, 0, 1,
		        0, 0, 0,	-1, 0, 1,
		        0, 0, 0, 	0, 1, 1,
		        0, 0, 0, 	0, -1, 1
            };

            for (int i = 0, j = 1, l = 32; i < l; i++, j++)
            {
                var p1 = (i / l) * Math.PI * 2;
                var p2 = (j / l) * Math.PI * 2;

                positions.Add((float)Math.Cos(p1), (float)Math.Sin(p1), 1);
                positions.Add((float)Math.Cos(p2), (float)Math.Sin(p2), 1);

            }

            geometry.SetAttribute("position", new BufferAttribute<float>(positions.ToArray(), 3));

            var material = new LineBasicMaterial() { Fog = false,ToneMapped=false };

            this.Cone = new LineSegments(geometry, material);

            this.Add(Cone);

            this.Update();

        }

        public void Update()
        {
            this.Light.UpdateMatrixWorld();

	        var coneLength = this.Light.Distance!=0 ? this.Light.Distance : 1000;
	        var coneWidth = coneLength * (float)Math.Tan( this.Light.Angle );

	        this.Cone.Scale.Set( coneWidth, coneWidth, coneLength );

	        _vector.SetFromMatrixPosition( this.Light.Target.MatrixWorld );

	        this.Cone.LookAt( _vector );

	        if ( this.Color != null ) {

		        this.Cone.Material.Color = this.Color ;

	        } else {

		        this.Cone.Material.Color = this.Light.Color;

	        }
        }

        public override void Dispose()
        {
            base.Dispose();

            this.Cone.Geometry.Dispose();
            this.Cone.Material.Dispose();
        }
    }
}
