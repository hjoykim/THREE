using System.Collections.Generic;

namespace THREE
{
    public class GridHelper : LineSegments
    {
        public GridHelper(int size=10,int divisions=10,int c1 = 0x444444,int c2 = 0x888888) : base()
        {
			Color color1 = new Color(c1);
			Color color2 = new Color(c2);

			var center = divisions / 2;
			var step = size / divisions;
			var halfSize = size / 2;

			List<float> vertices = new List<float>();
			List<float> colors = new List<float>();


			for (int i = 0, k = -halfSize; i <= divisions; i++, k += step)
			{

				vertices.Add(-halfSize, 0, k, halfSize, 0, k);
				vertices.Add(k, 0, -halfSize, k, 0, halfSize);

				var color = (i == center) ? color1 : color2;

				colors.Add(color.R, color.G, color.B);
				colors.Add(color.R, color.G, color.B);
				colors.Add(color.R, color.G, color.B);
				colors.Add(color.R, color.G, color.B);

			}

			var geometry = new BufferGeometry();
			geometry.SetAttribute("position", new BufferAttribute<float>(vertices.ToArray(), 3));
			geometry.SetAttribute("color", new BufferAttribute<float>(colors.ToArray(), 3));

			var material = new LineBasicMaterial() { VertexColors= true, ToneMapped = false };

			List<Material> materialList = new List<Material>() { material };
			InitGeometry(geometry, materialList);

			this.type = "GridHelper";
        }
    }
}
