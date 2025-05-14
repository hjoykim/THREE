using Silk.NET.OpenGLES;
using System;
using System.Collections;
using System.Collections.Generic;

using Geometry = THREE.Geometry;

namespace THREE
{
    [Serializable]
    public struct BindingStateStruct
    {
        public Guid uuid;
        public int? geometry;
        public int? program;
        public bool wireframe;
        public List<int> newAttributes;
        public List<int> enabledAttributes;
        public List<int> attributeDivisors;
        public int? vao;
        public Hashtable attributes;
        public BufferAttribute<int> index;

        public bool Equals(BindingStateStruct other)
        {
            return uuid.Equals(other.uuid);
        }
    }

    [Serializable]
    public class GLBindingStates : IDisposable
    {
        public event EventHandler<EventArgs> Disposed;

        private int maxVertexAttributes;

        private bool isGL2;

        private Hashtable bindingStates = new Hashtable();

        public BindingStateStruct defaultState;

        public BindingStateStruct currentState;

        private GLExtensions extensions;

        private GLCapabilities capabilities;

        private GLAttributes attributes;

        private bool vaoAvailable;

        private GL gl;

        public GLBindingStates(GL gl, GLExtensions extensions, GLAttributes attributes, GLCapabilities capabilities)
        {
            this.gl = gl;
            this.extensions = extensions;
            this.attributes = attributes;
            this.capabilities = capabilities;

           gl.GetInteger(GetPName.MaxVertexAttribs, out maxVertexAttributes);

            vaoAvailable = capabilities.IsGL2;

            defaultState = createBindingState(null);
            currentState = defaultState;

        }
        ~GLBindingStates()
        {
     
            Reset();
            foreach (int geometryId in bindingStates.Keys)
            {
                Hashtable programMap = bindingStates[geometryId] as Hashtable;
                foreach (int programId in programMap.Keys)
                {
                    Hashtable stateMap = programMap[programId] as Hashtable;
                    foreach (bool wireframe in stateMap.Keys)
                    {
                        BindingStateStruct bindingState = (BindingStateStruct)stateMap[wireframe];
                        deleteVertexArrayObject(bindingState.vao.Value);

                        stateMap.Remove(wireframe);
                    }
                    programMap.Remove(programId);
                }
                bindingStates.Remove(geometryId);

            }
            this.Dispose(false);
        }
        private int createVertexArrayObject()
        {
            return (int)gl.GenVertexArray();
        }
        private void bindVertexArrayObject(int vao)
        {
            gl.BindVertexArray((uint)vao);

        }
        private void deleteVertexArrayObject(int vao)
        {
            gl.DeleteVertexArray((uint)vao);
        }

        private BindingStateStruct getBindingState(Geometry geometry, GLProgram program, Material material)
        {
            bool wireframe = material.Wireframe;

            Hashtable programMap;

            if (!bindingStates.ContainsKey(geometry.Id))
            {
                programMap = new Hashtable();
                bindingStates.Add(geometry.Id, new Hashtable());
            }

            programMap = bindingStates[geometry.Id] as Hashtable;

            Hashtable stateMap;

            if (!programMap.ContainsKey(program.Id))
            {
                stateMap = new Hashtable();
                programMap.Add(program.Id, stateMap);
            }
            stateMap = programMap[program.Id] as Hashtable;

            BindingStateStruct? state = stateMap[wireframe] as BindingStateStruct?;

            if (state == null)
            {
                state = createBindingState(createVertexArrayObject());
                stateMap[wireframe] = state;
            }

            return state.Value;

        }

        private BindingStateStruct createBindingState(int? vao)
        {
            List<int> newAttributes = new List<int>();
            List<int> enabledAttributes = new List<int>();
            List<int> attributeDivisors = new List<int>();

            for (int i = 0; i < maxVertexAttributes; i++)
            {
                newAttributes.Add(0);
                enabledAttributes.Add(0);
                attributeDivisors.Add(0);
            }

            return new BindingStateStruct() { uuid = Guid.NewGuid(), newAttributes = newAttributes, enabledAttributes = enabledAttributes, attributeDivisors = attributeDivisors, vao = vao, attributes = new Hashtable(), index = null };
        }

        private bool needsUpdate(Geometry geometry, BufferAttribute<int> index)
        {
            var cachedAttributes = currentState.attributes;
            var geometryAttributes = (geometry as BufferGeometry).Attributes;

            if (cachedAttributes.Count != geometryAttributes.Count) return true;

            foreach (var items in geometryAttributes)
            {

                var cachedAttribute = cachedAttributes[items.Key];
                var geometryAttribute = geometryAttributes[items.Key];

                if (cachedAttribute == null) return true;


                if ((cachedAttribute as Hashtable)["attribute"] != geometryAttribute as BufferAttribute<float>) return true;

                if (geometryAttribute is InterleavedBufferAttribute<float>)
                {
                    if ((cachedAttribute as Hashtable)["data"] != (geometryAttribute as InterleavedBufferAttribute<float>).Data) return true;
                }

            }

            if (currentState.index != index) return true;

            return false;
        }

        private void saveCache(Geometry geometry, BufferAttribute<int> index, GLProgram program, Material material)
        {
            Hashtable cache = new Hashtable();

            foreach (var items in (geometry as BufferGeometry).Attributes)
            {
                var attribute = items.Value as BufferAttribute<float>;
                Hashtable data = new Hashtable();
                data.Add("attribute", attribute);
                if (attribute is InterleavedBufferAttribute<float>)
                {
                    data.Add("data", (attribute as InterleavedBufferAttribute<float>).Data);
                }
                cache.Add(items.Key, data);
            }

            currentState.attributes = cache;

            currentState.index = index;

            Hashtable programMap = bindingStates[geometry.Id] as Hashtable;
            Hashtable stateMap = programMap[program.Id] as Hashtable;
            bool wireframe = material.Wireframe;
            stateMap[wireframe] = currentState;
        }
        public void InitAttributes()
        {
            var newAttributes = currentState.newAttributes;

            for (int i = 0; i < newAttributes.Count; i++)
            {
                newAttributes[i] = 0;
            }
        }
        public void Setup(Object3D object3D, Material material, GLProgram program, Geometry geometry, BufferAttribute<int> index)
        {
            bool updateBuffers = false;

            if (vaoAvailable)
            {
                BindingStateStruct state = getBindingState(geometry, program, material);

                if (!currentState.Equals(state))
                {
                    currentState = state;
                    bindVertexArrayObject(currentState.vao.Value);
                }

                updateBuffers = needsUpdate(geometry, index);

                if (updateBuffers) saveCache(geometry, index, program, material);
            }
            else
            {

                bool wireframe = (material.Wireframe == true);

                if (currentState.geometry != geometry.Id ||
                    currentState.program != program.Id ||
                    currentState.wireframe != wireframe)
                {

                    currentState.geometry = geometry.Id;
                    currentState.program = program.Id;
                    currentState.wireframe = wireframe;

                    updateBuffers = true;

                }

            }

            if (object3D is InstancedMesh)
            {

                updateBuffers = true;

            }

            if (index != null)
            {

                attributes.Update(index, GLEnum.ElementArrayBuffer);

            }

            if (updateBuffers)
            {

                setupVertexAttributes(object3D, material, program, geometry);

                if (index != null)
                {

                    gl.BindBuffer(GLEnum.ElementArrayBuffer, (uint)attributes.Get<int>(index).buffer);

                }

            }

        }

        public void Reset()
        {
            ResetDefaultState();

            if (currentState.Equals(defaultState)) return;


            currentState = defaultState;

            if (currentState.vao == null)
            {
                bindVertexArrayObject(0);
                return;
            }
            if (!currentState.vao.HasValue)
                bindVertexArrayObject(0);
            else
                bindVertexArrayObject(currentState.vao.Value);
        }
        public void ResetDefaultState()
        {
            defaultState.geometry = null;
            defaultState.program = null;
            defaultState.wireframe = false;
        }
        public void ReleaseStatesOfGeometry(Geometry geometry)
        {

            if (this.bindingStates[geometry.Id] == null) return;

            Hashtable programMap = this.bindingStates[geometry.Id] as Hashtable;

            foreach (int programId in programMap.Keys)
            {

                Hashtable stateMap = (Hashtable)programMap[programId];

                foreach (bool wireframe in stateMap.Keys)
                {

                    BindingStateStruct binding = (BindingStateStruct)stateMap[wireframe];
                    deleteVertexArrayObject(binding.vao.Value);

                    stateMap.Remove(wireframe);

                }
                programMap.Remove(programId);
            }
            this.bindingStates.Remove(geometry.Id);
        }
        public void ReleaseStatesOfProgram(GLProgram program)
        {
            foreach (int geometryId in bindingStates.Keys)
            {

                var programMap = (Hashtable)bindingStates[geometryId];

                if (programMap[program.Id] == null) continue;

                Hashtable stateMap = (Hashtable)programMap[program.Id];

                foreach (bool wireframe in stateMap.Keys)
                {
                    BindingStateStruct binding = (BindingStateStruct)stateMap[wireframe];
                    deleteVertexArrayObject(binding.vao.Value);

                    stateMap.Remove(wireframe);

                }

                programMap.Remove(program.Id);

            }
        }

        public void EnableAttribute(int attribute)
        {
            enableAttributeAndDivisor(attribute, 0);
        }
        private void enableAttributeAndDivisor(int attribute, int meshPerAttribute)
        {
            var newAttributes = currentState.newAttributes;
            var enabledAttributes = currentState.enabledAttributes;
            var attributeDivisors = currentState.attributeDivisors;

            newAttributes[attribute] = 1;

            if (enabledAttributes[attribute] == 0)
            {

                gl.EnableVertexAttribArray((uint)attribute);
                enabledAttributes[attribute] = 1;

            }

            if (attributeDivisors[attribute] != meshPerAttribute)
            {

                //const extension = capabilities.isWebGL2 ? gl : extensions.get('ANGLE_instanced_arrays');
                //extension[capabilities.isWebGL2 ? 'vertexAttribDivisor' : 'vertexAttribDivisorANGLE'](attribute, meshPerAttribute);

                gl.VertexAttribDivisor((uint)attribute, (uint)meshPerAttribute);
                attributeDivisors[attribute] = meshPerAttribute;

            }
        }
        public void DisableUnusedAttributes()
        {
            var newAttributes = currentState.newAttributes;
            var enabledAttributes = currentState.enabledAttributes;

            for (int i = 0; i < enabledAttributes.Count; i++)
            {

                if (enabledAttributes[i] != newAttributes[i])
                {

                    gl.DisableVertexAttribArray((uint)i);
                    enabledAttributes[i] = 0;

                }

            }
        }
        private void vertexAttribPointer(int index, int size, GLEnum type, bool normalized, int stride, int offset)
        {
            if (isGL2 && (type == GLEnum.Int || type == GLEnum.UnsignedInt))
                gl.VertexAttribIPointer((uint)index, size, type, (uint) stride, IntPtr.Zero);
            else
                gl.VertexAttribPointer((uint)index, size, type, normalized, (uint)stride, offset);
        }

        private void setupVertexAttributes(Object3D object3D, Material material, GLProgram program, Geometry geometry)
        {
            //if (isGL2 == false && (object3D is InstancedMesh || geometry is InstancedBufferGeometry))
            //{
            //    if (extensions.Get("GL_ARB_instanced_arrays") == -1) return;
            //}

            InitAttributes();

            var geometryAttributes = (geometry as BufferGeometry).Attributes;

            var programAttributes = program.GetAttributes();

            Hashtable materialDefaultAttributeValues = null;
            if (material is ShaderMaterial)
            {
                materialDefaultAttributeValues = (material as ShaderMaterial).DefaultAttributeValues;
            }

            foreach (string name in programAttributes.Keys)
            {

                int programAttribute = (int)programAttributes[name];

                if (programAttribute >= 0)
                {

                    object geometryAttribute = null;
                    //const geometryAttribute = geometryAttributes[name];

                    if (geometryAttributes.TryGetValue(name, out geometryAttribute))
                    {
                        if (geometryAttribute != null)
                        {
                            var normalized = false;
                            var size = 0;
                            BufferType attribute = null;
                            if (geometryAttribute is BufferAttribute<float>)
                            {
                                normalized = (geometryAttribute as BufferAttribute<float>).Normalized;
                                size = (geometryAttribute as BufferAttribute<float>).ItemSize;
                                attribute = attributes.Get<float>(geometryAttribute);
                            }
                            if (geometryAttribute is BufferAttribute<int>)
                            {
                                normalized = (geometryAttribute as BufferAttribute<int>).Normalized;
                                size = (geometryAttribute as BufferAttribute<int>).ItemSize;
                                attribute = attributes.Get<int>(geometryAttribute);
                            }
                            if (geometryAttribute is BufferAttribute<uint>)
                            {
                                normalized = (geometryAttribute as BufferAttribute<uint>).Normalized;
                                size = (geometryAttribute as BufferAttribute<uint>).ItemSize;
                                attribute = attributes.Get<uint>(geometryAttribute);
                            }
                            if (geometryAttribute is BufferAttribute<byte>)
                            {
                                normalized = (geometryAttribute as BufferAttribute<byte>).Normalized;
                                size = (geometryAttribute as BufferAttribute<byte>).ItemSize;
                                attribute = attributes.Get<byte>(geometryAttribute);
                            }
                            if (geometryAttribute is BufferAttribute<ushort>)
                            {
                                normalized = (geometryAttribute as BufferAttribute<ushort>).Normalized;
                                size = (geometryAttribute as BufferAttribute<ushort>).ItemSize;
                                attribute = attributes.Get<ushort>(geometryAttribute);
                            }
                            // TODO Attribute may not be available on context restore

                            if (attribute == null) continue;

                            var buffer = attribute.buffer;
                            var type = (GLEnum)Enum.ToObject(typeof(GLEnum), attribute.Type);
                            var bytesPerElement = attribute.BytesPerElement;

                            if (geometryAttribute is InterleavedBufferAttribute<float>)
                            {

                                var data = (geometryAttribute as InterleavedBufferAttribute<float>).Data;
                                var stride = data.Stride;
                                var offset = (geometryAttribute as InterleavedBufferAttribute<float>).Offset;

                                if (data != null && data is InstancedInterleavedBuffer<float>)
                                {

                                    enableAttributeAndDivisor(programAttribute, (data as InstancedInterleavedBuffer<float>).MeshPerAttribute);

                                    if ((geometry as InstancedBufferGeometry).MaxInstanceCount == null)
                                    {


                                        (geometry as InstancedBufferGeometry).MaxInstanceCount = (data as InstancedInterleavedBuffer<float>).MeshPerAttribute * (data as InstancedInterleavedBuffer<float>).count;

                                    }
                                }
                                else
                                {
                                    EnableAttribute(programAttribute);
                                }

                                gl.BindBuffer(GLEnum.ArrayBuffer, (uint)buffer);
                                vertexAttribPointer(programAttribute, size, type, normalized, stride * bytesPerElement, offset * bytesPerElement);

                            }
                            else
                            {
                                if (geometryAttribute is InstancedBufferAttribute<float>)
                                {
                                    enableAttributeAndDivisor(programAttribute, (geometryAttribute as InstancedBufferAttribute<float>).MeshPerAttribute);

                                    if ((geometry as InstancedBufferGeometry).MaxInstanceCount == null)
                                    {

                                        (geometry as InstancedBufferGeometry).MaxInstanceCount = (geometryAttribute as InstancedBufferAttribute<float>).MeshPerAttribute * (geometryAttribute as InstancedBufferAttribute<float>).count;

                                    }
                                }
                                else
                                {
                                    EnableAttribute(programAttribute);

                                }

                                gl.BindBuffer(GLEnum.ArrayBuffer, (uint)buffer);
                                vertexAttribPointer(programAttribute, size, type, normalized, 0, 0);

                            }
                        }

                    }
                    else if (name.Equals("instanceMatrix"))
                    {

                        BufferType attribute = attributes.Get<float>((object3D as InstancedMesh).InstanceMatrix);

                        // TODO Attribute may not be available on context restore

                        if (attribute == null) continue;

                        var buffer = attribute.buffer;
                        var type = (VertexAttribPointerType)Enum.ToObject(typeof(VertexAttribPointerType), attribute.Type);

                        enableAttributeAndDivisor(programAttribute + 0, 1);
                        enableAttributeAndDivisor(programAttribute + 1, 1);
                        enableAttributeAndDivisor(programAttribute + 2, 1);
                        enableAttributeAndDivisor(programAttribute + 3, 1);

                        gl.BindBuffer(GLEnum.ArrayBuffer,(uint) buffer);

                        gl.VertexAttribPointer((uint)programAttribute + 0, 4, type, false, 64, 0);
                        gl.VertexAttribPointer((uint)programAttribute + 1, 4, type, false, 64, 16);
                        gl.VertexAttribPointer((uint)programAttribute + 2, 4, type, false, 64, 32);
                        gl.VertexAttribPointer((uint)programAttribute + 3, 4, type, false, 64, 48);

                    }
                    else if (name.Equals("instanceColor"))
                    {

                        var attribute = attributes.Get<float>((object3D as InstancedMesh).InstanceColor);

                        // TODO Attribute may not be available on context restore

                        if (attribute == null) continue;

                        var buffer = attribute.buffer;
                        var type = (VertexAttribPointerType)Enum.ToObject(typeof(VertexAttribPointerType), attribute.Type);

                        enableAttributeAndDivisor(programAttribute, 1);

                        gl.BindBuffer(GLEnum.ArrayBuffer, (uint)buffer);

                        gl.VertexAttribPointer((uint)programAttribute, 3, type, false, 12, 0);

                    }
                    else if (materialDefaultAttributeValues != null)
                    {

                        var value = (float[])materialDefaultAttributeValues[name];

                        if (value != null)
                        {

                            switch (value.Length)
                            {

                                case 2:
                                    gl.VertexAttrib2((uint)programAttribute, value);
                                    break;

                                case 3:
                                    gl.VertexAttrib3((uint)programAttribute, value);
                                    break;

                                case 4:
                                    gl.VertexAttrib4((uint)programAttribute, value);
                                    break;

                                default:
                                    gl.VertexAttrib1((uint)programAttribute, value[0]);
                                    break;
                            }

                        }

                    }

                }

            }

            DisableUnusedAttributes();
        }
        public virtual void Dispose()
        {
            Dispose(disposed);
        }
        protected virtual void RaiseDisposed()
        {
            var handler = this.Disposed;
            if (handler != null)
                handler(this, new EventArgs());
        }
        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            try
            {
                this.RaiseDisposed();
                this.disposed = true;
            }
            finally
            {

            }
            this.disposed = true;
        }
    }
}
