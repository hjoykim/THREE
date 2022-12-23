using OpenTK.Graphics.ES30;
using System.Collections;


namespace THREE
{
    public class GLObjects
    {
        public Hashtable UpdateList = new Hashtable();

        private GLInfo info;

        public GLAttributes Attributes;

        public GLGeometries Geometries;

        public GLObjects(GLGeometries geometries, GLAttributes attributes, GLInfo info)
        {
            this.Geometries = geometries;
            this.info = info;
        }

        public BufferGeometry Update(Object3D object3D)
        {
            var frame = this.info.render.Frame;

            var geometry = object3D.Geometry;

            var bufferGeometry = Geometries.Get(object3D, geometry);

            // Update once per frame

            if (!UpdateList.ContainsKey(bufferGeometry.Id) || (int)UpdateList[bufferGeometry.Id] != frame)
            {
                if (!(geometry is BufferGeometry))
                {
                    (bufferGeometry as BufferGeometry).UpdateFromObject(object3D);
                }

                Geometries.Update(bufferGeometry);

                if (!UpdateList.ContainsKey(bufferGeometry.Id))
                    UpdateList.Add(bufferGeometry.Id, frame);
                else
                    UpdateList[bufferGeometry.Id] = frame;
            }

            //bool objectExists = UpdateList.ContainsKey(bufferGeometry.Id) ? true : false;

            //if (!objectExists)
            //{
            //    if (geometry is BufferGeometry)
            //    {
            //        (bufferGeometry as BufferGeometry).UpdateFromObject(object3D);
            //    }

            //    Geometries.Update(bufferGeometry);

            //    UpdateList.Add(bufferGeometry.Id, frame);
            //}

            //if ((int)UpdateList[bufferGeometry.Id] != frame)
            //{
            //    if (geometry is BufferGeometry)
            //    {
            //        (bufferGeometry as BufferGeometry).UpdateFromObject(object3D);
            //    }

            //    Geometries.Update(bufferGeometry);

            //    UpdateList[bufferGeometry.Id] = frame;
            //}

            if (object3D is InstancedMesh)
            {
                Attributes.Update<float>((object3D as InstancedMesh).InstanceMatrix, BufferTarget.ArrayBuffer);
            }

            return bufferGeometry as BufferGeometry;
        }
    }
}
