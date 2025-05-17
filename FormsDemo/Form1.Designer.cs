
using OpenTK.WinForms;

namespace FormsDemo
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            treeView1 = new TreeView();
            statusStrip1 = new StatusStrip();
            var settings = new GLControlSettings
            {
                NumberOfSamples = 8, // MSAA x8
                DepthBits = 24,
                StencilBits = 8
            };
            glControl = new GLControl();
            statusStrip2 = new StatusStrip();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Size = new Size(93, 22);
            exitMenuItem.Text = "&Exit";
            exitMenuItem.Click += exitMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutMenuItem
            // 
            aboutMenuItem.Name = "aboutMenuItem";
            aboutMenuItem.Size = new Size(107, 22);
            aboutMenuItem.Text = "&About";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Margin = new Padding(3, 4, 3, 4);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeView1);
            splitContainer1.Panel1.Controls.Add(statusStrip1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(glControl);
            splitContainer1.Panel2.Controls.Add(statusStrip2);
            splitContainer1.Size = new Size(800, 538);
            splitContainer1.SplitterDistance = 150;
            splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(0, 0);
            treeView1.Margin = new Padding(3, 4, 3, 4);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(150, 516);
            treeView1.TabIndex = 1;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 516);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(150, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // glControl
            // 
            glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl.APIVersion = new Version(3, 3, 0, 0);
            glControl.BackColor = Color.Black;
            glControl.Dock = DockStyle.Fill;
            glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl.IsEventDriven = true;
            glControl.Location = new Point(0, 0);
            glControl.Margin = new Padding(3, 4, 3, 4);
            glControl.Name = "glControl";
            glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            glControl.Size = new Size(646, 516);
            glControl.TabIndex = 1;
            glControl.Load += glControl_Load;
            glControl.Paint += glControl_Paint;
            glControl.KeyDown += glControl_KeyDown;
            glControl.KeyPress += glControl_KeyPress;
            glControl.KeyUp += glControl_KeyUp;
            glControl.MouseDown += glControl_MouseDown;
            glControl.MouseEnter += glControl_MouseEnter;
            glControl.MouseLeave += glControl_MouseLeave;
            glControl.MouseHover += glControl_MouseHover;
            glControl.MouseMove += glControl_MouseMove;
            glControl.MouseUp += glControl_MouseUp;
            glControl.PreviewKeyDown += glControl_PreviewKeyDown;
            glControl.Resize += glControl_Resize;
            // 
            // statusStrip2
            // 
            statusStrip2.Location = new Point(0, 516);
            statusStrip2.Name = "statusStrip2";
            statusStrip2.Size = new Size(646, 22);
            statusStrip2.TabIndex = 0;
            statusStrip2.Text = "statusStrip2";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 562);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private GLControl glControl;
        private System.Windows.Forms.StatusStrip statusStrip2;
    }
}

