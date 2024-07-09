﻿/**
 * This WinForms project  and Example templates were created by referring to Three.cs( (https://github.com/lathoub/three.cs).  
 * */
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.WinForms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using THREE;

namespace THREEExample
{
    [Serializable]
    abstract public class Example
    {
        public GLRenderer renderer;

        protected readonly Random random = new Random();


        protected readonly Stopwatch stopWatch = new Stopwatch();

        public GLControl glControl;
        public virtual void Load(GLControl control)
        {
            Debug.Assert(null != control);

            glControl = control;
            this.renderer = new THREE.GLRenderer();

            this.renderer.Context = control.Context;
            this.renderer.Width = control.Width;
            this.renderer.Height = control.Height;

            this.renderer.Init();

            stopWatch.Start();
        }

        public virtual void Resize(ResizeEventArgs clientSize)
        {
            if (renderer != null)
                renderer.Resize(clientSize.Width, clientSize.Height);
        }

        public virtual void MouseUp(MouseButtonEventArgs clientSize, Point point)
        {

        }

        public virtual void MouseWheel(MouseButtonEventArgs clientSize, Point point, int delta)
        {

        }

        public virtual void MouseMove(MouseButtonEventArgs clientSize, double posX,double posY)
        {

        }

        public virtual void MouseDown(MouseButtonEventArgs clientSize, double posX,double posY)
        {

        }

        public abstract void Render();

        public virtual void Unload()
        {
            this.renderer.Dispose();
        }

        public virtual void KeyDown(Keys keyCode)
        {
            
        }
        public virtual void KeyUp(Keys keyCode)
        {

        }
    }
}