using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;

namespace THREE.Renderers.gl
{
    public class GLMorphtargets
    {
        public Hashtable InfluencesList = new Hashtable();

        public float[] MorphInfluences = new float[8];

        public GLMorphtargets()
        {
        }

        public void Update(Object3D object3D, BufferGeometry geometry, Material material, GLProgram program)
        {
            var objectInfluences = object3D.MorphTargetInfluences;

            var length = objectInfluences.Count;

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

            GLAttribute[] morphTargets = geometry.MorphAttributes.ContainsKey("position") ? (GLAttribute[])geometry.MorphAttributes["position"] : null;
            GLAttribute[] morphNormals = geometry.MorphAttributes.ContainsKey("normal") ? (GLAttribute[])geometry.MorphAttributes["normal"] : null;

            // Remove current morphAttributes

            for (int i = 0; i < length; i++)
            {
                var influence = influences[i];

                if (influence[1] != 0)
                {
                    if (morphTargets!=null) geometry.deleteAttribute("morphTarget" + i);
                    if (morphNormals!=null) geometry.deleteAttribute("morphNormal" + i);
                }
            }

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

            // Add morphAttributes

            float morphInfluencesSum = 0;

            for (int i = 0; i < 8; i++)
            {
                var influence = influences[i];

                if (influence != null)
                {
                    var index = influence[0];
                    var value = influence[1];

                    if (value != null)
                    {
                        if (morphTargets!=null) geometry.SetAttribute("morphTarget" + i,morphTargets[(int)index]);
                        if (morphNormals != null) geometry.SetAttribute("morphNormal" + i, morphNormals[(int)index]);

                        this.MorphInfluences[i] = value;
                        morphInfluencesSum += value;
                        continue;
                    }
                }
                this.MorphInfluences[i] = 0;
            }

            var MorphBaseInfluence = geometry.MorphTargetsRelative ? 1 : System.Math.Max(0, 1 - morphInfluencesSum);

            program.GetUniforms().SetValue("morphTargetBaseInfluence", MorphBaseInfluence);
            program.GetUniforms().SetValue("morphTargetInfluences", this.MorphInfluences);
        }
       
    }
}
