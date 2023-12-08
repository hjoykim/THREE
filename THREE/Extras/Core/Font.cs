using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace THREE
{
	public class Font
	{
		public string Type;

		public string Data;

		public Font(string path)
		{
			Type = "Font";

			Data = File.ReadAllText(path);
		}
		public static Font Load(string path)
        {
			return new Font(path);
        }
		public List<Shape> GenerateShapes(string text, float? size)
		{

			if (size == null) size = 100;

			List<Shape> shapes = new List<Shape>();
			var paths = CreatePaths(text, size.Value, this.Data);

			for (int p = 0, pl = paths.Count; p < pl; p++)
			{							
				shapes = shapes.Concat(paths[p].ToShapes(false,false)).ToList();
			}

			return shapes;
		}
		public List<ShapePath> CreatePaths(string text, float size, string data)
		{

			text = text.Replace("\\n", "\n");
			var chars = text.ToArray();// Array.from ? Array.from(text) : String(text).split(''); // see #13988
			JObject array = JObject.Parse(data);
			var res = (float ?)array["resolution"];
			var scale = size / res;
			var yMax = Convert.ToInt32(array["boundingBox"]["yMax"].ToString());
			var yMin = Convert.ToInt32(array["boundingBox"]["yMin"].ToString());
			var underlineThickness = Convert.ToInt32(array["underlineThickness"]);
			var line_height = (yMax - yMin + underlineThickness) * scale.Value;// (data.boundingBox.yMax - data.boundingBox.yMin + data.underlineThickness) * scale;

			var paths = new List<ShapePath>();

			float offsetX = 0, offsetY = 0;

			for (int i = 0; i < chars.Length; i++)
			{

				var ch = chars[i];

				if (ch == '\n')
				{

					offsetX = 0;
					offsetY -=line_height;

				}
				else
				{

					var ret = CreatePath(ch.ToString(), scale.Value, offsetX, offsetY, array);
					offsetX += (float)ret["OffsetX"];
					paths.Add(ret["path"] as ShapePath);

				}
			}

			return paths;

		}

		public Hashtable CreatePath(string ch, float scale, float offsetX, float offsetY, JObject array)
		{

			var glyph = array["glyphs"][ch] != null ? array["glyphs"][ch] : array["glyphs"]["?"];

			if (glyph == null)
			{

				Console.WriteLine("THREE.Font: character " + ch + " does not exists in font family " + array["familyName"].ToString() + ".");

				return null;

			}

			var path = new ShapePath();

			float x, y, cpx, cpy, cpx1, cpy1, cpx2, cpy2;

			if (glyph["o"] != null)
			{

				var outline = glyph["_cachedOutline"]!=null ? glyph["_cachedOutline"].ToString().Split(' ') : glyph["o"].ToString().Split(' '); 

				for (int i = 0, l = outline.Length; i < l;)
				{

					var action = outline[i++].ToString();

					switch (action)
					{

						case "m": // moveTo

							x = Convert.ToSingle(outline[i++].ToString()) * scale + offsetX;
							y = Convert.ToSingle(outline[i++].ToString()) * scale + offsetY;

							path.MoveTo(x, y);

							break;

						case "l": // lineTo

							x = Convert.ToSingle(outline[i++].ToString()) * scale + offsetX;
							y = Convert.ToSingle(outline[i++].ToString()) * scale + offsetY;

							path.LineTo(x, y);

							break;

						case "q": // quadraticCurveTo

							cpx = Convert.ToSingle(outline[i++].ToString()) * scale + offsetX;
							cpy = Convert.ToSingle(outline[i++].ToString()) * scale + offsetY;
							cpx1 = Convert.ToSingle(outline[i++].ToString()) * scale + offsetX;
							cpy1 = Convert.ToSingle(outline[i++].ToString()) * scale + offsetY;

							path.QuadraticCurveTo(cpx1, cpy1, cpx, cpy);

							break;

						case "b": // bezierCurveTo

							cpx = Convert.ToSingle(outline[i++].ToString()) * scale + offsetX;
							cpy = Convert.ToSingle(outline[i++].ToString()) * scale + offsetY;
							cpx1 = Convert.ToSingle(outline[i++].ToString()) * scale + offsetX;
							cpy1 = Convert.ToSingle(outline[i++].ToString()) * scale + offsetY;
							cpx2 = Convert.ToSingle(outline[i++].ToString()) * scale + offsetX;
							cpy2 = Convert.ToSingle(outline[i++].ToString()) * scale + offsetY;

							path.BezierCurveTo(cpx1, cpy1, cpx2, cpy2, cpx, cpy);

							break;

					}

				}

			}

			return new Hashtable() { { "OffsetX", Convert.ToSingle(glyph["ha"].ToString()) * scale }, { "path", path } };

		}
	}
}
