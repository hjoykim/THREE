/**
 * This WinForms project  and Example templates were created by referring to Three.cs( (https://github.com/lathoub/three.cs).  
 * */
using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using THREEExample;

namespace FormsDemo
{
    public partial class Form1 : Form
    {
        private Example example;
        private int SleepTime = 5;
        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
        }
        void Application_Idle(object sender, EventArgs e)
        {
            //while (glControl.IsIdle)
            {
                Render();

                Thread.Sleep(SleepTime);
            }
        }
        private void Render()
        {
            this.glControl.MakeCurrent();
            if (null != example)
                example.Render();

            //stats.update();

            this.glControl.SwapBuffers();
        }
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        private void LoadExampleFromAssembly(Assembly assembly)
        {
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

                        TreeNode rootItem = null;
                        foreach (var item in treeView1.Nodes)
                        {
                            var header = example.Category.ToString() + " " + string.Format("{0}", example.Subcategory);
                            if ((item as TreeNode).Text.Equals(header))
                            {
                                rootItem = item as TreeNode;
                                break;
                            }
                        }


                        if (rootItem == null)
                        {
                            rootItem = new TreeNode();
                            rootItem.Text = example.Category.ToString() + " " + string.Format("{0}", example.Subcategory);
                            treeView1.Nodes.Add(rootItem);

                        }


                        var treeItem = new TreeNode();
                        treeItem.Text = example.Title;



                        treeItem.Tag = new ExampleInfo(type, example);

                        rootItem.Nodes.Add(treeItem);
                    }
                }
            }
            treeView1.Sort();
            //treeView1.ExpandAll();
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                ActivateNode(e.Node);
            }
        }
        void ActivateNode(TreeNode node)
        {
            if (node == null)
                return;

            if (node.Tag == null)
            {
                if (node.IsExpanded)
                    node.Collapse();
                else
                    node.Expand();
            }
            else
            {
                RunSample((ExampleInfo)node.Tag);
            }
        }
        void RunSample(ExampleInfo e)
        {
            if (null != example)
            {
                example.Unload();
                
                example = null;

                statusStrip1.Tag = statusStrip1.Text = string.Empty;                
               
            }

            Application.Idle -= Application_Idle;

            example = (Example)Activator.CreateInstance(e.Example);
            if (null != example)
            {
                this.glControl.MakeCurrent();

                example.Load(this.glControl);

                statusStrip1.Text = e.Example.Name.Replace("_", " - ");
                statusStrip1.Tag = e.Example.Name;


                GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
                example.Resize(glControl.ClientSize);

                Application.Idle += Application_Idle;
            }
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            Type t = typeof(Example);
            LoadExampleFromAssembly(Assembly.GetAssembly(t));

            Text =
                GL.GetString(StringName.Vendor) + " " +
                GL.GetString(StringName.Renderer) + " " +
                GL.GetString(StringName.Version);
#if NET6_0_OR_GREATER
            this.glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
#endif
            statusStrip1.Text = string.Empty;
        }

        private void glControl_Resize(object sender, EventArgs e)
        {

            var control = sender as Control;

            if (control.ClientSize.Height == 0)
                control.ClientSize = new Size(control.ClientSize.Width, 1);

            GL.Viewport(0, 0, control.ClientSize.Width, control.ClientSize.Height);

            if (example != null)
                example.Resize(control.ClientSize);
           
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            this.Render();
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (example != null)
                example.KeyDown(e.KeyCode);
        }
    }
}
