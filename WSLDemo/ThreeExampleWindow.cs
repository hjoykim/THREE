using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREEExample;
using THREEExample.ThreeImGui;
using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using THREEExample.Learning.Chapter01;
//using THREEExample.Learning.Chapter02;
using System.Collections;
using System.Reflection;
using THREE;
namespace WSLDemo
{
    public class TreeNode<T> : IEnumerable<TreeNode<T>>
    {

        public T Data { get; set; }
        public TreeNode<T> Parent { get; set; }
        public ICollection<TreeNode<T>> Children { get; set; }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsLeaf
        {
            get { return Children.Count == 0; }
        }

        public int Level
        {
            get
            {
                if (this.IsRoot)
                    return 0;
                return Parent.Level + 1;
            }
        }

        public object Tag { get; set; }

        public TreeNode(T data)
        {
            this.Data = data;
            this.Children = new LinkedList<TreeNode<T>>();

            this.ElementsIndex = new LinkedList<TreeNode<T>>();
            this.ElementsIndex.Add(this);
        }

        public TreeNode<T> AddChild(T child)
        {
            TreeNode<T> childNode = new TreeNode<T>(child) { Parent = this };
            this.Children.Add(childNode);

            this.RegisterChildForSearch(childNode);

            return childNode;
        }

        public override string ToString()
        {
            return Data != null ? Data.ToString() : "[data null]";
        }


        #region searching

        private ICollection<TreeNode<T>> ElementsIndex { get; set; }

        private void RegisterChildForSearch(TreeNode<T> node)
        {
            ElementsIndex.Add(node);
            if (Parent != null)
                Parent.RegisterChildForSearch(node);
        }

        public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate)
        {
            return this.ElementsIndex.FirstOrDefault(predicate);
        }

        #endregion


        #region iterating

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in this.Children)
            {
                foreach (var anyChild in directChild)
                    yield return anyChild;
            }
        }

        #endregion
    }
    public class ThreeExampleWindow : ThreeWindow
    {
        private Example? currentExample;     
        private ImGuiManager imGuiManager;
        private List<TreeNode<string>> examplesList;
        private Dictionary<string,Example> exampleInstances = new Dictionary<string,Example>();

        public ThreeExampleWindow(int width,int height,string title) : base(width,height,title)
        {

        }
        private  TreeNode<string> LoadExampleFromAssembly(Assembly assembly)
        {
            TreeNode<string> treeView = new TreeNode<string>("Root");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(false);

                foreach (var exampleType in attributes)
                {
                    if (exampleType is ExampleAttribute)
                    {
                        var example = exampleType as ExampleAttribute;
                        string key = example.Category.ToString();
                        TreeNode<string> rootNode = treeView.FindTreeNode(node => node.Data != null && node.Data.Equals(key));
                        if (rootNode == null)
                        {
                            rootNode = treeView.AddChild(key);
                        }
                        TreeNode<string> subNode = rootNode.FindTreeNode(node => node.Data != null && node.Data.Equals(example.Subcategory));
                        if (subNode == null)
                        {
                            subNode = rootNode.AddChild(example.Subcategory);
                        }
                        TreeNode<string> nodeItem = subNode.FindTreeNode(node => node.Data != null && node.Data.Equals(example.Title));
                        if (nodeItem == null)
                        {
                            nodeItem = subNode.AddChild(example.Title);
                            nodeItem.Tag = new ExampleInfo(type, example);
                            //var exampleInstance = (Example)Activator.CreateInstance((nodeItem.Tag as ExampleInfo).Example);
                            //if (null != exampleInstance)
                            //{
                            //    imGuiManager.Dispose();
                            //    imGuiManager = new ImGuiManager(this);
                            //    exampleInstance.imGuiManager = imGuiManager;
                            //    exampleInstances[example.Title] = exampleInstance;
                            //}
                        }
                    }
                }
            }
            return treeView;
        }
        private int SortByName(TreeNode<string> a, TreeNode<string> b)
        {
            return a.Data.CompareTo(b.Data);
        }

        private void GetExamplesList()
        {
            Type t = typeof(Example);
            examplesList = LoadExampleFromAssembly(Assembly.GetAssembly(t)).Children.ToList();
            examplesList.Sort(SortByName);
        }
        private void ShowExamplesMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Exit", "Ctrl+E"))
                    {
                        System.Environment.Exit(0);
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
            ImGui.Begin("Examples");
            for (int i = 0; i < examplesList.Count; i++)
            {
                TreeNode<string> category = examplesList[i];
                if (ImGui.CollapsingHeader(category.Data))
                {
                    List<TreeNode<string>> subList = category.Children.ToList();
                    subList.Sort(SortByName);
                    for (int j = 0; j < subList.Count; j++)
                    {
                        TreeNode<string> subCategory = subList[j];
                        if (ImGui.TreeNode(subCategory.Data))
                        {
                            List<TreeNode<string>> titleList = subCategory.Children.ToList();
                            titleList.Sort(SortByName);
                            for(int k=0; k < titleList.Count; k++)
                            {
                                var title = titleList[k];
                                if(ImGui.Button(title.Data))
                                {
                                    RunSample(title.Tag as ExampleInfo);
                                }
                            }
                            ImGui.TreePop();
                        }
                    }
                }
            }
            ImGui.End();
        }
        private string MakeExampleTitle(string exampleName)
        {
            if (string.IsNullOrEmpty(exampleName))
                return "THREEExample";
            else
                return "THREEExample : " + exampleName;
        }
        private void RunSample(ExampleInfo e)
        {
            if (null != currentExample)
            {
                currentExample.Dispose();
                currentExample = null;
                Title = MakeExampleTitle("");
            }

            //Application.Idle -= Application_Idle;

            currentExample = (Example)Activator.CreateInstance(e.Example);
            if (null != currentExample)
            {
                this.MakeCurrent();
                currentExample.Load(this);
                currentExample.imGuiManager = imGuiManager;
                Title = MakeExampleTitle(e.Attribute.Title);
                GL.Viewport(0, 0, Width, Height);
                currentExample.OnResize(new ResizeEventArgs(Width, Height));
            }
        }
        public override void OnLoad()
        {
            base.OnLoad();
            imGuiManager = new ImGuiManager(this );
            currentExample = new FirstSceneExample();
            currentExample.Load(this);
            currentExample.imGuiManager = imGuiManager;

            GetExamplesList();

        }
        public override void RenderFrame()
        {
            if(currentExample!=null)
                currentExample.Render();

            base.PollEvents();
            ImGui.NewFrame();
            ShowExamplesMenu();
            if (currentExample != null && currentExample.AddGuiControlsAction != null)
            {
                currentExample.ShowGUIControls();
            }
            ImGui.Render();
            imGuiManager.ImGui_ImplOpenGL3_RenderDrawData(ImGui.GetDrawData());
            //if (currentExample != null && currentExample.AddGuiControlsAction != null)
            //{
            //    this.currentExample.renderer.state.currentProgram = -1;
            //    this.currentExample.renderer.bindingStates.currentState = this.currentExample.renderer.bindingStates.defaultState;
            //}
            base.SwapBuffers();
        }
        public override void OnResize(ResizeEventArgs clientSize)
        {
            base.OnResize(clientSize);
            if (currentExample != null)
            {
                GL.Viewport(0,0,clientSize.Width,clientSize.Height);
                currentExample.OnResize(new ResizeEventArgs(clientSize.Width,clientSize.Height));
            }
        }
        public override void OnDispose()
        {
            if(currentExample != null)
            {
                currentExample.Dispose();
            }
            base.OnDispose();

        }
        public override void OnMouseMove(MouseMoveEventArgs args)
        {
            base.OnMouseMove(args);
            if (currentExample == null) return;
            currentExample.OnMouseMove(0,(int)args.X, (int)args.Y);
        }
        public override unsafe void MouseButtonCallback(Window* window, MouseButton button, InputAction action, KeyModifiers modes)
        {
            base.MouseButtonCallback(window, button, action, modes);
            if (currentExample == null) return;
            bool lbutton_down = false;
            bool rbutton_down = false;
            bool mbutton_down = false;

            switch(button)
            {
                case MouseButton.Left:
                    if (action == InputAction.Press) 
                        lbutton_down=true;
                    else
                        lbutton_down=false;
                    break;
                case MouseButton.Middle:
                    if (action == InputAction.Press)
                        mbutton_down = true;
                    else
                        mbutton_down = false;
                    break;
                case MouseButton.Right:
                    if (action == InputAction.Press)
                        rbutton_down = true;
                    else
                        rbutton_down = false;
                    break;
            }
            THREE.Vector2 pos = base.GetMousePosition();
            if (lbutton_down || rbutton_down || mbutton_down)
            {
                currentExample.OnMouseDown(button, (int)pos.X, (int)pos.Y);
            }
            else
            {
                currentExample.OnMouseUp(button, (int)pos.X, (int)pos.Y);
            }
           
        }
        public override void OnMouseWheel(MouseWheelEventArgs args)
        {
            base.OnMouseWheel(args);
            if(currentExample==null) return;
            THREE.Vector2 pos = base.GetMousePosition();
            currentExample.OnMouseWheel((int)pos.X, (int)pos.Y,(int)args.OffsetY * 120);
        }
    }
}
