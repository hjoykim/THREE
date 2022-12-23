using OpenTK.Graphics.ES30;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = THREE.Color;
using Vector3 = THREE.Vector3;
namespace THREE
{
    public class LightProbeGenerator
    {
		public static LightProbe FromCubeTexture(CubeTexture cubeTexture)
		{

			float norm, lengthSq, weight, totalWeight = 0;

			var coord = new Vector3();

			var dir = new Vector3();

			var color = new Color();

			float[] shBasis = new float[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0};

			var sh = new SphericalHarmonics3();
			var shCoefficients = sh.Coefficients;

			for (var faceIndex = 0; faceIndex < 6; faceIndex++)
			{

				var image = cubeTexture.Images[faceIndex];

				var width = image.Image.Width;
				var height = image.Image.Height;

				BitmapData imageData = image.Image.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, image.Image.PixelFormat);


				var imageWidth = imageData.Width; // assumed to be square

				var pixelSize = 2.0f / imageWidth;

				var data = new byte[imageData.Height * imageData.Stride];

                try 
				{
					Marshal.Copy(imageData.Scan0, data, 0, data.Length);
				}
                finally
                {
					image.Image.UnlockBits(imageData);
                }

				for (var i = 0;i<data.Length;i += 4)
				{ // RGBA assumed

					// pixel color
					color.SetRGB(data[i] / 255.0f, data[i + 1] / 255.0f, data[i + 2] / 255.0f);

					// convert to linear color space
					ConvertColorToLinear(ref color, cubeTexture.Encoding);

					// pixel coordinate on unit cube

					var pixelIndex = i / 4.0f;

					var col = -1 + (pixelIndex % imageWidth + 0.5f) * pixelSize;

					var row = 1 - (float)(System.Math.Floor((float)pixelIndex / imageWidth) + 0.5f) * pixelSize;

					switch (faceIndex)
					{

						case 0: coord.Set(-1, row, -col); break;

						case 1: coord.Set(1, row, col); break;

						case 2: coord.Set(-col, 1, -row); break;

						case 3: coord.Set(-col, -1, row); break;

						case 4: coord.Set(-col, row, 1); break;

						case 5: coord.Set(col, row, -1); break;

					}

					// weight assigned to this pixel

					lengthSq = coord.LengthSq();

					weight = 4 / (float)(System.Math.Sqrt(lengthSq) * lengthSq);

					totalWeight += weight;

					// direction vector to this pixel
					dir.Copy(coord).Normalize();

					// evaluate SH basis functions in direction dir
					SphericalHarmonics3.GetBasisAt(dir, shBasis);

					// accummuulate
					for (var j = 0; j < 9; j++)
					{

						shCoefficients[j].X += shBasis[j] * color.R * weight;
						shCoefficients[j].Y += shBasis[j] * color.G * weight;
						shCoefficients[j].Z += shBasis[j] * color.B * weight;

					}

				}

			}

			// normalize
			norm = (4 * (float)System.Math.PI) / totalWeight;

			for (var j = 0; j < 9; j++)
			{

				shCoefficients[j].X *= norm;
				shCoefficients[j].Y *= norm;
				shCoefficients[j].Z *= norm;

			}

			return new LightProbe(sh,null);

		}
		public static LightProbe FromCubeRenderTarget(GLRenderer renderer, GLCubeRenderTarget cubeRenderTarget )
		{

			// The renderTarget must be set to RGBA in order to make readRenderTargetPixels works
			float norm, lengthSq, weight, totalWeight = 0;

			var coord = new Vector3();

			var dir = new Vector3();

			var color = new Color();

			float[] shBasis = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

			var sh = new SphericalHarmonics3();
			var shCoefficients = sh.Coefficients;

			for (var faceIndex = 0; faceIndex < 6; faceIndex++)
			{

				var imageWidth = cubeRenderTarget.Width; // assumed to be square
				var data = new byte[imageWidth * imageWidth * 4];
				renderer.ReadRenderTargetPixels(cubeRenderTarget, 0, 0, imageWidth, imageWidth, data, faceIndex);

				var pixelSize = 2.0f / imageWidth;

				for (int i = 0; i< data.Length; i += 4)
				{ // RGBA assumed

					// pixel color
					color.SetRGB(data[i] / 255.0f, data[i + 1] / 255.0f, data[i + 2] / 255.0f);

					// convert to linear color space
					ConvertColorToLinear(ref color, cubeRenderTarget.Texture.Encoding);

					// pixel coordinate on unit cube

					var pixelIndex = i / 4.0f;

					var col = -1 + (pixelIndex % imageWidth + 0.5f) * pixelSize;

					var row = 1 - (float)(System.Math.Floor((float)pixelIndex / imageWidth) + 0.5f) * pixelSize;

					switch (faceIndex)
					{

						case 0: coord.Set(1, row, -col); break;

						case 1: coord.Set(-1, row, col); break;

						case 2: coord.Set(col, 1, -row); break;

						case 3: coord.Set(col, -1, row); break;

						case 4: coord.Set(col, row, 1); break;

						case 5: coord.Set(-col, row, -1); break;

					}

					// weight assigned to this pixel

					lengthSq = coord.LengthSq();

					weight = 4 / (float)(System.Math.Sqrt(lengthSq) * lengthSq);

					totalWeight += weight;

					// direction vector to this pixel
					dir.Copy(coord).Normalize();

					// evaluate SH basis functions in direction dir
					SphericalHarmonics3.GetBasisAt(dir, shBasis);

					// accummuulate
					for (var j = 0; j < 9; j++)
					{

						shCoefficients[j].X += shBasis[j] * color.R * weight;
						shCoefficients[j].Y += shBasis[j] * color.G * weight;
						shCoefficients[j].Z += shBasis[j] * color.B * weight;

					}

				}

			}

			// normalize
			norm = (4 * (float)System.Math.PI) / totalWeight;

			for (var j = 0; j < 9; j++)
			{

				shCoefficients[j].X *= norm;
				shCoefficients[j].Y *= norm;
				shCoefficients[j].Z *= norm;

			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			if (renderer.capabilities.IsGL2 == true)
			{

				GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
				GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);

			}

			return new LightProbe(sh,null);

		}
		private static Color ConvertColorToLinear(ref Color color, int encoding) {

			switch (encoding)
			{

				case 3001:// Constants.sRGBEncoding:

					color.ConvertSRGBToLinear();
					break;

				case 3000://Constants.LinearEncoding:

					break;

				default:

					Trace.WriteLine("WARNING: LightProbeGenerator convertColorToLinear() encountered an unsupported encoding.");
					break;

			}

			return color;

		}

	}
}
