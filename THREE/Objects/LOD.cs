using System.Collections.Generic;

namespace THREE
{
    public struct LevelStruct
    {
        public float distance;
        public Object3D object3D;
    }

    public class LOD : Object3D
    {
        

        public List<LevelStruct> Levels = new List<LevelStruct>();

        public bool AutoUpdate = true;

        private Vector3 v1 = Vector3.Zero();

        private Vector3 v2 = Vector3.Zero();

        public LOD()
        {
            this.type = "LOD";
        }

        protected LOD(LOD other) : base(other)
        {
            var levels = other.Levels;

            for (int i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                this.AddLevel((Object3D)level.object3D.Clone(), level.distance);
            }
            this.AutoUpdate = other.AutoUpdate;
        }

        public LOD AddLevel(Object3D object3D, float? distance)
        {
            if (distance == null) distance = 0;

            distance = (float)System.Math.Abs((float)distance);

            var levels = this.Levels;

            int l = 0;
            for (l = 0; l < levels.Count; l++)
            {
                if (distance < levels[l].distance)
                {
                    break;
                }
            }

            LevelStruct level = new LevelStruct { distance = distance.Value, object3D=object3D };

            if (l >= Levels.Count)
                Levels.Add(level);
            else
                Levels.Insert(l,level);

            this.Add(object3D);

            return this;
        }

        public Object3D GetObjectForDistance(float distance)
        {
            int i;
            if (this.Levels.Count > 0)
            {
                for (i = 0; i < this.Levels.Count; i++)
                {
                    if (distance < this.Levels[i].distance)
                        break;
                }
                return this.Levels[i - 1].object3D;
            }
            return null;
        }

        public override void Raycast(Raycaster raycaster, List<Intersection> intersectionList)
        {
            if (this.Levels.Count > 0)
            {
                v1.SetFromMatrixPosition(this.MatrixWorld);
                var distance = raycaster.ray.origin.DistanceTo(v1);
                this.GetObjectForDistance(distance).Raycast(raycaster, intersectionList);

            }
        }

        public void Update(Camera camera)
        {
            var levels = this.Levels;

            if (levels.Count > 1)
            {
                v1.SetFromMatrixPosition(camera.MatrixWorld); 
                v2.SetFromMatrixPosition(this.MatrixWorld);

                var distance = v1.DistanceTo(v2);

                levels[0].object3D.Visible = true;

                int i,l = levels.Count;
                for (i = 1; i < l; i++)
                {
                    if (distance >= levels[i].distance)
                    {
                        levels[i - 1].object3D.Visible = false;
                        levels[i].object3D.Visible = true;
                    }
                    else
                    {
                        break;
                    }
                }

                for (; i < l; i++)
                {
                    levels[i].object3D.Visible = false;
                }
            }
        }
    }

}
