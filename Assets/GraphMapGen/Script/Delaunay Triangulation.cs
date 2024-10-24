//using System;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class DelaunayTriangulation1 : MonoBehaviour
//{
//    public List<Vector2> points;
//    private List<Triangle> triangulation;

//    public void Triangulation(List<Vector3> RoomDetectionList)
//    {
//        points = new List<Vector2>();
//        HashSet<Vector2> uniquePoints = new HashSet<Vector2>(); // �ߺ��� ���� ó���ϱ� ���� �ؽü�

//        // 3D ��ǥ�� 2D�� ��ȯ�Ͽ� �ߺ����� �ʴ� ���� �߰�
//        for (int i = 0; i < RoomDetectionList.Count; i++)
//        {
//            Vector2 point = new Vector2(RoomDetectionList[i].x, RoomDetectionList[i].z);
//            if (!uniquePoints.Contains(point))
//            {
//                points.Add(point);
//                uniquePoints.Add(point);
//            }
//        }

//        // ������ �������� �ִ��� Ȯ�� (�������� ��� ó��)
//        if (IsCollinear(points))
//        {
//            Debug.LogWarning("Points are collinear. Cannot perform triangulation.");
//            return;
//        }

//        triangulation = BowyerWatson(points);
//        Debug.Log(triangulation);
//        foreach (var triangle in triangulation)
//        {
//            Debug.Log(triangle.ToString());
//        }
//    }

//    private bool IsCollinear(List<Vector2> points)
//    {
//        if (points.Count < 3) return true;

//        Vector2 a = points[0];
//        Vector2 b = points[1];
//        for (int i = 2; i < points.Count; i++)
//        {
//            Vector2 c = points[i];
//            float crossProduct = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
//            if (Mathf.Abs(crossProduct) > Mathf.Epsilon) return false;
//        }
//        return true;
//    }
//    public List<Triangle> BowyerWatson(List<Vector2> points)
//    {
//        Triangle superTriangle = CreateSuperTriangle(points);
//        List<Triangle> triangleList = new List<Triangle> { superTriangle };

//        foreach (Vector2 point in points)
//        {
//            // ���� �����ϴ� �ﰢ���� �ϳ��� ó��
//            for (int i = 0; i < triangleList.Count; i++)
//            {
//                Triangle triangle = triangleList[i];

//                // �ﰢ���� ���� ���� �������� �ȿ� �����ϰ� ������, ó�� �� ���ο� �ﰢ���� �߰�
//                if (triangle.IsPointInsideCircumcircle(point))
//                {
//                    HashSet<Edge> polygon = new HashSet<Edge>();

//                    foreach (Edge edge in triangle.GetEdges())
//                    {
//                        // �ﰢ���� ���� �ٸ� �ﰢ����� �������� ������ �߰�
//                        if (!IsEdgeShared(edge, triangleList))
//                        {
//                            polygon.Add(edge);
//                        }
//                    }

//                    // ���� �ﰢ�� ����
//                    triangleList.Remove(triangle);
//                    i--;  // �ﰢ���� ���������Ƿ� �ε����� �� ĭ �ڷ� �̵�

//                    // ���ο� �ﰢ���� �����Ͽ� ����Ʈ�� �߰�
//                    foreach (Edge edge in polygon)
//                    {
//                        Triangle newTriangle = new Triangle(edge.v1, edge.v2, point);
//                        triangleList.Add(newTriangle);
//                    }

//                    // �� ���� �ϳ��� �ﰢ���� ó���ϴ� ���̹Ƿ� ���⼭ break
//                    break;
//                }
//            }
//        }

//        // ���� Ʈ���̾ޱ� ����
//        triangleList.RemoveAll(t => t.SharesVertexWith(superTriangle));

//        return triangleList;
//    }

//    // �� �ﰢ���� ������ ���� �����ϴ��� Ȯ���ϴ� �Լ� ����
//    private bool IsEdgeShared(Edge edge, List<Triangle> triangles)
//    {
//        HashSet<Edge> edges = new HashSet<Edge>();

//        // �ﰢ�� ����Ʈ���� ��� ���� Ȯ���Ͽ� HashSet�� �߰�
//        foreach (Triangle triangle in triangles)
//        {
//            foreach (Edge triEdge in triangle.GetEdges())
//            {
//                if (edges.Contains(triEdge))
//                {
//                    // �̹� ���� ������ ���� ���� ���̹Ƿ� true ��ȯ
//                    return true;
//                }
//                edges.Add(triEdge);
//            }
//        }

//        // �ߺ��� ���� ���ٸ� false ��ȯ
//        return false;
//    }


//    private Triangle CreateSuperTriangle(List<Vector2> points)
//    {
//        float minX = points[0].x, maxX = points[0].x;
//        float minY = points[0].y, maxY = points[0].y;

//        foreach (var point in points)
//        {
//            minX = Mathf.Min(minX, point.x);
//            maxX = Mathf.Max(maxX, point.x);
//            minY = Mathf.Min(minY, point.y);
//            maxY = Mathf.Max(maxY, point.y);
//        }

//        float dx = maxX - minX;
//        float dy = maxY - minY;
//        float deltaMax = Mathf.Max(dx, dy) * 2f;

//        Vector2 v1 = new Vector2(minX - deltaMax, minY - deltaMax);
//        Vector2 v2 = new Vector2(minX + 2 * deltaMax, minY - deltaMax);
//        Vector2 v3 = new Vector2(minX - deltaMax, minY + 2 * deltaMax);

//        return new Triangle(v1, v2, v3);
//    }

   

//    private void OnDrawGizmos()
//    {
//        if (triangulation != null)
//        {
//            Gizmos.color = Color.green;

//            foreach (var triangle in triangulation)
//            {
//                Gizmos.DrawLine(new Vector3(triangle.v1.x, 0, triangle.v1.y), new Vector3(triangle.v2.x, 0, triangle.v2.y));
//                Gizmos.DrawLine(new Vector3(triangle.v2.x, 0, triangle.v2.y), new Vector3(triangle.v3.x, 0, triangle.v3.y));
//                Gizmos.DrawLine(new Vector3(triangle.v3.x, 0, triangle.v3.y), new Vector3(triangle.v1.x, 0, triangle.v1.y));
//            }

//            // ������ ���� ��ü�� ǥ��
//            Gizmos.color = Color.red;
//            foreach (var point in points)
//            {
//                Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 0.1f);
//            }
//        }
//    }
//}

//// �ﰢ�� Ŭ����
//public class Triangle
//{
//    public Vector2 v1, v2, v3;

//    public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
//    {
//        this.v1 = v1;
//        this.v2 = v2;
//        this.v3 = v3;
//    }

//    public bool IsPointInsideCircumcircle(Vector2 p)
//    {
//        Vector2 A = v1;
//        Vector2 B = v2;
//        Vector2 C = v3;

//        // ������ ����� ��ǥ ���
//        float ax = A.x - p.x;
//        float ay = A.y - p.y;
//        float bx = B.x - p.x;
//        float by = B.y - p.y;
//        float cx = C.x - p.x;
//        float cy = C.y - p.y;

//        // ��Ľ� ����� ���� ������ �Ǻ�
//        float det = (ax * (by * (cx * cx + cy * cy) - cy * (bx * bx + by * by))
//                   - ay * (bx * (cx * cx + cy * cy) - cx * (bx * bx + by * by))
//                   + (ax * ax + ay * ay) * (bx * cy - by * cx));


//        // det > 0�̸� �� p�� ������ ���ο� ����
//        return det > 0;
//    }

//    public bool SharesVertexWith(Triangle other)
//    {
//        return v1 == other.v1 || v1 == other.v2 || v1 == other.v3 ||
//               v2 == other.v1 || v2 == other.v2 || v2 == other.v3 ||
//               v3 == other.v1 || v3 == other.v2 || v3 == other.v3;
//    }

//    public List<Edge> GetEdges()
//    {
//        return new List<Edge> { new Edge(v1, v2), new Edge(v2, v3), new Edge(v3, v1) };
//    }

//    public bool HasEdge(Edge edge)
//    {
//        return (v1 == edge.v1 && v2 == edge.v2) || (v2 == edge.v1 && v3 == edge.v2) ||
//               (v3 == edge.v1 && v1 == edge.v2);
//    }

//    public override string ToString()
//    {
//        return $"Triangle: {v1}, {v2}, {v3}";
//    }
//}

//// �� Ŭ����
//public class Edge : IEquatable<Edge>
//{
//    public Vector2 v1, v2;

//    public Edge(Vector2 v1, Vector2 v2)
//    {
//        // �������� ������ � �����ε� �񱳵� �� �ְ� �׻� ���� ������ ����
//        if (v1.x < v2.x || (v1.x == v2.x && v1.y < v2.y))
//        {
//            this.v1 = v1;
//            this.v2 = v2;
//        }
//        else
//        {
//            this.v1 = v2;
//            this.v2 = v1;
//        }
//    }

//    public bool Equals(Edge other)
//    {
//        if (other == null) return false;
//        // �� Edge�� �������� Ȯ�� (�������� ������ ������ ������� ��)
//        return (v1 == other.v1 && v2 == other.v2);
//    }

//    public override int GetHashCode()
//    {
//        // �� ���� �ؽ��ڵ带 �����Ͽ� ��ȯ
//        return v1.GetHashCode() ^ v2.GetHashCode();
//    }
//}
