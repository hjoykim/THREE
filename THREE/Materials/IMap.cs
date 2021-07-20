using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Textures;

namespace THREE.Materials
{
    public interface IMap
    {
        Texture Map{get;set;}

        Texture SpecularMap{get;set;}

        Texture AlphaMap{get;set;}

        Texture EnvMap{get;set;}
        
        Texture NormalMap { get; set; }

        int NormalMapType { get; set; }

        Vector2 NormalScale { get; set; }

        Texture BumpMap { get; set; }

        float BumpScale { get; set; }

        Texture LightMap { get; set; }

        Texture AoMap { get; set; }

        Texture EmissiveMap { get; set; }

        Texture DisplacementMap { get; set; }

        float DisplacementScale { get; set; }

        int DisplacementBias { get; set; }

        Texture ClearCoatNormalMap { get; set; }

        Texture RoughnessMap { get; set; }

        Texture MetaInessMap { get; set; }

        Texture GradientMap { get; set; }

        Color? Sheen { get; set; }

        int Combine { get; set; }

    }
}
