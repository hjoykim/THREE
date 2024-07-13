using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.OpenGL;

namespace THREE
{
    public struct KeyPressEventArgs
    {
        public string Key { get; set; }
        public KeyPressEventArgs(string key) { this.Key = key; }
    }
}
