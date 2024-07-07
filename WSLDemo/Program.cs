//using OpenTK.Mathematics;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.Desktop;
//using OpenTK.Graphics.OpenGL4;
//using OpenTK.Windowing.GraphicsLibraryFramework;
//using THREE;
//using System.Reflection;

//namespace WSLDemo
//{
//    public static class Program
//    {
//        private static void Main()
//        {
//            //ShaderLib shaderlib = new ShaderLib();
//            //var shader = (GLShader)shaderlib["basic"];
//            //GLUniforms uniforms = (GLUniforms)shader.Uniforms.Clone();
//            var nativeWindowSettings = new NativeWindowSettings()
//            {
//                ClientSize = new Vector2i(800, 600),
//                Title = "LearnOpenTK - Creating a Window",
//                Flags = ContextFlags.ForwardCompatible,
//            };

//            var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
//            window.renderer.Render(window.scene, window.camera);
//            window.SwapBuffers();

//        }
//    }
//}
using System;
using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using THREE;
using WSLDemo;
class Program
{

    static void Main(string[] args)
    {
        ThreeExampleWindow window = new ThreeExampleWindow(1200, 800, "THREE Examples");
        window.Run();
    }

}

