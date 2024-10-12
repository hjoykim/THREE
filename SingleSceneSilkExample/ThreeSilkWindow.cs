using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.ImGui;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MouseButton = Silk.NET.Input.MouseButton;
using THREE.Silk.Example;
namespace THREE.Silk
{
    public class ThreeSilkWindow
    {
        public IWindow window;
        public GL gl;
        public ImGuiController? imGuiManager;
        public IInputContext input;
        public IKeyboard primaryKeyboard;
        public Example.Example currentThreeContainer;
        private string _title;
        public string Title
        {
            get { return _title; }
            set { window.Title = value; _title = value; }
        }
        public ThreeSilkWindow()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            _title = "Three Example with Silk.NET";
            options.Title =_title;
            options.API = new GraphicsAPI(ContextAPI.OpenGL,ContextProfile.Compatability,ContextFlags.Default,new APIVersion(3,1));
            window = Window.Create(options);
            
            window.Load += OnLoad;
            window.Render += OnRender;
            window.FramebufferResize += OnResize;
            window.Closing += OnClose;
        }
        public virtual void InitThreeSilkWindow()
        {
            input = window.CreateInput();
            primaryKeyboard = input.Keyboards.FirstOrDefault();
            if (primaryKeyboard != null)
            {
                primaryKeyboard.KeyDown += KeyDown;
            }
            for (int i = 0; i < input.Mice.Count; i++)
            {
                //input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
                input.Mice[i].MouseDown += OnMouseDown;
                input.Mice[i].MouseUp += OnMouseUp;
            }
            gl = GL.GetApi(window);
            imGuiManager = new ImGuiController(gl, window, input);
        }

        public virtual void InitImGuiStyle()
        {
            ImGui.StyleColorsDark();
            var style = ImGui.GetStyle();
            style.Colors[(int)ImGuiCol.WindowBg].W = 0.78f;
            style.Colors[(int)ImGuiCol.FrameBg].W = 0.71f;
            style.Colors[(int)ImGuiCol.ChildBg].W = 0.78f;
        }

        public void SetCurrentExample(Example.Example example)
        {
            currentThreeContainer = example;
        }

        public virtual void OnLoad() 
        {
            InitThreeSilkWindow();
            InitImGuiStyle();

            Vector2D<int> size;
            size.X = window.Size.X;
            size.Y = window.Size.Y;
            if (currentThreeContainer != null)
            {
                currentThreeContainer.Load(window);
            }
            OnResize(size);
        }

        public virtual void OnResize(Vector2D<int> newSize)
        {
            if (currentThreeContainer != null)
            {
                gl.Viewport(0,0,(uint)newSize.X,(uint)newSize.Y);
                currentThreeContainer.OnResize(new ResizeEventArgs(newSize));
            }
        }
        public virtual void KeyDown(IKeyboard keyboard,Key key,int arg)
        {
            if (currentThreeContainer == null) return;
            currentThreeContainer.OnKeyDown(key, (int)key, (KeyModifiers)arg);
        }
        public virtual void OnMouseMove(IMouse mouse,System.Numerics.Vector2 position)
        {
            if (currentThreeContainer == null) return;
            currentThreeContainer.OnMouseMove(0, (int)position.X, (int)position.Y);
        }
        public virtual void OnMouseWheel(IMouse mouse,ScrollWheel scrollWheel)
        {
            if(currentThreeContainer == null) return;
            currentThreeContainer.OnMouseWheel((int)mouse.Position.X, (int)mouse.Position.Y, (int)scrollWheel.Y * 120);
        }
        public virtual void OnMouseDown(IMouse mouse,MouseButton button)
        {
            if (currentThreeContainer == null) return;
            currentThreeContainer.OnMouseDown(button,(int)mouse.Position.X,(int)mouse.Position.Y);
        }
        public virtual void OnMouseUp(IMouse mouse,MouseButton button)
        {
            if (currentThreeContainer == null) return;
            currentThreeContainer.OnMouseUp(button,(int)mouse.Position.X, (int)mouse.Position.Y);
        }
        public virtual void OnClose()
        {
            currentThreeContainer?.Dispose();
            imGuiManager?.Dispose();
            input?.Dispose();
            gl?.Dispose();  
        }
        public virtual void OnRender(double deltaTime)
        {
            window.MakeCurrent();
            imGuiManager.Update((float)deltaTime);
            if (currentThreeContainer == null) return;
            
            currentThreeContainer.Render();
            if (currentThreeContainer.AddGuiControlsAction != null)
            {
                currentThreeContainer.AddGuiControlsAction();
            }
            imGuiManager.Render();
        }
        public virtual void Render()
        {
            window.Run();
        }
    }
}
