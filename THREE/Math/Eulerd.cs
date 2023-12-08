using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace THREE
{
    public class Eulerd : INotifyPropertyChanged
    {
        private double _x;
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                if (value != _x) 
                {
                    _x = value;
                    //OnRotationChange(this);
                    OnPropertyChanged();
                }
            }
        }

        private double _y;

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    //OnRotationChange(this);
                    OnPropertyChanged();
                }
            }
        }

        private double _z;
       
        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                if (value != _z)
                {
                    _z = value;
                    //OnRotationChange(this);
                    OnPropertyChanged();
                }
            }
        }
        private RotationOrder _order = RotationOrder.XYZ;
        public RotationOrder Order
        {
            get { return _order; }
            set
            {
                if (value != _order)
                {
                    this._order = value;
                    OnPropertyChanged();
                }
                
            }
        }

        //public Action<Eulerd> OnRotationChange;

        public Eulerd()
        {
            this._x = this._y = this._z = 0;
            this._order = RotationOrder.XYZ;
        }

        public Eulerd(double x, double y, double z, RotationOrder order = RotationOrder.XYZ)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this.Order = order;
        }

        public Eulerd Set(double a, double b, double c, RotationOrder o)
        {
            this._x = a;
            this._y = b;
            this._z = c;
            this.Order = o;

            this.OnPropertyChanged();

            return this;
        }

        public static double Clamp(double value, double min, double max)
        {
            return value.Clamp(min, max);
        }

        public Eulerd SetFromRotationMatrix(Matrix4d m){

            return SetFromRotationMatrix(m, this.Order);
        }

        public Eulerd SetFromRotationMatrix(Matrix4d m,RotationOrder? rotationOrder=null,bool update=true)
        {


            var te = m.Elements;

            var m11 = te[0]; var m12 = te[4]; var m13 = te[8];
            var m21 = te[1]; var m22 = te[5]; var m23 = te[9];
            var m31 = te[2]; var m32 = te[6]; var m33 = te[10];

            var order = rotationOrder != null ? rotationOrder.Value : this.Order;

            if (order == RotationOrder.XYZ)
            {
                this._y = System.Math.Asin(m13.Clamp(-1, 1));

                if (System.Math.Abs(m13) < 0.99999)
                {
                    this._x = System.Math.Atan2(-m23, m33);
                    this._z = System.Math.Atan2(-m12, m11);
                }
                else
                {
                    this._x = System.Math.Atan2(m32, m22);
                    this._z = 0;
                }
            }
            else if (order == RotationOrder.YXZ)
            {
                this._x = System.Math.Asin(m23.Clamp(-1, 1));
                if (System.Math.Abs(m23) < 0.99999)
                {
                    this._y = System.Math.Atan2(m13, m33);
                    this._z = System.Math.Atan2(m21, m22);
                }
                else
                {
                    this._y = System.Math.Atan2(-m31, m11);
                    this._z = 0;
                }
            }
            else if (order == RotationOrder.ZXY)
            {
                this._x = System.Math.Asin(m32.Clamp(-1, 1));
                if (System.Math.Abs(m32) < 0.99999)
                {
                    this._y = System.Math.Atan2(-m31, m33);
                    this._z = System.Math.Atan2(-m12, m22);
                }
                else
                {
                    this._y = 0;
                    this._z = System.Math.Atan2(m21, m11);
                }
            }
            else if (order == RotationOrder.ZYX)
            {
                this._y = System.Math.Asin(m31.Clamp(-1, 1));
                if (System.Math.Abs(m31) < 0.99999)
                {
                    this._x = System.Math.Atan2(m32, m33);
                    this._z = System.Math.Atan2(m21, m11);
                }
                else {
                    this._x = 0;
                    this._z = System.Math.Atan2(-m12,m22);
                }
            }
            else if (order == RotationOrder.YZX)
            {
                this._z = System.Math.Asin(m21.Clamp(-1, 1));
                if (System.Math.Abs(m21) < 0.99999)
                {
                    this._x = System.Math.Atan2(-m23, m22);
                    this._y = System.Math.Atan2(-m31, m11);
                }
                else
                {
                    this._x = 0;
                    this._y = System.Math.Atan2(m13, m33);
                }
            }
            else if (order == RotationOrder.XZY)
            {
                this._z = System.Math.Asin(-m12.Clamp(-1, 1));
                if (System.Math.Abs(m12) < 0.99999)
                {
                    this._x = System.Math.Atan2(m32, m22);
                    this._y = System.Math.Atan2(m13, m11);
                }
                else
                {
                    this._x = System.Math.Atan2(-m23, m33);
                    this._y = 0;
                }
            }
            else
            {
                Trace.TraceInformation("THREE.Math.Eulerd: .setFromRotationMatrix() given unsupported order:" + order);
            }

            this.Order = order;

            if (update != false) OnPropertyChanged();
            return this;
        }

        public Eulerd SetFromQuaternion(Quaterniond q,RotationOrder? order,bool update=true)
        {
            Matrix4d _m = Matrix4d.Identity().MakeRotationFromQuaternion(q);
            return SetFromRotationMatrix(_m, this.Order,update);
        }

        public Eulerd SetFromQuaternion(Quaterniond q, RotationOrder order)
        {
            var sqx = q.X * q.X;
            var sqy = q.Y * q.Y;
            var sqz = q.Z * q.Z;
            var sqw = q.W * q.W;

            this.Order = order;
            if (this.Order == RotationOrder.XYZ)
            {

                this._x = System.Math.Atan2(2 * (q.X * q.W - q.Y * q.Z), (sqw - sqx - sqy + sqz));
                this._y = System.Math.Asin(Clamp(2 * (q.X * q.Z + q.Y * q.W), -1, 1));
                this._z = System.Math.Atan2(2 * (q.Z * q.W - q.X * q.Y), (sqw + sqx - sqy - sqz));

            }
            else if (this.Order == RotationOrder.YXZ)
            {

                this._x = System.Math.Asin(Clamp(2 * (q.X * q.W - q.Y * q.Z), -1, 1));
                this._y = System.Math.Atan2(2 * (q.X * q.Z + q.Y * q.W), (sqw - sqx - sqy + sqz));
                this._z = System.Math.Atan2(2 * (q.X * q.Y + q.Z * q.W), (sqw - sqx + sqy - sqz));

            }
            else if (this.Order == RotationOrder.ZXY)
            {

                this._x = System.Math.Asin(Clamp(2 * (q.X * q.W + q.Y * q.Z), -1, 1));
                this._y = System.Math.Atan2(2 * (q.Y * q.W - q.Z * q.X), (sqw - sqx - sqy + sqz));
                this._z = System.Math.Atan2(2 * (q.Z * q.W - q.X * q.Y), (sqw - sqx + sqy - sqz));

            }
            else if (this.Order == RotationOrder.ZYX)
            {

                this._x = System.Math.Atan2(2 * (q.X * q.W + q.Z * q.Y), (sqw - sqx - sqy + sqz));
                this._y = System.Math.Asin(Clamp(2 * (q.Y * q.W - q.X * q.Z), -1, 1));
                this._z = System.Math.Atan2(2 * (q.X * q.Y + q.Z * q.W), (sqw + sqx - sqy - sqz));

            }
            else if (this.Order == RotationOrder.YZX)
            {

                this._x = System.Math.Atan2(2 * (q.X * q.W - q.Z * q.Y), (sqw - sqx + sqy - sqz));
                this._y = System.Math.Atan2(2 * (q.Y * q.W - q.X * q.Z), (sqw + sqx - sqy - sqz));
                this._z = System.Math.Asin(Clamp(2 * (q.X * q.Y + q.Z * q.W), -1, 1));

            }
            else if (this.Order == RotationOrder.XZY)
            {

                this._x = System.Math.Atan2(2 * (q.X * q.W + q.Y * q.Z), (sqw - sqx + sqy - sqz));
                this._y = System.Math.Atan2(2 * (q.X * q.Z + q.Y * q.W), (sqw + sqx - sqy - sqz));
                this._z = System.Math.Asin(Clamp(2 * (q.Z * q.W - q.X * q.Y), -1, 1));

            }
            return this;
        }
        public Eulerd SetFromVector3(Vector3 v, RotationOrder order)
        {
            return this.Set(v.X, v.Y, v.Z, order);
        }
        public void Reorder(RotationOrder newOrder)
        {
            var q = new Quaterniond().SetFromEuler(this);// FromEulerdAngles(new Vector3(this._x, this._y, this._z));
            this.Order = newOrder;
            this.SetFromQuaternion(q,newOrder);
        }

        public bool Equals(Eulerd o)
        {
            return (o._x == this._x) && (o._y == this._y) && (o._z == this._z) && (o._order == this._order);
        }

        public Eulerd fromArray(double[] array)
        {
            this._x = array[0];
            this._y = array[1];
            this._z = array[2];
            if (array.Length > 3) this._order = (RotationOrder)array[3];

            this.OnPropertyChanged();

            return this;

        }

        public double[] ToArray(int offset=0)
        {
            double[] array = new double[4];

            array[offset] = this._x;
            array[offset+1] = this._y;
            array[offset + 2] = this._z;
            array[offset + 3] = (int)this._order;

            return array;
        }

        public Vector3d ToVector3(Vector3d optionalResult=null)
        {
            if (optionalResult != null)
            {
                return optionalResult.Set(this._x, this._y, this._z);
            }
            else
            {
                return new Vector3d(this._x, this._y, this._z);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
}
