using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**
 *  * Port from https://github.com/mapbox/earcut (v2.2.2)
 */
namespace THREE.Extras
{
    public class Node
    {
        public float X;

        public float Y;

        public int Z = 0;

        public int I;

        // previous and next vertice nodes in a polygon ring
        public Node Prev;

        public Node Next;

        // previous and next nodes in z-order
        public Node PrevZ;

        public Node NextZ;

        // indicates whether this is a steiner point
        public bool Steiner = false;

        public Node(int i,float x,float y)
        {
            I = i;
            X = x;
            Y = y;
        }

    }
    public class Earcut
    {
        private int dim;
        public List<int> Triangulate(List<float> data,List<int> holeIndices,int? dim=null)
        {
            dim = dim != null ? dim : 2;

            var hasHoles = holeIndices != null && holeIndices.Count > 0;

            var outerLen = hasHoles ? holeIndices[0] * dim.Value : data.Count;

            var outerNode = LinkedList(data, 0, outerLen, dim.Value, true);

            List<int> triangles = new List<int>();

            if (outerNode==null || outerNode.Next == outerNode.Prev) return triangles;

            float minX=0, minY=0, maxX, maxY, x, y, invSize=0;

            if (hasHoles) outerNode = EliminateHoles(data, holeIndices, outerNode, dim.Value);

            // if the shape is not too simple, we'll use z-order curve hash later; calculate polygon bbox
            if (data.Count > 80 * dim)
            {
                minX = maxX = data[0];
                minY = maxY = data[1];

                for (int i = dim.Value; i < outerLen; i += dim.Value)
                {
                    x = data[i];
                    y = data[i + 1];
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }

                // minX, minY and invSize are later used to transform coords into integers for z-order calculation
                invSize = (float)System.Math.Max(maxX - minX, maxY - minY);
                invSize = invSize != 0 ? 1 / invSize : 0;
            }

            EarcutLinked(outerNode, triangles, dim.Value, minX, minY, invSize);
            
            return triangles;
        }

        private bool NodeEquals(Node p1,Node p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public Node LinkedList(List<float> data,int start,int end,int dim,bool clockwise)
        {
            int i;
            Node last=null;

            if (clockwise == (SignedArea(data, start, end, dim) > 0))
            {
                for (i = start; i < end; i += dim)
                {
                    last = InsertNode(i, data[i], data[i + 1], last);
                }
            }
            else
            {
                for (i = end - dim; i >= start; i -= dim) 
                {
                    last = InsertNode(i, data[i], data[i + 1], last);
                }
            }

            if(last!=null && NodeEquals(last,last.Next))
            {
                RemoveNode(last);
                last = last.Next;
            }

            return last;
        }

        public Node FilterPoints(Node start=null,Node end=null)
        {
            if (start == null) return start;
            if (end == null) end = start;

            var p = start;
            bool again;

            do
            {
                again = false;

                if (!p.Steiner && (NodeEquals(p, p.Next) || Area(p.Prev, p, p.Next) == 0))
                {
                    RemoveNode(p);
                    p = end = p.Prev;

                    if (p == p.Next) break;

                    again = true;
                }
                else
                {
                    p = p.Next;
                }
            } while (again || p != end);

            return end;
        }

        public void EarcutLinked(Node ear,List<int> triangles,int dim,float minX,float minY,float invSize,int? pass=null)
        {
            if (ear==null) return;

            // interlink polygon nodes in z-order
            if (pass==null && invSize!=0) IndexCurve(ear, minX, minY, invSize);

            Node stop = ear;
            Node prev, next;

            // iterate through ears, slicing them one by one
            while (ear.Prev != ear.Next)
            {
                prev = ear.Prev;
                next = ear.Next;

                if (invSize!=0 ? IsEarHashed(ear, minX, minY, invSize) : IsEar(ear))
                {
                    // cut off the triangle
                    triangles.Add(prev.I / dim);
                    triangles.Add(ear.I / dim);
                    triangles.Add(next.I / dim);

                    RemoveNode(ear);

                    // skipping the next vertex leads to less sliver triangles
                    ear = next.Next;
                    stop = next.Next;

                    continue;
                }

                ear = next;

                // if we looped through the whole remaining polygon and can't find any more ears
                if (ear == stop)
                {
                    // try filtering points and slicing again
                    if (pass==null)
                    {
                        EarcutLinked(FilterPoints(ear), triangles, dim, minX, minY, invSize, 1);

                        // if this didn't work, try curing all small self-intersections locally
                    }
                    else if (pass == 1)
                    {
                        ear = CureLocalIntersections(FilterPoints(ear), triangles, dim);
                        EarcutLinked(ear, triangles, dim, minX, minY, invSize, 2);

                        // as a last resort, try splitting the remaining polygon into two
                    }
                    else if (pass == 2)
                    {
                        SplitEarcut(ear, triangles, dim, minX, minY, invSize);
                    }

                    break;
                }
            }
        }
        public Node InsertNode(int i,float x,float y,Node last)
        {
            Node p = new Node(i, x, y);

            if(last==null)
            {
                p.Prev = p;
                p.Next = p;
            }
            else
            {
                p.Next = last.Next;
                p.Prev = last;
                last.Next.Prev = p;
                last.Next = p;
            }

            return p;
        }
        public bool Intersects(Node p1, Node q1, Node p2, Node q2)
        {
            var o1 = Sign(Area(p1, q1, p2));
            var o2 = Sign(Area(p1, q1, q2));
            var o3 = Sign(Area(p2, q2, p1));
            var o4 = Sign(Area(p2, q2, q1));

            if (o1 != o2 && o3 != o4) return true; // general case

            if (o1 == 0 && OnSegment(p1, p2, q1)) return true; // p1, q1 and p2 are collinear and p2 lies on p1q1
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true; // p1, q1 and q2 are collinear and q2 lies on p1q1
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true; // p2, q2 and p1 are collinear and p1 lies on p2q2
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true; // p2, q2 and q1 are collinear and q1 lies on p2q2

            return false;
        }
        public int Sign(float num)
        {
            return num > 0 ? 1 : num < 0 ? -1 : 0;
        }
        public bool OnSegment(Node p, Node q, Node r)
        {
            return q.X <= System.Math.Max(p.X, r.X) && q.X >= System.Math.Min(p.X, r.X) && q.Y <= System.Math.Max(p.Y, r.Y) && q.Y >= System.Math.Min(p.Y, r.Y);
        }
        public bool LocallyInside(Node a, Node b)
        {
            return Area(a.Prev, a, a.Next) < 0 ?
                Area(a, b, a.Next) >= 0 && Area(a, a.Prev, b) >= 0 :
                Area(a, b, a.Prev) < 0 || Area(a, a.Next, b) < 0;
        }
        public Node CureLocalIntersections(Node start, List<int> triangles, int dim)
        {
            var p = start;
            do
            {
                Node a = p.Prev,
                     b = p.Next.Next;

                if (!NodeEquals(a, b) && Intersects(a, p, p.Next, b) && LocallyInside(a, b) && LocallyInside(b, a))
                {

                    triangles.Add(a.I / dim);
                    triangles.Add(p.I / dim);
                    triangles.Add(b.I / dim);

                    // remove two nodes involved
                    RemoveNode(p);
                    RemoveNode(p.Next);

                    p = start = b;
                }
                p = p.Next;
            } while (p != start);

            return FilterPoints(p);
        }

        // try splitting polygon into two and triangulate them independently
        public void SplitEarcut(Node start, List<int> triangles, int dim, float minX, float minY, float invSize)
        {
            // look for a valid diagonal that divides the polygon into two
            var a = start;
            do
            {
                var b = a.Next.Next;
                while (b != a.Prev)
                {
                    if (a.I != b.I && IsValidDiagonal(a, b))
                    {
                        // split the polygon in two by the diagonal
                        var c =SplitPolygon(a, b);

                        // filter colinear points around the cuts
                        a = FilterPoints(a, a.Next);
                        c = FilterPoints(c, c.Next);

                        // run earcut on each half
                        EarcutLinked(a, triangles, dim, minX, minY, invSize);
                        EarcutLinked(c, triangles, dim, minX, minY, invSize);
                        return;
                    }
                    b = b.Next;
                }
                a = a.Next;
            } while (a != start);
        }
        public Node EliminateHoles(List<float> data, List<int> holeIndices, Node outerNode, int dim)
        {
            List<Node> queue = new List<Node>();
            int i, len;
            int start, end;
            Node list;

            for (i = 0, len = holeIndices.Count; i < len; i++)
            {
                start = holeIndices[i] * dim;
                end = i < len - 1 ? holeIndices[i + 1] * dim : data.Count;
                list = LinkedList(data, start, end, dim, false);
                if (list == list.Next) list.Steiner = true;
                queue.Add(GetLeftmost(list));
            }

            queue.Sort(delegate (Node a, Node b) 
            {
                return (int)(a.X - b.X);
            });

            // process holes from left to right
            for (i = 0; i < queue.Count; i++)
            {
                EliminateHole(queue[i], outerNode);
                outerNode = FilterPoints(outerNode, outerNode.Next);
            }

            return outerNode;
        }
        public void EliminateHole(Node hole, Node outerNode)
        {
            outerNode = FindHoleBridge(hole, outerNode);
            if (outerNode!=null)
            {
                var b = SplitPolygon(outerNode, hole);

                // filter collinear points around the cuts
                FilterPoints(outerNode, outerNode.Next);
                FilterPoints(b, b.Next);
            }
        }
        // David Eberly's algorithm for finding a bridge between hole and outer polygon
        public Node FindHoleBridge(Node hole, Node outerNode)
        {
            Node p = outerNode;
            float hx = hole.X,
                hy = hole.Y,
                qx = float.NegativeInfinity;

            Node m = null;

            // find a segment intersected by a ray from the hole's leftmost point to the left;
            // segment's endpoint with lesser x will be potential connection point
            do
            {
                if (hy <= p.Y && hy >= p.Next.Y && p.Next.Y != p.Y)
                {
                    var x = p.X + (hy - p.Y) * (p.Next.X - p.X) / (p.Next.Y - p.Y);
                    if (x <= hx && x > qx)
                    {
                        qx = x;
                        if (x == hx)
                        {
                            if (hy == p.Y) return p;
                            if (hy == p.Next.Y) return p.Next;
                        }
                        m = p.X < p.Next.X ? p : p.Next;
                    }
                }
                p = p.Next;
            } while (p != outerNode);

            if (m==null) return null;

            if (hx == qx) return m; // hole touches outer segment; pick leftmost endpoint

            // look for points inside the triangle of hole point, segment intersection and endpoint;
            // if there are no points found, we have a valid connection;
            // otherwise choose the point of the minimum angle with the ray as connection point

            Node stop = m;
            float mx = m.X,
                my = m.Y,
                tanMin = float.PositiveInfinity,
                tan;

            p = m;

            do
            {
                if (hx >= p.X && p.X >= mx && hx != p.X &&
                        PointInTriangle(hy < my ? hx : qx, hy, mx, my, hy < my ? qx : hx, hy, p.X, p.Y))
                {

                    tan = (float)System.Math.Abs(hy - p.Y) / (hx - p.X); // tangential

                    if (LocallyInside(p, hole) &&
                        (tan < tanMin || (tan == tanMin && (p.X > m.X || (p.X == m.X && SectorContainsSector(m, p))))))
                    {
                        m = p;
                        tanMin = tan;
                    }
                }

                p = p.Next;
            } while (p != stop);

            return m;
        }
        public bool SectorContainsSector(Node m, Node p)
        {
            return Area(m.Prev, m, p.Prev) < 0 && Area(p.Next, m, m.Next) < 0;
        }

        // Simon Tatham's linked list merge sort algorithm
        // http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html
        public Node SortLinked(Node list)
        {
            Node p, q, e, tail;
            int numMerges, pSize, qSize;
            int inSize = 1;

            do
            {
                p = list;
                list = null;
                tail = null;
                numMerges = 0;

                while (p!=null)
                {
                    numMerges++;
                    q = p;
                    pSize = 0;
                    for (var i = 0; i < inSize; i++)
                    {
                        pSize++;
                        q = q.NextZ;
                        if (q==null) break;
                    }
                    qSize = inSize;

                    while (pSize > 0 || (qSize > 0 && q!=null))
                    {

                        if (pSize != 0 && (qSize == 0 || q==null || p.Z <= q.Z))
                        {
                            e = p;
                            p = p.NextZ;
                            pSize--;
                        }
                        else
                        {
                            e = q;
                            q = q.NextZ;
                            qSize--;
                        }

                        if (tail!=null) tail.NextZ = e;
                        else list = e;

                        e.PrevZ = tail;
                        tail = e;
                    }

                    p = q;
                }

                tail.NextZ = null;
                inSize *= 2;

            } while (numMerges > 1);

            return list;
        }
        public Node GetLeftmost(Node start)
        {
            Node p = start,
                leftmost = start;
            do
            {
                if (p.X < leftmost.X || (p.X == leftmost.X && p.Y < leftmost.Y)) leftmost = p;
                p = p.Next;
            } while (p != start);

            return leftmost;
        }
        public Node SplitPolygon(Node a, Node b)
        {
            Node a2 = new Node(a.I, a.X, a.Y),
                b2 = new Node(b.I, b.X, b.Y),
                an = a.Next,
                bp = b.Prev;

            a.Next = b;
            b.Prev = a;

            a2.Next = an;
            an.Prev = a2;

            b2.Next = a2;
            a2.Prev = b2;

            bp.Next = b2;
            b2.Prev = bp;

            return b2;
        }
        public bool IsValidDiagonal(Node a, Node b)
        {
            return a.Next.I != b.I && a.Prev.I != b.I && !IntersectsPolygon(a, b) && // dones't intersect other edges
                   (LocallyInside(a, b) && LocallyInside(b, a) && MiddleInside(a, b) && // locally visible
                    (Area(a.Prev, a, b.Prev)>0 || Area(a, b.Prev, b)>0) || // does not create opposite-facing sectors
                    NodeEquals(a, b) && Area(a.Prev, a, a.Next) > 0 && Area(b.Prev, b, b.Next) > 0); // special zero-length case
        }
        public bool MiddleInside(Node a, Node b)
        {
            Node p = a;
            bool inside = false;
            float px = (a.X+ b.X) / 2,
                 py = (a.Y + b.Y) / 2;
            do
            {
                if (((p.Y > py) != (p.Next.Y > py)) && p.Next.Y != p.Y &&
                        (px < (p.Next.X - p.X) * (py - p.Y) / (p.Next.Y - p.Y) + p.X))
                    inside = !inside;
                p = p.Next;
            } while (p != a);

            return inside;
        }
        public bool IntersectsPolygon(Node a, Node b)
        {
            var p = a;
            do
            {
                if (p.I != a.I && p.Next.I != a.I && p.I != b.I && p.Next.I != b.I &&
                        Intersects(p, p.Next, a, b)) return true;
                p = p.Next;
            } while (p != a);

            return false;
        }
        public void RemoveNode(Node p)
        {
            p.Next.Prev = p.Prev;
            p.Prev.Next = p.Next;

            if (p.PrevZ!=null) p.PrevZ.NextZ = p.NextZ;
            if (p.NextZ != null) p.NextZ.PrevZ = p.PrevZ;
        }
        public bool IsEar(Node ear)
        {
            Node a = ear.Prev,
                b = ear,
                c = ear.Next;

            if (Area(a, b, c) >= 0) return false; // reflex, can't be an ear

            // now make sure we don't have other points inside the potential ear
            var p = ear.Next.Next;

            while (p != ear.Prev)
            {
                if (PointInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y) &&
                    Area(p.Prev, p, p.Next) >= 0) return false;
                p = p.Next;
            }

            return true;
        }

        public bool IsEarHashed(Node ear, float minX, float minY, float invSize)
        {
            Node a = ear.Prev,
                b = ear,
                c = ear.Next;

            if (Area(a, b, c) >= 0) return false; // reflex, can't be an ear

            // triangle bbox; min & max are calculated like this for speed
            float minTX = a.X < b.X ? (a.X < c.X ? a.X : c.X) : (b.X < c.X ? b.X : c.X),
                  minTY = a.Y < b.Y ? (a.Y < c.Y ? a.Y : c.Y) : (b.Y < c.Y ? b.Y : c.Y),
                  maxTX = a.X > b.X ? (a.X > c.X ? a.X : c.X) : (b.X > c.X ? b.X : c.X),
                  maxTY = a.Y > b.Y ? (a.Y > c.Y ? a.Y : c.Y) : (b.Y > c.Y ? b.Y : c.Y);

            // z-order range for the current triangle bbox;
            float minZ = zOrder(minTX, minTY, minX, minY, invSize),
                  maxZ = zOrder(maxTX, maxTY, minX, minY, invSize);

            Node p = ear.PrevZ,
                n = ear.NextZ;

            // look for points inside the triangle in both directions
            while (p!=null && p.Z >= minZ && n!=null && n.Z <= maxZ)
            {
                if (p != ear.Prev && p != ear.Next &&
                    PointInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y) &&
                    Area(p.Prev, p, p.Next) >= 0) return false;
                p = p.PrevZ;

                if (n != ear.Prev && n != ear.Next &&
                    PointInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, n.X, n.Y) &&
                    Area(n.Prev, n, n.Next) >= 0) return false;
                n = n.NextZ;
            }

            // look for remaining points in decreasing z-order
            while (p!=null && p.Z >= minZ)
            {
                if (p != ear.Prev && p != ear.Next &&
                    PointInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, p.X, p.Y) &&
                    Area(p.Prev, p, p.Next) >= 0) return false;
                p = p.PrevZ;
            }

            // look for remaining points in increasing z-order
            while (n!=null && n.Z <= maxZ)
            {
                if (n != ear.Prev && n != ear.Next &&
                    PointInTriangle(a.X, a.Y, b.X, b.Y, c.X, c.Y, n.X, n.Y) &&
                    Area(n.Prev, n, n.Next) >= 0) return false;
                n = n.NextZ;
            }

            return true;
        }
        // interlink polygon nodes in z-order
        public void IndexCurve(Node start, float minX, float minY, float invSize)
        {
            var p = start;
            do
            {
                if (p.Z == 0) p.Z = zOrder(p.X, p.Y, minX, minY, invSize);
                p.PrevZ = p.Prev;
                p.NextZ = p.Next;
                p = p.Next;
            } while (p != start);

            p.PrevZ.NextZ = null;
            p.PrevZ = null;

            SortLinked(p);
        
        }
       
        public int zOrder(float _x, float _y, float minX, float minY, float invSize)
        {
            // coords are transformed into non-negative 15-bit integer range
            int x = Convert.ToInt32(32767 * (_x - minX) * invSize);
            int y = Convert.ToInt32(32767 * (_y - minY) * invSize);

            x = (x | (x << 8)) & 0x00FF00FF;
            x = (x | (x << 4)) & 0x0F0F0F0F;
            x = (x | (x << 2)) & 0x33333333;
            x = (x | (x << 1)) & 0x55555555;

            y = (y | (y << 8)) & 0x00FF00FF;
            y = (y | (y << 4)) & 0x0F0F0F0F;
            y = (y | (y << 2)) & 0x33333333;
            y = (y | (y << 1)) & 0x55555555;

            return x | (y << 1);
        }
        public float Area(Node p, Node q, Node r)
        {
            return (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
        }
        public float SignedArea(List<float> data, int start, int end, int dim)
        {
            var sum = 0.0f;
            for (int i = start, j = end - dim; i < end; i += dim)
            {
                var sum1 = (data[j] - data[i]);
                var sum2 = (data[i + 1] + data[j + 1]);
                var data1 = data[j];
                var data2 = data[i];
                var data3 = data[i + 1];
                var data4 = data[j + 1];
                var sum3 = sum1 * sum2; 
                sum += (data[j] - data[i]) * (data[i + 1] + data[j + 1]);
                j = i;
            }
            return sum;
        }
        public bool PointInTriangle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
        {
            return (cx - px) * (ay - py) - (ax - px) * (cy - py) >= 0 &&
                   (ax - px) * (by - py) - (bx - px) * (ay - py) >= 0 &&
                   (bx - px) * (cy - py) - (cx - px) * (by - py) >= 0;
        }

    }
}
