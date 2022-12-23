using System.Collections;
using System.Collections.Generic;

namespace THREE
{
    public class GLMorphtargets
    {
        public Hashtable InfluencesList = new Hashtable();

        public float[] MorphInfluences = new float[8];

        public List<float[]> WorkInfluences = new List<float[]>();
        public GLMorphtargets()
        {
            for (var i = 0; i < 8; i++)
            {

                WorkInfluences.Add(new float[2] { i, 0 });

            }
        }

        public void Update(Object3D object3D, BufferGeometry geometry, Material material, GLProgram program)
        {
            var objectInfluences = object3D.MorphTargetInfluences;

            // When object doesn't have morph target influences defined, we treat it as a 0-length array
            // This is important to make sure we set up morphTargetBaseInfluence / morphTargetInfluences

            int length = objectInfluences == null ? 0 : objectInfluences.Count;

            List<float[]> influences = null;

            if (!InfluencesList.ContainsKey(geometry.Id))
            {
                influences = new List<float[]>();

                for (int i = 0; i < length; i++)
                {
                    influences.Add(new float[2] { i, 0 });
                }

                InfluencesList.Add(geometry.Id, influences);
            }
            else
            {
                influences = (List<float[]>)InfluencesList[geometry.Id];
            }

            //List<BufferAttribute<float>> morphTargets = geometry.MorphAttributes.ContainsKey("position") ? (List<BufferAttribute<float>>)geometry.MorphAttributes["position"] : null;
            //List<BufferAttribute<float>> morphNormals = geometry.MorphAttributes.ContainsKey("normal") ? (List<BufferAttribute<float>>)geometry.MorphAttributes["normal"] : null;

            //// Remove current morphAttributes

            //for (int i = 0; i < length; i++)
            //{
            //    var influence = influences[i];

            //    if (influence[1] != 0)
            //    {
            //        if (morphTargets!=null) geometry.deleteAttribute("morphTarget" + i);
            //        if (morphNormals!=null) geometry.deleteAttribute("morphNormal" + i);
            //    }
            //}

            // Collect influences

            for (int i = 0; i < length; i++)
            {
                var influence = influences[i];

                influence[0] = i;
                influence[1] = objectInfluences[i];

            }

            influences.Sort(delegate(float[] a, float[] b)
            {
                return (int)(System.Math.Abs(b[1]) - System.Math.Abs(a[1]));
            });

            for (int i = 0; i < 8; i++)
            {

                if (i < length && influences[i][1]>0)
                {

                    WorkInfluences[i][0] = influences[i][0];
                    WorkInfluences[i][1] = influences[i][1];

                }
                else
                {

                    WorkInfluences[i][0] = int.MaxValue;
                    WorkInfluences[i][1] = 0;

                }

            }

            WorkInfluences.Sort(delegate (float[] a, float[] b)
            {
                return (int)(a[0] - b[0]);
            });

            List<BufferAttribute<float>> morphTargets = material.MorphTargets && geometry.MorphAttributes.ContainsKey("position") ?geometry.MorphAttributes["position"] as List<BufferAttribute<float>> : null;
            List<BufferAttribute<float>> morphNormals = material.MorphNormals && geometry.MorphAttributes.ContainsKey("normal") ? geometry.MorphAttributes["normal"]  as List<BufferAttribute<float>> : null;
            float morphInfluencesSum = 0;

            if (influences.Count > 0)
            {

                for (int i = 0; i < 8; i++)
                {
                    var influence = WorkInfluences[i];

                    if (influence != null)
                    {
                        int index = (int)influence[0];
                        var value = influence[1];

                        if (index != int.MaxValue && value > 0)
                        {
                            if (morphTargets != null)
                            {
                                if (morphTargets[index] != null && !geometry.Attributes.ContainsKey("morphTarget" + i))
                                {

                                    geometry.SetAttribute("morphTarget" + i, morphTargets[index]);
                                }
                                else if (morphTargets[index] != null && geometry.Attributes["morphTarget" + i] != morphTargets[index])
                                {
                                    geometry.SetAttribute("morphTarget" + i, morphTargets[index]);
                                }
                            }
                            if (morphNormals != null) 
                            { 
                                if (morphNormals[index] != null && !geometry.Attributes.ContainsKey("morphNormal" + i))
                                {

                                    geometry.SetAttribute("morphNormal" + i, morphNormals[index]);
                                }
                                else if (morphNormals != null && geometry.Attributes["morphNormal" + i] != morphNormals[index])
                                {
                                    geometry.SetAttribute("morphNormal" + i, morphTargets[index]);
                                }
                            }                            
                            this.MorphInfluences[i] = value;
                            morphInfluencesSum += value;
                            continue;
                        }
                        else
                        {
                            if (morphTargets != null && geometry.Attributes.ContainsKey("morphTarget" + i) == true)
                            {

                                geometry.deleteAttribute("morphTarget" + i);

                            }

                            if (morphNormals != null && geometry.Attributes.ContainsKey("morphNormal" + i) == true)
                            {

                                geometry.deleteAttribute("morphNormal" + i);

                            }
                            this.MorphInfluences[i] = 0;
                        }
                    }

                }
            }

            // GLSL shader uses formula baseinfluence * base + sum(target * influence)
            // This allows us to switch between absolute morphs and relative morphs without changing shader code
            // When baseinfluence = 1 - sum(influence), the above is equivalent to sum((target - base) * influence)
            var MorphBaseInfluence = geometry.MorphTargetsRelative ? 1 : 1 - morphInfluencesSum;

            program.GetUniforms().SetValue("morphTargetBaseInfluence", MorphBaseInfluence);
            program.GetUniforms().SetValue("morphTargetInfluences", this.MorphInfluences);
        }
       
    }
}
