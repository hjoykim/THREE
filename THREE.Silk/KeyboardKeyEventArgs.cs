using Silk.NET.GLFW;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Silk
{
    [Serializable]
    public readonly struct KeyboardKeyEventArgs
    {
        public Key Key { get; }
        public int ScanCode { get; }
        public KeyModifiers Modifiers { get; }
        public bool IsRepeat { get; }
        public bool Alt => Modifiers.HasFlag(KeyModifiers.Alt);
        public bool Control =>Modifiers.HasFlag(KeyModifiers.Control);
        public bool Shift => Modifiers.HasFlag(KeyModifiers.Shift);
        public bool Command => Modifiers.HasFlag(KeyModifiers.Super);
        public KeyboardKeyEventArgs(Key key,int scancode,KeyModifiers modifiers,bool isRepeat)
        {
            Key = key;
            ScanCode = scancode;
            Modifiers = modifiers;
            IsRepeat = isRepeat;
        }
    }
}
