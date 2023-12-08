using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace THREE
{
   
   
    public class MTLLoader
    {
        public struct MaterialCreatorOptions
        {
            public int? Side;

            public int? Wrap;

            public bool? NormalizeRGB;

            public bool? ignoreZeroRGBs;

            public bool? invertTrproperty;
        }

        public struct MaterialInfo
        {
            public List<int> Ks;

            public List<int> Kd;

            public List<int> Ke;

            public string Map_kd;

            public string Map_ks;

            public string Map_ke;

            public string Norm;

            public string Map_Bump;

            public string Bump;

            public string Map_d;

            public int? Ns;

            public int? D;

            public int? Tr;
        }

        public struct TexParams
        {
            public Vector2 Scale;

            public Vector2 Offset;

            public string Url;
        }

        public MaterialCreatorOptions MaterialOptions;

        public string CrossOrigin;

        public MaterialCreator MultiMaterialCreator = new MaterialCreator();

        public MTLLoader()
        {

        }

        public MTLLoader.MaterialCreator Load(string filepath)
        {
            var creator = Parse(filepath);

            creator.Preload();

            if(creator.Materials!=null && creator.Materials.Count>0)
            {
                foreach(string key in creator.Materials.Keys)
                {
                    if(!MultiMaterialCreator.Materials.ContainsKey(key))
                    {
                        MultiMaterialCreator.Materials[key] = creator.Materials[key];
                    }
                }
            }

            return creator;

        }

        public MTLLoader.MaterialCreator Parse(string filepath)
        {           

            var textAll = File.ReadAllText(filepath);

            var lines = textAll.Split('\n');

            Hashtable info=null;

            var delimiter_pattern = @"\s+";

            var materialsInfo = new Hashtable();

            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (line.Length == 0 || line[0] == '#') continue;

                var pos = line.IndexOf(' ');

                var key = (pos >= 0) ? line.Substring(0, pos) : line;

                key = key.ToLower();

                var value = (pos >= 0) ? line.Substring(pos + 1) : "";

                if(key=="newmtl")
                {
                    info = new Hashtable();
                    info.Add("name", value);
                    materialsInfo.Add(value, info);
                }
                else
                {
                    if(key=="ka" || key=="kd" || key=="ks" || key=="ke")
                    {
                        //value = value.Substring(3).Trim();
                        value = (pos >= 0) ? line.Substring(pos - 1) : "";
                        value = value.Substring(pos).Trim();
                        var ss = Regex.Split(value, delimiter_pattern);
                        info.Add(key, new float[] { float.Parse(ss[0]), float.Parse(ss[1]), float.Parse(ss[2]) });
                    }
                    else
                    {
                        info.Add(key, value);
                    }
                }
            }

            var materialCreator = new MTLLoader.MaterialCreator(filepath, this.MaterialOptions);

            materialCreator.SetCrossOrigin(this.CrossOrigin);
            materialCreator.SetMaterials(materialsInfo);

            return materialCreator;
        }
        
        public void SetMaterialOptions(MaterialCreatorOptions value)
        {
            this.MaterialOptions = value;
        }

        public class MaterialCreator
        {
            public string FilePath;

            public MaterialCreatorOptions? Options;

            public Hashtable MaterialsInfo = new Hashtable();

            public Hashtable Materials = new Hashtable();

            public List<Material> MaterialsArray = new List<Material>();

            public Hashtable NameLookup = new Hashtable();

            public int Side;

            public int Wrap;

            public string CrossOrigin = "anonymous";

            public MaterialCreator()
            {

            }
            public MaterialCreator(string path,MaterialCreatorOptions? options=null)
            {
                FilePath = System.IO.Path.GetDirectoryName(path);

                this.Options = options;

                this.Side = (options != null && options.Value.Side != null) ? this.Options.Value.Side.Value : Constants.FrontSide;

                this.Wrap = (options != null && options.Value.Wrap != null) ? this.Options.Value.Wrap.Value : Constants.RepeatWrapping;
            }

            public MaterialCreator SetCrossOrigin(string value)
            {
                this.CrossOrigin = value;

                return this;
            }
            public void SetMaterials(Hashtable materialsInfo)
            {
                this.MaterialsInfo = this.Convert(materialsInfo);

            }

            public Hashtable Convert(Hashtable materialsInfo)
            {
                if (this.Options == null) return materialsInfo;

                Hashtable Converted = new Hashtable();

                foreach(string mn in materialsInfo.Keys)
                {
                    var mat = materialsInfo[mn] as Hashtable;

                    var covmat = new Hashtable();

                    Converted[mn] = covmat;

                    foreach(string prop in mat.Keys)
                    {
                        var save = true;
                        var value = mat[prop] as float[];
                        var lprop = prop.ToLower();

                        switch(lprop)
                        {
                            case "kd":
                            case "ka":
                            case "ks":
                                if(this.Options!=null && this.Options.Value.NormalizeRGB!=null && this.Options.Value.NormalizeRGB.Value)
                                {
                                    value = new float[3] { value[0] / 255.0f, value[1] / 255.0f, value[2] / 255.0f };
                                }

                                if(this.Options!=null && this.Options.Value.ignoreZeroRGBs!=null && this.Options.Value.ignoreZeroRGBs.Value)
                                {
                                    if(value[0]==0 && value[1]==0 && value[2] == 0)
                                    {
                                        save = false;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        if (save)
                        {
                            covmat[lprop] = mat[prop];
                        }
                    }

                }

                return Converted;
            }

            public void Preload()
            {
                foreach(string mn in this.MaterialsInfo.Keys)
                {
                    this.Create(mn);
                }
            }

            public int GetIndex(string materialName)
            {
                return (int)this.NameLookup[materialName];
            }
            public List<Material> GetAsArray()
            {
                var index = 0;

                MaterialsArray.Clear();

                foreach(string mn in this.MaterialsInfo.Keys)
                {
                    this.MaterialsArray.Add(this.Create(mn));
                    this.NameLookup[mn] = index;
                    index++;
                }
                return this.MaterialsArray;
            }
            public Material Create(string materialName)
            {
                if (!this.Materials.ContainsKey(materialName))
                {
                    this.CreateMaterial(materialName);
                }

                return this.Materials[materialName] as Material;
            }

            public Material CreateMaterial(string materialName)
            {
                var mat = this.MaterialsInfo[materialName] as Hashtable;

                Hashtable parameter = new Hashtable()
                    {
                        { "name",materialName },
                        {"side",this.Side }
                    };

                foreach(string prop in mat.Keys)
                {
                    var value = mat[prop];
                    float n;
                    if (value is string && value.ToString() == "") continue;

                    if (value == null) continue;
                    switch (prop.ToLower())
                    {
                        case "kd":
                            var colorArray = (float[])value;
                            parameter["color"] = new Color().FromArray((float[])value);
                            break;
                        case "ks":
                            parameter["specular"] = new Color().FromArray((float[])value);
                            break;
                        case "ke":
                            parameter["emissive"] = new Color().FromArray((float[])value);
                            break;
                        case "map_kd":
                            SetMapForType(parameter,"map", value);
                            break;
                        case "map_ks":
                            SetMapForType(parameter, "specularMap", value);
                            break;
                        case "map_ke":
                            SetMapForType(parameter, "emissiveMap", value);
                            break;
                        case "norm":
                            SetMapForType(parameter, "normalMap", value);
                            break;
                        case "map_bump":
                        case"bump":
                            SetMapForType(parameter, "bumpMap", value);
                            break;
                        case "map_d":
                            SetMapForType(parameter, "alphaMap", value);
                            parameter["transparent"] = true;
                            break;
                        case "ns":
                            parameter["shininess"] = float.Parse((string)value);
                            break;
                        case "d":
                            n = float.Parse((string)value);
                            if (n < 1)
                            {
                                parameter["opacity"] = n;
                                parameter["transparent"] = true;                                
                            }
                            break;
                        case "tr":
                            n = float.Parse((string)value);

                            if (this.Options != null && this.Options.Value.invertTrproperty != null && this.Options.Value.invertTrproperty.Value) n = 1 - n;

                            if (n > 0)
                            {
                                parameter["opacity"] = 1 - n;
                                parameter["transparent"] = true;
                            }
                            break;
                        default:
                            break;
                    }                    
                }

                this.Materials[materialName] = new MeshPhongMaterial(parameter);

                return this.Materials[materialName] as Material;
            }
            private void SetMapForType(Hashtable parameter,string mapType,object value)
            {
                if (parameter.ContainsKey(mapType)) return;

                var texParams = GetTextureParams((string)value, parameter);
                var map = LoadTexture(System.IO.Path.Combine(FilePath,(string)texParams["url"]));

                map.Repeat.Copy((Vector2)texParams["scale"]);
                map.Offset.Copy((Vector2)texParams["offset"]);

                map.WrapS = this.Wrap;
                map.WrapT = this.Wrap;

                parameter[mapType] = map;
                
            }
            private Hashtable GetTextureParams(string value,Hashtable matParams)
            {
                var texParams = new Hashtable()
                {
                    {"scale",new Vector2(1,1) },
                    {"offset",new Vector2(0,0) }
                };

                string pattern = @"\s+";
                List<String> items = System.Text.RegularExpressions.Regex.Split(value, pattern).ToList();
                int pos =-1;
                for(int i = 0; i < items.Count; i++)
                {
                    if(items[i].IndexOf("-bm")>-1)
                    {
                        pos = i;
                        break;
                    }
                }

                if (pos >= 0)
                {
                    matParams["bumpScale"] = float.Parse(items[pos + 1]);
                    items.Splice(pos, 2);
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].IndexOf("-s") > -1)
                    {
                        pos = i;
                        break;
                    }
                    pos = -1;
                }
                if (pos >= 0)
                {
                    Vector2 scale = texParams["scale"] as Vector2;
                    scale.Set(float.Parse(items[pos + 1]), float.Parse(items[pos + 2]));
                    items.Splice(pos, 4);
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].IndexOf("-o") > -1)
                    {
                        pos = i;
                        break;
                    }
                    pos = -1;
                }
                if (pos >= 0)
                {
                    Vector2 offset = texParams["offset"] as Vector2;
                    offset.Set(float.Parse(items[pos + 1]), float.Parse(items[pos + 2]));
                    items.Splice(pos, 4);
                }

                texParams["url"] = String.Join(" ",items).Trim();

                return texParams;
            }

            public Texture LoadTexture(string filePath,int? mapping=null)
            {
                Texture texture = TextureLoader.Load(filePath);

                if (mapping != null) texture.Mapping = mapping.Value;

                return texture;
                
            }
        }
    }
}
