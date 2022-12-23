
namespace THREE
{
    public class PlaneHelper : Line
    {
        public Plane Plane { get; set; }
		public float Size { get; set; }
        public PlaneHelper(Plane plane,float? size,int? hex) : base()
        {
			var color = (hex != null) ? hex.Value : 0xffff00;

			float[] positions = new float[] { 1, -1, 1, -1, 1, 1, -1, -1, 1, 1, 1, 1, -1, 1, 1, -1, -1, 1, 1, -1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0 };

			var geometry = new BufferGeometry();
			geometry.SetAttribute("position", new BufferAttribute<float>(positions, 3));
			geometry.ComputeBoundingSphere();

			this.InitGeometry(geometry, new LineBasicMaterial() { Color = Color.Hex(color), ToneMapped = false });

			this.type = "PlaneHelper";

			this.Plane = plane;

			this.Size = (size == null) ? 1 : size.Value;

			float[] positions2 = new float[] { 1, 1, 1, -1, 1, 1, -1, -1, 1, 1, 1, 1, -1, -1, 1, 1, -1, 1 };

			var geometry2 = new BufferGeometry();
			geometry2.SetAttribute("position", new BufferAttribute<float>(positions2, 3));
			geometry2.ComputeBoundingSphere();

			this.Add(new Mesh(geometry2, new MeshBasicMaterial() { Color= Color.Hex(color), Opacity= 0.2f, Transparent= true, DepthWrite= false, ToneMapped= false }) );
		}

        public override void UpdateMatrixWorld(bool force = false)
        {
			var scale = -this.Plane.Constant;

			if (System.Math.Abs(scale) < 1e-8) scale = 1e-8f; // sign does not matter

			this.Scale.Set(0.5f * this.Size, 0.5f * this.Size, scale);

			this.Children[0].Material.Side = (scale < 0) ? Constants.BackSide : Constants.FrontSide; // renderer flips side when determinant < 0; flipping not wanted here

			this.LookAt(this.Plane.Normal);

			base.UpdateMatrixWorld(force);
        }
    }
}
