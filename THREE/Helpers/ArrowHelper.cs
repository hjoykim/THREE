namespace THREE
{
    public class ArrowHelper : Object3D
    {
        Vector3 Dir = new Vector3(0,0,1);
        Vector3 Origin = Vector3.Zero();
        float Length = 1;
        Color Color =Color.Hex(0xffff00);
        float HeadLength;
        float HeadWidth;

        Vector3 _axis = Vector3.Zero();
        BufferGeometry _lineGeometry;
        CylinderBufferGeometry _coneGeometry;
        Line line;
        Mesh cone;
        public ArrowHelper(Vector3 dir=null,Vector3 origin=null,float? length=null,Color? color=null,float? headLength=null,float? headWidth=null)
        {
            if (dir != null) Dir = dir;

            if (origin != null) Origin = origin;

            if (length != null) Length = length.Value;

            if (color != null) Color = color.Value;

            HeadLength = headLength != null ? headLength.Value : 0.2f * Length;

            HeadWidth = headWidth != null ? headWidth.Value : 0.2f * Length;

            if(_lineGeometry==null)
            {
                _lineGeometry = new BufferGeometry();
                _lineGeometry.SetAttribute("position", new BufferAttribute<float>(new float[] { 0, 0, 0, 0, 1, 0 }, 3));

                _coneGeometry = new CylinderBufferGeometry(0, 0.5f, 1, 5, 1);
                _coneGeometry.Translate(0, -0.5f, 0);
            }

            Position.Copy(Origin);

            line = new Line(_lineGeometry, new LineBasicMaterial() {Color=this.Color,ToneMapped=false });
            line.MatrixAutoUpdate = false;
            this.Add(line);

            cone = new Mesh(_coneGeometry, new MeshBasicMaterial() { Color = this.Color ,ToneMapped=false});
            cone.MatrixAutoUpdate = false;
            this.Add(cone);

            this.SetDirection(Dir);
            this.SetLength(Length, HeadLength, HeadWidth);

        }

        protected ArrowHelper(ArrowHelper source) : base()
        {
            this.line = (Line)source.line.Clone();
            this.cone = (Mesh)source.cone.Clone();
        }
        public new object Clone()
        {
            return new ArrowHelper(this);
        }
        private void SetDirection(Vector3 dir)
        {
            // dir is assumed to be normalized

            if (dir.Y > 0.99999)
            {
                this.Quaternion.Set(0, 0, 0, 1);
            }
            else if (dir.Y < -0.99999)
            {
                this.Quaternion.Set(1, 0, 0, 0);
            }
            else
            {

                _axis.Set(dir.Z, 0, -dir.X).Normalize();

                var radians = System.Math.Acos(dir.Y);

                this.Quaternion.SetFromAxisAngle(_axis, (float)radians);

            }
        }

        private void SetLength(float length,float headLength,float headWidth)
        {
            this.line.Scale.Set(1, System.Math.Max(0.0001f, length - headLength), 1); // see #17458

            this.line.UpdateMatrix();

            this.cone.Scale.Set(headWidth, headLength, headWidth);

            this.cone.Position.Y = length;

            this.cone.UpdateMatrix();
        }

        public void SetColor(Color color)
        {
            this.line.Material.Color = color;

            this.cone.Material.Color = color;
        }
    }
}
