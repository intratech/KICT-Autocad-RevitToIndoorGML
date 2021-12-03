using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Adapter
{
    public class OrderPosition
    {
        public OrderPosition(Vector3 pos, int count)
        {
            this.Pos = pos;
            this.Index = count;
        }

        public Vector3 Pos { get; set; }
        public int Index { get; set; }

        public static List<OrderPosition> SortPoint(List<OrderPosition> order)
        {
            var target = new List<OrderPosition>();

            while (order.Count > target.Count)
            {
                if (target.Count == 0)
                {
                    target.Add(order.First());
                }
                var vertex = target.Last();
                int nextIdex = order.FindIndex(x => !target.Any(y => y.Index == x.Index) && IsSameSide(x, vertex, order));
                if (nextIdex == -1)
                {
                    return order;
                }
                target.Add(order[nextIdex]);
            }
            return target;
        }



        public static bool IsSameSide(OrderPosition p1, OrderPosition p2, List<OrderPosition> points)
        {
            var v1 = (p2.Pos - p1.Pos);
            Vector3? cp1 = null;



            foreach (var p in points.Where(x => x.Index != p1.Index && x.Index != p2.Index))
            {
                var v2 = (p.Pos - p1.Pos);

                if (cp1 == null)
                {
                    cp1 = Vector3.Normalize(Vector3.Cross(v1, v2));
                }
                else
                {
                    if (Vector3.Dot(cp1.Value, Vector3.Normalize(Vector3.Cross(v1, v2))) < 0)
                        return false;
                }
            }
            return true;
        }
    }
}
