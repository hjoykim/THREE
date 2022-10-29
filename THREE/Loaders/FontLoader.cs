using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Extras.Core;

namespace THREE.Loaders
{
    public class FontLoader
    {
        public FontLoader()
        {

        }
        public static Font Load(string jsonFile)
        {
            return new Font(jsonFile);

        }
    }
}
