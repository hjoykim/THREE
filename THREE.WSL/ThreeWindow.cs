using System;
using System.Runtime.ExceptionServices;
using System.ComponentModel;
using System.Collections.Concurrent;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Reflection;
using ErrorCode = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;
using System.Drawing;
using System.Runtime.InteropServices;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;


namespace THREE.WSL
{
    public unsafe class ThreeWindow : IThreeWindow,IDisposable
    {
        public Window* windowPtr { get; set; }        
        public Rectangle Bounds { get; set; }
        public int Width { get; set; }        
        public int Height { get; set; }
        public string Title;
        public IGraphicsContext Context { get; set; }
        public float AspectRatio
        {
            get
            {
                if (Height == 0) return 1;
                else return (float)Width / Height;
            }
        }
        private static ConcurrentQueue<ExceptionDispatchInfo> _callbackExceptions = new ConcurrentQueue<ExceptionDispatchInfo>();
        private bool _isFocused;
        public bool IsFocused
        {
            get => _isFocused;
        }
        private WindowState _windowState = WindowState.Normal;
        #region Mouse Action
        public event Action<WindowPositionEventArgs> Move;
        public event Action<ResizeEventArgs> Resize;
        public event Action<FramebufferResizeEventArgs> FramebufferResize;
        public event Action Refresh;
        public event Action<CancelEventArgs> Closing;
        public event Action<MinimizedEventArgs> Minimized;
        public event Action<MaximizedEventArgs> Maximized;
        public event Action<JoystickEventArgs> JoystickConnected;
        public event Action<FocusedChangedEventArgs> FocusedChanged;
        public event Action<KeyboardKeyEventArgs> KeyDown;
        public event Action<TextInputEventArgs> TextInput;
        public event Action<KeyboardKeyEventArgs> KeyUp;
        public event Action MouseLeave;
        public event Action MouseEnter;
        public event Action<MouseButtonEventArgs> MouseDown;
        public event Action<MouseButtonEventArgs> MouseUp;
        public event Action<MouseMoveEventArgs> MouseMove;
        public event Action<MouseWheelEventArgs> MouseWheel;
        public event Action<FileDropEventArgs> FileDrop;
        #endregion

        public ThreeWindow(int width, int height,string title)
        {
            PrepareContext();
            this.Width = width;
            this.Height = height;
            this.Title = title;
            windowPtr = CreateWindow(width, height, title);
        }
        ~ThreeWindow()
        {
            Dispose(disposing: true);
        }
        private void PrepareContext()
        {
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            GLFW.WindowHint(WindowHintBool.DoubleBuffer, true);
            GLFW.WindowHint(WindowHintBool.Decorated, true);
        }
        private static void InitializeGlBindings()
        {
            Assembly assembly;
            GLFWBindingsContext provider;
            try
            {
                assembly = Assembly.Load("OpenTK.Graphics");
            }
            catch
            {
                return;
            }
            provider = new GLFWBindingsContext();
            void LoadBindings(string typeNamespace)
            {
                Type type = assembly.GetType("OpenTK.Graphics." + typeNamespace + ".GL");
                if (!(type == null))
                {
                    MethodInfo? method = type.GetMethod("LoadBindings");
                    if (method == null)
                    {
                        throw new MissingMethodException("OpenTK tried to auto-load the OpenGL bindings. We found the OpenTK.Graphics." + typeNamespace + ".GL class, but we could not find the 'LoadBindings' method. If you are trying to run a trimmed assembly please add a [DynamicDependency()] attribute to your program, or set NativeWindowSettings.AutoLoadBindings = false and load the OpenGL bindings manually.");
                    }

                    method?.Invoke(null, new object[1] { provider });
                }
            }
            LoadBindings("ES11");
            LoadBindings("ES20");
            LoadBindings("ES30");
            LoadBindings("OpenGL");
            LoadBindings("OpenGL4");
        }
        public THREE.Vector2 GetMousePosition()
        {
            GLFW.GetCursorPos(windowPtr, out var x, out var y);
            return new Vector2((float)x, (float)y);
        }
        public void MakeCurrent()
        {
            GLFW.MakeContextCurrent(windowPtr);
        }

        public void SwapBuffers()
        {
            GLFW.SwapBuffers(windowPtr);
        }

        public void PollEvents()
        {
            GLFW.PollEvents();
        }
        private unsafe Window* CreateWindow(int width,int height,string title)
        {
            GLFWProvider.EnsureInitialized();
            // Create window, make the OpenGL context current on the thread, and import graphics functions
            var window = GLFW.CreateWindow(width, height, title, null, (Window*)IntPtr.Zero);
            var primaryMonitor = GLFW.GetPrimaryMonitor();
            GLFW.GetMonitorWorkarea(primaryMonitor,out int sx,out int sy,out int swidth,out int sheight);
            var x = (swidth-width)/2;
            var y =(sheight-height)/2;
            GLFW.SetWindowPos(window,x,y);
            GLFW.MakeContextCurrent(window);
            InitializeGlBindings();
            RegisterWindowCallbacks(window);

            GLFW.GetCursorPos(window, out var xPos, out var yPos);
            _lastReportedMousePos = new OpenTK.Mathematics.Vector2((float)xPos, (float)yPos);
            _isFocused = GLFW.GetWindowAttrib(window, WindowAttributeGetBool.Focused);
            return window;

        }
        public virtual void RenderFrame()
        {
            GLFW.PollEvents();
            GLFW.SwapBuffers(windowPtr);
        }
        public void Run()
        {
            OnLoad();           
            OnResize(new ResizeEventArgs(Width,Height));

            while(!GLFW.WindowShouldClose(windowPtr))
            {
                RenderFrame();
                //ImGui.NewFrame();
                //ImGui.ShowDemoWindow();
                //ImGui.Render();
                //imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());

                //imGuiManager.ImDraw();
                //GLFW.SwapBuffers(windowPtr);

            }
        }

 

        public virtual void OnLoad()
        {
            Context = new GLFWGraphicsContext(windowPtr);
        }

        private unsafe WindowState GetWindowStateFromGLFW()
        {
            if (GLFW.GetWindowAttrib(windowPtr, WindowAttributeGetBool.Iconified))
            {
                return WindowState.Minimized;
            }

            if (GLFW.GetWindowAttrib(windowPtr, WindowAttributeGetBool.Maximized))
            {
                return WindowState.Maximized;
            }

            if (GLFW.GetWindowMonitor(windowPtr) != null)
            {
                return WindowState.Fullscreen;
            }

            return WindowState.Normal;
        }

        private OpenTK.Mathematics.Vector2 _lastReportedMousePos;
        private GLFWCallbacks.ErrorCallback _errorCallback;
        private GLFWCallbacks.WindowPosCallback _windowPosCallback;
        //private GLFWCallbacks.WindowSizeCallback _windowSizeCallback;
        private GLFWCallbacks.FramebufferSizeCallback _framebufferSizeCallback;
        private GLFWCallbacks.WindowIconifyCallback _windowIconifyCallback;
        private GLFWCallbacks.WindowMaximizeCallback _windowMaximizeCallback;
        private GLFWCallbacks.WindowFocusCallback _windowFocusCallback;
        private GLFWCallbacks.CharCallback _charCallback;
        private GLFWCallbacks.ScrollCallback _scrollCallback;
        private GLFWCallbacks.WindowRefreshCallback _windowRefreshCallback;
        private GLFWCallbacks.WindowCloseCallback _windowCloseCallback;
        private GLFWCallbacks.KeyCallback _keyCallback;
        private GLFWCallbacks.CursorEnterCallback _cursorEnterCallback;
        private GLFWCallbacks.MouseButtonCallback _mouseButtonCallback;
        private GLFWCallbacks.CursorPosCallback _cursorPosCallback;
        private GLFWCallbacks.DropCallback _dropCallback;
        private GLFWCallbacks.JoystickCallback _joystickCallback;
        private unsafe void RegisterWindowCallbacks(Window* window)
        {
            _errorCallback = WindowErrorCallback;
            _windowPosCallback = WindowPosCallback;
            //_windowSizeCallback = WindowSizeCallback;
            _framebufferSizeCallback = FramebufferSizeCallback;
            _windowCloseCallback = WindowCloseCallback;
            _windowRefreshCallback = WindowRefreshCallback;
            _windowFocusCallback = WindowFocusCallback;
            _windowIconifyCallback = WindowIconifyCallback;
            _windowMaximizeCallback = WindowMaximizeCallback;
            _mouseButtonCallback = MouseButtonCallback;
            _cursorPosCallback = CursorPosCallback;
            _cursorEnterCallback = CursorEnterCallback;
            _scrollCallback = ScrollCallback;
            _keyCallback = KeyCallback;
            _charCallback = CharCallback;
            _dropCallback = DropCallback;

            GLFW.SetWindowPosCallback(window, _windowPosCallback);
            //GLFW.SetWindowSizeCallback(window, _windowSizeCallback);
            GLFW.SetFramebufferSizeCallback(window, _framebufferSizeCallback);
            GLFW.SetWindowCloseCallback(window, _windowCloseCallback);
            GLFW.SetWindowRefreshCallback(window, _windowRefreshCallback);
            GLFW.SetWindowFocusCallback(window, _windowFocusCallback);
            GLFW.SetWindowIconifyCallback(window, _windowIconifyCallback);
            GLFW.SetWindowMaximizeCallback(window, _windowMaximizeCallback);
            GLFW.SetMouseButtonCallback(window, _mouseButtonCallback);
            GLFW.SetCursorPosCallback(window, _cursorPosCallback);
            GLFW.SetCursorEnterCallback(window, _cursorEnterCallback);
            GLFW.SetScrollCallback(window, _scrollCallback);
            GLFW.SetKeyCallback(window, _keyCallback);
            GLFW.SetCharCallback(window, _charCallback);
            GLFW.SetDropCallback(window, _dropCallback);

            //_framebufferSizeCallback = WindowFramebufferSizeCallback;
            //_cursorPosCallback = CursorPosCallbakc;
            //_mouseButtonCallback = MouseButtonCallback;
            //GLFW.SetErrorCallback(_errorCallback);
            //GLFW.SetFramebufferSizeCallback(window,_framebufferSizeCallback);
            //GLFW.SetCursorPosCallback(window,_cursorPosCallback);
            //GLFW.SetMouseButtonCallback(window,_mouseButtonCallback);
            //GLFW.SetScrollCallback(window,_scrollCallback);

        }
        protected virtual void OnMove(WindowPositionEventArgs e)
        {
            this.Move?.Invoke(e);
        }
        private unsafe void WindowPosCallback(Window* window, int x, int y)
        {
            try
            {
                OnMove(new WindowPositionEventArgs(x, y));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        private unsafe void WindowSizeCallback(Window* window, int width, int height)
        {
            try
            {
                OnResize(new ResizeEventArgs(width, height));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }
        protected virtual void OnClosing(CancelEventArgs e)
        {
            this.Closing?.Invoke(e);
        }
        private unsafe void WindowCloseCallback(Window* window)
        {
            try
            {
                CancelEventArgs cancelEventArgs = new CancelEventArgs();
                OnClosing(cancelEventArgs);
                if (cancelEventArgs.Cancel)
                {
                    GLFW.SetWindowShouldClose(windowPtr, value: false);
                }
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnRefresh()
        {
            this.Refresh?.Invoke();
        }
        private unsafe void WindowRefreshCallback(Window* window)
        {
            try
            {
                OnRefresh();
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            this.FocusedChanged?.Invoke(e);
            _isFocused = e.IsFocused;
        }
        private unsafe void WindowFocusCallback(Window* window, bool focused)
        {
            try
            {
                OnFocusedChanged(new FocusedChangedEventArgs(focused));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnMinimized(MinimizedEventArgs e)
        {
            _windowState = (e.IsMinimized ? WindowState.Minimized : GetWindowStateFromGLFW());
            this.Minimized?.Invoke(e);
        }
        private unsafe void WindowIconifyCallback(Window* window, bool iconified)
        {
            try
            {
                OnMinimized(new MinimizedEventArgs(iconified));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnMaximized(MaximizedEventArgs e)
        {
            _windowState = (e.IsMaximized ? WindowState.Maximized : GetWindowStateFromGLFW());
            this.Maximized?.Invoke(e);
        }
        private unsafe void WindowMaximizeCallback(Window* window, bool maximized)
        {
            try
            {
                OnMaximized(new MaximizedEventArgs(maximized));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        private unsafe void WindowErrorCallback(ErrorCode code,string message)
        {
            Debug.WriteLine(code.ToString() + "," + message);
        }
        private unsafe void FramebufferSizeCallback(Window* window, int width, int height)
        {
            Width = width; Height = height;
            OnResize(new ResizeEventArgs(width, height));
        }
        private unsafe void CursorPosCallback(Window* window, double posX, double posY)
        {
            try
            {
                OpenTK.Mathematics.Vector2 vector = new OpenTK.Mathematics.Vector2((float)posX, (float)posY);
                OpenTK.Mathematics.Vector2 delta = vector - _lastReportedMousePos;
                _lastReportedMousePos = vector;
                OnMouseMove(new MouseMoveEventArgs(vector, delta));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        public unsafe virtual void MouseButtonCallback(Window* window,MouseButton button,InputAction action,KeyModifiers modes)
        {
            try
            {
                MouseButtonEventArgs e = new MouseButtonEventArgs(button, action, modes);
                if (action == InputAction.Release)
                {
                    //MouseState[button] = false;
                    OnMouseUp(e);
                }
                else
                {
                    //MouseState[button] = true;
                    OnMouseDown(e);
                }
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnMouseEnter()
        {
            this.MouseEnter?.Invoke();
        }
        protected virtual void OnMouseLeave()
        {
            this.MouseLeave?.Invoke();
        }
        private unsafe void CursorEnterCallback(Window* window, bool entered)
        {
            try
            {
                if (entered)
                {
                    OnMouseEnter();
                }
                else
                {
                    OnMouseLeave();
                }
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        private unsafe void ScrollCallback(Window* window, double offsetX, double offsetY)
        {
            try
            {

                OnMouseWheel(new MouseWheelEventArgs((float)offsetX,(float)offsetY));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
            this.KeyDown?.Invoke(e);
        }

        protected virtual void OnKeyUp(KeyboardKeyEventArgs e)
        {
            this.KeyUp?.Invoke(e);
        }
        private unsafe void KeyCallback(Window* window, Keys key, int scancode, InputAction action, KeyModifiers mods)
        {
            try
            {
                KeyboardKeyEventArgs args = new KeyboardKeyEventArgs(key, scancode, mods, action == InputAction.Repeat);
                if (action == InputAction.Release)
                {


                    OnKeyUp(args);
                }
                else
                {


                    OnKeyDown(args);
                }
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnTextInput(TextInputEventArgs e)
        {
            this.TextInput?.Invoke(e);
        }
        private unsafe void CharCallback(Window* window, uint codepoint)
        {
            try
            {
                OnTextInput(new TextInputEventArgs((int)codepoint));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        protected virtual void OnFileDrop(FileDropEventArgs e)
        {
            this.FileDrop?.Invoke(e);
        }
        private unsafe void DropCallback(Window* window, int count, byte** paths)
        {
            try
            {
                string[] array = new string[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = Marshal.PtrToStringUTF8((IntPtr)paths[i]);
                }

                OnFileDrop(new FileDropEventArgs(array));
            }
            catch (Exception source)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(source));
            }
        }
        public virtual void OnMouseDown(MouseButtonEventArgs args)
        {
            this.MouseDown?.Invoke(args);
        }
        public virtual void OnMouseUp(MouseButtonEventArgs args)
        {
            this.MouseUp?.Invoke(args);
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs args)
        {
            this.MouseWheel?.Invoke(args);
        }

        public virtual void OnMouseMove(MouseMoveEventArgs args)
        {
            this.MouseMove?.Invoke(args);
        }
        public virtual void OnResize(ResizeEventArgs clientSize)
        {
            Resize?.Invoke(clientSize);
        }
        #region Dispose
        private bool _disposedValue;
        protected unsafe virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (!GLFWProvider.IsOnMainThread)
                {
                    throw new GLFWException("You can only dispose windows on the main thread. The window needs to be disposed as it cannot safely be disposed in the finalizer.");
                }

                GLFW.DestroyWindow(windowPtr);
                _disposedValue = true;
            }
        }
        public virtual void OnDispose()
        {

        }
        public void Dispose()
        {
            OnDispose();
            Dispose(disposing: true);
        }
        #endregion
    }
}