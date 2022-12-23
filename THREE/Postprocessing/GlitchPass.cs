using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace THREE
{
    public class GlitchPass : Pass
    {
        GLUniforms uniforms;
        ShaderMaterial material;
        bool goWild = false;
        float curF = 0.0f;
        int randX = 0;
        Random random = new Random();
        public GlitchPass(float? dt_size=null) : base()
        {
            var shader = new DigitalGlitch();

            uniforms = UniformsUtils.CloneUniforms(shader.Uniforms);

            if (dt_size == null) dt_size = 64;
            if (dt_size != null && dt_size.Value == 0) dt_size = 64;
            (uniforms["tDisp"] as GLUniform)["value"] = GenerateHeightmap(dt_size.Value);

            this.material = new ShaderMaterial {
                Uniforms = this.uniforms,
                VertexShader = shader.VertexShader,
                FragmentShader = shader.FragmentShader
            };

            this.fullScreenQuad = new Pass.FullScreenQuad(this.material);

            this.goWild = false;
            this.curF = 0;
            GenerateTrigger();
        }

        public override void Render(GLRenderer renderer, GLRenderTarget writeBuffer, GLRenderTarget readBuffer, float? deltaTime = null, bool? maskActive = null)
        {
            (this.uniforms["tDiffuse"] as GLUniform)["value"] = readBuffer.Texture;
            (this.uniforms["seed"]as GLUniform)["value"] = (float)random.NextDouble();//default seeding
            (this.uniforms["byp"]as GLUniform)["value"] = 0;

            if (this.curF % this.randX == 0 || this.goWild == true)
            {

                (this.uniforms["amount"]as GLUniform)["value"] = (float)random.NextDouble() / 30;
                (this.uniforms["angle"]as GLUniform)["value"] = MathUtils.NextFloat((float)-System.Math.PI,(float)System.Math.PI);
                (this.uniforms["seed_x"]as GLUniform)["value"] = MathUtils.NextFloat(-1, 1);
                (this.uniforms["seed_y"]as GLUniform)["value"] = MathUtils.NextFloat(-1, 1);
                (this.uniforms["distortion_x"]as GLUniform)["value"] = MathUtils.NextFloat(0, 1);
                (this.uniforms["distortion_y"]as GLUniform)["value"] = MathUtils.NextFloat(0, 1);
                this.curF = 0;
                this.GenerateTrigger();

            }
            else if (this.curF % this.randX < this.randX / 5)
            {

                (this.uniforms["amount"]as GLUniform)["value"] = (float)random.NextDouble()/ 90;
                (this.uniforms["angle"]as GLUniform)["value"] = MathUtils.NextFloat(-(float)System.Math.PI, (float)System.Math.PI);
                (this.uniforms["distortion_x"]as GLUniform)["value"] = MathUtils.NextFloat(0, 1);
                (this.uniforms["distortion_y"]as GLUniform)["value"] = MathUtils.NextFloat(0, 1);
                (this.uniforms["seed_x"]as GLUniform)["value"] = MathUtils.NextFloat(-0.3f, 0.3f);
                (this.uniforms["seed_y"]as GLUniform)["value"] = MathUtils.NextFloat(-0.3f, 0.3f);

            }
            else if (this.goWild == false)
            {

                (this.uniforms["byp"]as GLUniform)["value"] = 1;

            }

            this.curF++;

            if (this.RenderToScreen)
            {

                renderer.SetRenderTarget(null);
                this.fullScreenQuad.Render(renderer);

            }
            else
            {

                renderer.SetRenderTarget(writeBuffer);
                if (this.Clear) renderer.Clear();
                this.fullScreenQuad.Render(renderer);

            }
        }

        public override void SetSize(float width, float height)
        {
            
        }
        private void GenerateTrigger()
        {
            this.randX = random.Next(120, 240);
        }

        private DataTexture GenerateHeightmap(float dt_size)
        {
            var data_arr = new byte[(int)(dt_size * dt_size * 3)];
            var length = dt_size * dt_size;

            for (var i = 0; i < length; i++)
            {

                var val = (byte)(random.NextDouble()*255);
                data_arr[i * 3 + 0] = val;
                data_arr[i * 3 + 1] = val;
                data_arr[i * 3 + 2] = val;

            }
            Bitmap bitmap = new Bitmap((int)dt_size,(int)dt_size,PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, (int)dt_size, (int)dt_size), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);
            IntPtr iptr = bitmapData.Scan0;

            Marshal.Copy(iptr, data_arr, 0, data_arr.Length);

            bitmap.UnlockBits(bitmapData);

            return new DataTexture(bitmap, (int)dt_size, (int)dt_size, Constants.RGBAFormat, Constants.ByteType);
        }
    }
}
