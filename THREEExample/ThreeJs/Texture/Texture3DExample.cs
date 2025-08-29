using ImGuiNET;
using OpenTK.Windowing.Common;
using Rhino.DocObjects;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using THREE;
using Color = THREE.Color;
namespace THREEExample
{
    [Example("Texture3D", ExampleCategory.ThreeJs, "Texture")]
    public class Texture3DExample : Example
    {
        OrthographicCamera ocamera;
        OrbitControls ocontrols;
        public Texture3DExample() : base()
        {
            scene.Background = new Color(0x000000);
        }
        public override void InitCamera()
        {
            var h = 512;
            ocamera = new THREE.OrthographicCamera(-h * glControl.AspectRatio / 2, h * glControl.AspectRatio / 2, h / 2, -h / 2, 1, 1000);
            ocamera.Position.Set(0, 0, 128);
            ocamera.Up.Set(0, 0, 1);
            ocamera.UpdateProjectionMatrix();
        }
        public override void InitCameraController()
        {
            ocontrols = new OrbitControls(this, ocamera);
            ocontrols.target.Set(64, 64, 128);
            ocontrols.MinZoom = 0.5f;
            ocontrols.MaxZoom = 4;
            ocontrols.Update();
        }
        public override void InitLighting()
        {
            base.InitLighting();
            scene.Add(new THREE.AmbientLight(0xaaaaaa, 0.2f));
            var light = new THREE.DirectionalLight(0xddffdd, 0.6f);
            light.Position.Set(1, 1, 1);
            light.CastShadow = true;
            light.Shadow.MapSize.Width = 1024;
            light.Shadow.MapSize.Height = 1024;
            var d = 10;
            light.Shadow.Camera.Left = -d;
            light.Shadow.Camera.CameraRight = d;
            light.Shadow.Camera.Top = d;
            light.Shadow.Camera.Bottom = -d;
            light.Shadow.Camera.Far = 1000;
            scene.Add(light);
        }
        byte[] FloatArrayToByteArray(float[] floatArray, float min, float max)
        {
            int length = floatArray.Length;
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                // min~max 범위의 값을 0~255로 정규화
                float normalized = (floatArray[i] - min) / (max - min);
                normalized = Math.Clamp(normalized, 0f, 1f);
                result[i] = (byte)(normalized * 255f);
            }
            return result;
        }
        private SKBitmap CreateSKBitmapFromByteArray(byte[] bytes, int width, int height)
        {
            var bitmap = new SKBitmap(width, height, SKColorType.Gray8, SKAlphaType.Opaque);
            var pixmap = bitmap.PeekPixels();

            unsafe
            {
                byte* ptr = (byte*)pixmap.GetPixels().ToPointer();
                for (int i = 0; i < bytes.Length && i < width * height; i++)
                {
                    ptr[i] = bytes[i];
                }
            }
            return bitmap;
        }

        public override void Init()
        {
            base.Init();

            NRRDLoader loader = new NRRDLoader();
            var volume = loader.Load("../../../../assets/models/nrrd/stent.nrrd");
            float[] floatArray = volume.Data as float[];
            //byte[] bytes  = FloatArrayToByteArray(floatArray, volume.Min, volume.Max);
            //SKBitmap bitmap = CreateSKBitmapFromByteArray(bytes,volume.XLength,volume.YLength);

            var texture = new THREE.DataTexture3D(floatArray, volume.XLength, volume.YLength, volume.ZLength);
            texture.Format = Constants.RedFormat;
            texture.MinFilter = texture.MagFilter = Constants.LinearFilter;
            texture.UnpackAlignment = 1;

            Hashtable cmtextures = new Hashtable()
            {
                {"viridis", TextureLoader.Load("../../../../assets/textures/cm_viridis.png")},
                {"gray", TextureLoader.Load("../../../../assets/textures/cm_gray.png")}
            };

            var shader = new VolumeRenderShader1();
            (shader.Uniforms["u_data"] as GLUniform)["value"] = texture;
            (shader.Uniforms["u_size"] as GLUniform)["value"] = new Vector3(volume.XLength, volume.YLength, volume.ZLength);
            (shader.Uniforms["u_clim"] as GLUniform)["value"] = new Vector2(0, 1);
            (shader.Uniforms["u_renderstyle"] as GLUniform)["value"] = 0;
            (shader.Uniforms["u_renderthreshold"] as GLUniform)["value"] = 0.15f;
            (shader.Uniforms["u_cmdata"] as GLUniform)["value"] = cmtextures["viridis"] as THREE.Texture;

            var material = new THREE.ShaderMaterial()
            {
                Uniforms = shader.Uniforms,
                VertexShader = shader.VertexShader,
                FragmentShader = shader.FragmentShader,
                Side = Constants.BackSide
            };
            material.Transparent = true;
            var geometry = new THREE.BoxGeometry(volume.XLength, volume.YLength, volume.ZLength);
            geometry.Translate(volume.XLength / 2 - 0.5f, volume.YLength / 2 - 0.5f, volume.ZLength / 2 - 0.5f);

            var mesh = new THREE.Mesh(geometry, material);
            scene.Add(mesh);

            //AddGuiControlsAction = () =>
            //{
            //    ImGui.Text("Controls");
            //    var uclim1 = (Vector2)(shader.Uniforms["u_clim"] as GLUniform)["value"];
            //    System.Numerics.Vector2 clim1 = new System.Numerics.Vector2(uclim1.X, uclim1.Y);
            //    if (ImGui.SliderFloat2("clim1", ref clim1, 0.0f, 1.0f))
            //    {
            //        ((shader.Uniforms["u_clim"] as GLUniform)["value"] as Vector2).Set(clim1.X, clim1.Y);
            //    }
            //    var usize = (Vector3)(shader.Uniforms["u_size"] as GLUniform)["value"];
            //    System.Numerics.Vector3 size = new System.Numerics.Vector3(usize.X, usize.Y, usize.Z);
            //    if (ImGui.SliderFloat3("size", ref size, 0.0f, 1.0f))
            //    {
            //        ((shader.Uniforms["u_size"] as GLUniform)["value"] as Vector3).Set(size.X, size.Y, size.Z);
            //    }
            //};
        }

        public override void OnResize(ResizeEventArgs clientSize)
        {
            if (renderer != null)
            {
                renderer.Resize(clientSize.Width, clientSize.Height);
                ocamera.Aspect = this.glControl.AspectRatio;
                var frustumHeight = ocamera.Top - ocamera.Bottom;

                camera.Left = -frustumHeight * ocamera.Aspect / 2;
                camera.CameraRight = frustumHeight * ocamera.Aspect / 2;
                camera.UpdateProjectionMatrix();
            }
            base.OnResize(clientSize);

        }
        public override void Render()
        {
            if (!imGuiManager.ImWantMouse)
                ocontrols.Enabled = true;
            else
                ocontrols.Enabled = false;

            ocontrols?.Update();
            renderer?.Render(scene, ocamera);

        }
    }
}
