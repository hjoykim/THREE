using THREE;
namespace THREEExample.Three.Buffergeometry
{
    [Example("buffergeometry_point_interleaved", ExampleCategory.ThreeJs, "Buffergeometry")]
    public class BuffergeometryPointsInterleaved : BuffergeometryPointExample
    {
        public BuffergeometryPointsInterleaved() : base() { }

		public override void CreateObject()
		{
			var particles = 500000;

			var geometry = new BufferGeometry();

			// create a generic buffer of binary data (a single particle has 16 bytes of data)

			var arrayBuffer = new float[particles * 16];

			// the following typed arrays share the same buffer

			var interleavedFloat32Buffer = new float[arrayBuffer.Length];
			var interleavedUint8Buffer = new float[arrayBuffer.Length*4];

			//

			var color = new Color();

			var n = 1000;
			var n2 = n / 2; // particles spread in the cube

			for (var i = 0; i < interleavedFloat32Buffer.Length; i += 4)
			{

				// position (first 12 bytes)

				var x = MathUtils.NextFloat() * n - n2;
				var y = MathUtils.NextFloat() * n - n2;
				var z = MathUtils.NextFloat() * n - n2;

				interleavedFloat32Buffer[i + 0] = x;
				interleavedFloat32Buffer[i + 1] = y;
				interleavedFloat32Buffer[i + 2] = z;

				// color (last 4 bytes)

				var vx = (x / n) + 0.5f;
				var vy = (y / n) + 0.5f;
				var vz = (z / n) + 0.5f;

				color.SetRGB(vx, vy, vz);

				var j = (i + 3) * 4;
				if (j >= interleavedUint8Buffer.Length) continue;
				interleavedUint8Buffer[j + 0] = (color.R);
				interleavedUint8Buffer[j + 1] = (color.G);
				interleavedUint8Buffer[j + 2] = (color.B);
				interleavedUint8Buffer[j + 3] = 0; // not needed

			}

			var interleavedBuffer32 = new InterleavedBuffer<float>(interleavedFloat32Buffer, 4);
			var interleavedBuffer8 = new InterleavedBuffer<float>(interleavedUint8Buffer, 16);

			geometry.SetAttribute("position", new InterleavedBufferAttribute<float>(interleavedBuffer32, 3, 0, false));
			geometry.SetAttribute("color", new InterleavedBufferAttribute<float>(interleavedBuffer8, 3, 12, false));

			//

			var material = new PointsMaterial() { Size = 15, VertexColors = true };

			points = new Points(geometry, material);
			scene.Add(points);
		}
    }
}
