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
//        HashSet<Vector2> uniquePoints = new HashSet<Vector2>(); // 중복된 점을 처리하기 위한 해시셋

//        // 3D 좌표를 2D로 변환하여 중복되지 않는 점만 추가
//        for (int i = 0; i < RoomDetectionList.Count; i++)
//        {
//            Vector2 point = new Vector2(RoomDetectionList[i].x, RoomDetectionList[i].z);
//            if (!uniquePoints.Contains(point))
//            {
//                points.Add(point);
//                uniquePoints.Add(point);
//            }
//        }

//        // 점들이 일직선상에 있는지 확인 (일직선일 경우 처리)
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
//            // 점을 포함하는 삼각형을 하나씩 처리
//            for (int i = 0; i < triangleList.Count; i++)
//            {
//                Triangle triangle = triangleList[i];

//                // 삼각형이 현재 점을 외접원의 안에 포함하고 있으면, 처리 후 새로운 삼각형을 추가
//                if (triangle.IsPointInsideCircumcircle(point))
//                {
//                    HashSet<Edge> polygon = new HashSet<Edge>();

//                    foreach (Edge edge in triangle.GetEdges())
//                    {
//                        // 삼각형의 변이 다른 삼각형들과 공유되지 않으면 추가
//                        if (!IsEdgeShared(edge, triangleList))
//                        {
//                            polygon.Add(edge);
//                        }
//                    }

//                    // 기존 삼각형 삭제
//                    triangleList.Remove(triangle);
//                    i--;  // 삼각형을 삭제했으므로 인덱스를 한 칸 뒤로 이동

//                    // 새로운 삼각형을 생성하여 리스트에 추가
//                    foreach (Edge edge in polygon)
//                    {
//                        Triangle newTriangle = new Triangle(edge.v1, edge.v2, point);
//                        triangleList.Add(newTriangle);
//                    }

//                    // 한 번에 하나의 삼각형만 처리하는 것이므로 여기서 break
//                    break;
//                }
//            }
//        }

//        // 슈퍼 트라이앵글 제거
//        triangleList.RemoveAll(t => t.SharesVertexWith(superTriangle));

//        return triangleList;
//    }

//    // 두 삼각형이 동일한 변을 공유하는지 확인하는 함수 개선
//    private bool IsEdgeShared(Edge edge, List<Triangle> triangles)
//    {
//        HashSet<Edge> edges = new HashSet<Edge>();

//        // 삼각형 리스트에서 모든 변을 확인하여 HashSet에 추가
//        foreach (Triangle triangle in triangles)
//        {
//            foreach (Edge triEdge in triangle.GetEdges())
//            {
//                if (edges.Contains(triEdge))
//                {
//                    // 이미 변이 있으면 공유 중인 변이므로 true 반환
//                    return true;
//                }
//                edges.Add(triEdge);
//            }
//        }

//        // 중복된 변이 없다면 false 반환
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

//            // 점들을 작은 구체로 표시
//            Gizmos.color = Color.red;
//            foreach (var point in points)
//            {
//                Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 0.1f);
//            }
//        }
//    }
//}

//// 삼각형 클래스
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

//        // 점들의 상대적 좌표 계산
//        float ax = A.x - p.x;
//        float ay = A.y - p.y;
//        float bx = B.x - p.x;
//        float by = B.y - p.y;
//        float cx = C.x - p.x;
//        float cy = C.y - p.y;

//        // 행렬식 계산을 통한 외접원 판별
//        float det = (ax * (by * (cx * cx + cy * cy) - cy * (bx * bx + by * by))
//                   - ay * (bx * (cx * cx + cy * cy) - cx * (bx * bx + by * by))
//                   + (ax * ax + ay * ay) * (bx * cy - by * cx));


//        // det > 0이면 점 p가 외접원 내부에 있음
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

//// 변 클래스
//public class Edge : IEquatable<Edge>
//{
//    public Vector2 v1, v2;

//    public Edge(Vector2 v1, Vector2 v2)
//    {
//        // 시작점과 끝점이 어떤 순서로든 비교될 수 있게 항상 작은 값부터 저장
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
//        // 두 Edge가 동일한지 확인 (시작점과 끝점의 순서에 상관없이 비교)
//        return (v1 == other.v1 && v2 == other.v2);
//    }

//    public override int GetHashCode()
//    {
//        // 두 점의 해시코드를 조합하여 반환
//        return v1.GetHashCode() ^ v2.GetHashCode();
//    }
//}
