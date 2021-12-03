using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Adapter
{
    class Vector3Comparer : IEqualityComparer<Vector3>
    {
        public float Tolerance = 0.001f;

        public Vector3Comparer(float tolerance)
        {
            this.Tolerance = tolerance;
        }
        public bool Equals(Vector3 x, Vector3 y)
        {
            return ((x - y).Length() < Tolerance);
        }

        public int GetHashCode(Vector3 obj)
        {
            var x =(int)obj.X;
            var y =(int)obj.Y;
            var z = (int)obj.Z;

            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
        }
    }
}
