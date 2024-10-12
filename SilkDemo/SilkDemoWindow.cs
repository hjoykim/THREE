using ImGuiNET;
using Silk.NET.Maths;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using THREE.Silk;
using THREE.Silk.Example;
namespace SilkDemo
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
    public class SilkDemoWindow : ThreeSilkWindow
    {
        private List<TreeNode<string>> examplesList;
        private Dictionary<string, Example> exampleInstances = new Dictionary<string, Example>();
        public SilkDemoWindow() : base() { }

        private TreeNode<string> LoadExampleFromAssembly(Assembly assembly)
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
        private string MakeExampleTitle(string exampleName)
        {
            if (string.IsNullOrEmpty(exampleName))
                return "THREE.Silk.Example";
            else
                return "THREE.Silk.Example : " + exampleName;
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
                            for (int k = 0; k < titleList.Count; k++)
                            {
                                var title = titleList[k];
                                if (ImGui.Button(title.Data))
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
        private void RunSample(ExampleInfo e)
        {
            if (null != currentThreeContainer)
            {
                currentThreeContainer.Dispose();
                currentThreeContainer = null;
                Title = MakeExampleTitle("");
            }

            //Application.Idle -= Application_Idle;

            currentThreeContainer = (Example)Activator.CreateInstance(e.Example);
            if (null != currentThreeContainer)
            {
                currentThreeContainer.Load(this.window);
                currentThreeContainer.imGuiManager = imGuiManager;
                Title = MakeExampleTitle(e.Attribute.Title);
                Vector2D<int> size;
                size.X = window.Size.X;
                size.Y = window.Size.Y;
                OnResize(size);
            }
        }
        public override void OnLoad()
        {
            base.OnLoad();
            currentThreeContainer = new FirstSceneExample();
            currentThreeContainer.Load(window);
            currentThreeContainer.imGuiManager = imGuiManager;
            currentThreeContainer.OnResize(new ResizeEventArgs(window.Size.X,window.Size.Y));
            GetExamplesList();

        }
        public override void OnRender(double deltaTime)
        {
            imGuiManager.Update((float)deltaTime);
            if (currentThreeContainer == null) return;

            ShowExamplesMenu();

            currentThreeContainer.Render();
            if (currentThreeContainer.AddGuiControlsAction != null)
            {
                currentThreeContainer.AddGuiControlsAction();
            }
            imGuiManager.Render();
        }
    }


}
