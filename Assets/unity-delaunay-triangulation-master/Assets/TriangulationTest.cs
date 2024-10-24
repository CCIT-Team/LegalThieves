using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TriangulationTest : MonoBehaviour
{
    public List<Vector3> points;
    int gizmosCount = 0;

    // 삼각형을 그리기 위한 데이터
    private List<Vector3> roomVectors;
    private List<int> indices;
    private List<Edge> edges; // 모든 삼각형의 변을 저장
    private List<Edge> mstEdges; // 최소 신장 트리의 변을 저장
    private List<Edge> excludedEdges; // MST에 제외된 엣지 저장
    private List<int> leafRooms; // 끝부분 방 저장 (리프 노드)

    // 삼각분할 및 기즈모 그리기 호출 함수
    public void StartTriangulation(List<Vector3> list)
    {
        gizmosCount = 0;
        points = list;
        roomVectors = new List<Vector3>();
        indices = new List<int>();
        edges = new List<Edge>();
        mstEdges = new List<Edge>();
        excludedEdges = new List<Edge>();
        leafRooms = new List<int>();

        List<Vertex> triangulationData = new List<Vertex>();

        for (int i = 0; i < points.Count; ++i)
        {
            Vector3 position = points[i];
            roomVectors.Add(position);
            triangulationData.Add(new Vertex(new Vector2(position.x, position.z), i));
        }

        // 삼각분할 수행 (DelaunayTriangulation 클래스는 정의되어 있어야 함)
        Triangulation triangulation = new Triangulation(triangulationData);

        foreach (Triangle triangle in triangulation.triangles)
        {
            indices.Add(triangle.vertex0.index);
            indices.Add(triangle.vertex1.index);
            indices.Add(triangle.vertex2.index);

            // 삼각형의 각 변을 Edge로 저장
            edges.Add(triangle.edge0);
            edges.Add(triangle.edge1);
            edges.Add(triangle.edge2);
        }
    }

    // 최소 신장 트리 생성 함수 (크루스칼 알고리즘)
    public void CreateMST()
    {
        gizmosCount = 1;
        edges.Sort((e1, e2) => e1.Length().CompareTo(e2.Length()));

        UnionFind uf = new UnionFind(points.Count);

        foreach (Edge edge in edges)
        {
            int vertexIndex0 = edge.point0.index;
            int vertexIndex1 = edge.point1.index;

            // 사이클이 생기지 않도록 두 정점을 Union-Find로 관리
            if (uf.Find(vertexIndex0) != uf.Find(vertexIndex1))
            {
                mstEdges.Add(edge);
                uf.Union(vertexIndex0, vertexIndex1);
            }
            else
            {
                // MST에서 제외된 엣지 저장
                excludedEdges.Add(edge);
            }

            // MST가 완성되면 종료 (간선 개수는 정점 - 1)
            if (mstEdges.Count == points.Count - 1)
                break;
        }

        // 끝부분 방(리프 노드) 탐색
        FindLeafRooms();
    }

    // 랜덤으로 제외된 엣지를 추가하여 사이클을 생성
    public void AddRandomEdgesToCreateCycle(int cycleEdgeCount)
    {
        gizmosCount = 2;

        // 랜덤으로 제외된 엣지들 중에서 cycleEdgeCount만큼 선택하여 추가
        System.Random rand = new System.Random();
        for (int i = 0; i < cycleEdgeCount && excludedEdges.Count > 0; i++)
        {
            int randomIndex = rand.Next(excludedEdges.Count);
            Edge edgeToAdd = excludedEdges[randomIndex];

            // MST에 엣지 추가
            mstEdges.Add(edgeToAdd);

            // 추가된 엣지는 제외된 엣지 리스트에서 삭제
            excludedEdges.RemoveAt(randomIndex);
        }
    }

    // 끝부분 방(리프 노드) 탐색
    public void FindLeafRooms()
    {
        Dictionary<int, int> connectionCounts = new Dictionary<int, int>();

        // 각 방이 몇 개의 간선에 연결되어 있는지 계산
        foreach (Edge edge in mstEdges)
        {
            if (!connectionCounts.ContainsKey(edge.point0.index))
                connectionCounts[edge.point0.index] = 0;
            if (!connectionCounts.ContainsKey(edge.point1.index))
                connectionCounts[edge.point1.index] = 0;

            connectionCounts[edge.point0.index]++;
            connectionCounts[edge.point1.index]++;
        }

        // 연결된 간선이 1개인 방을 리프 노드로 간주
        foreach (var entry in connectionCounts)
        {
            if (entry.Value == 1)
            {
                leafRooms.Add(entry.Key); // 리프 노드 저장
            }
        }

        // 리프 노드 출력
        foreach (int room in leafRooms)
        {
            Debug.Log($"Leaf Room (End of MST): {room}");
        }
    }

    // 기즈모로 삼각형 및 MST 시각화
    private void OnDrawGizmos()
    {
        if (roomVectors == null)
        {
            return;
        }

        Gizmos.color = Color.green;

        switch (gizmosCount)
        {
            case 0:
                for (int i = 0; i < indices.Count && indices != null; i += 3)
                {
                    Vector3 vertex0 = roomVectors[indices[i]];
                    Vector3 vertex1 = roomVectors[indices[i + 1]];
                    Vector3 vertex2 = roomVectors[indices[i + 2]];

                    Gizmos.DrawLine(vertex0, vertex1);
                    Gizmos.DrawLine(vertex1, vertex2);
                    Gizmos.DrawLine(vertex2, vertex0);
                }
                break;
            case 1:
                // 최소 신장 트리 그리기
                Gizmos.color = Color.yellow;
                if (mstEdges == null) return;
                foreach (Edge edge in mstEdges)
                {
                    Vector3 vertex0 = roomVectors[edge.point0.index];
                    Vector3 vertex1 = roomVectors[edge.point1.index];
                    Gizmos.DrawLine(vertex0, vertex1);
                }

                // 리프 노드 시각화
                Gizmos.color = Color.blue;
                foreach (int leafRoomIndex in leafRooms)
                {
                    Vector3 leafPosition = roomVectors[leafRoomIndex];
                    Gizmos.DrawSphere(leafPosition, 0.3f); // 리프 노드의 위치에 파란색 구 표시
                }
                break;
            case 2:
                // 사이클 포함된 MST 그리기
                Gizmos.color = Color.magenta; // 사이클을 포함하는 간선을 다른 색으로 표시
                if (mstEdges == null) return;
                foreach (Edge edge in mstEdges)
                {
                    Vector3 vertex0 = roomVectors[edge.point0.index];
                    Vector3 vertex1 = roomVectors[edge.point1.index];
                    Gizmos.DrawLine(vertex0, vertex1);
                }

                // 리프 노드 시각화
                Gizmos.color = Color.blue;
                foreach (int leafRoomIndex in leafRooms)
                {
                    Vector3 leafPosition = roomVectors[leafRoomIndex];
                    Gizmos.DrawSphere(leafPosition, 0.3f); // 리프 노드의 위치에 파란색 구 표시
                }
                break;
        }

        // 정점 시각화
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i], 0.2f);
        }
    }


// Union-Find 자료구조 (크루스칼 알고리즘에서 사용)
public class UnionFind
    {
        private int[] parent, rank;

        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];
            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }
        }

        public int Find(int i)
        {
            if (parent[i] != i)
            {
                parent[i] = Find(parent[i]);  // 경로 압축
            }
            return parent[i];
        }

        public void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);

            if (rootX != rootY)
            {
                if (rank[rootX] > rank[rootY])
                {
                    parent[rootY] = rootX;
                }
                else if (rank[rootX] < rank[rootY])
                {
                    parent[rootX] = rootY;
                }
                else
                {
                    parent[rootY] = rootX;
                    rank[rootX]++;
                }
            }
        }
    }

    public class Vertex
    {
        private Vector2 m_Position;
        public Vector2 position
        {
            get
            {
                return m_Position;
            }
        }

        private int m_Index;
        public int index
        {
            get
            {
                return m_Index;
            }
        }

        public Vertex(Vector2 position, int index)
        {
            m_Position = position;
            m_Index = index;
        }

        public override bool Equals(object obj)
        {
            Vertex other = obj as Vertex;

            if (other == null)
            {
                return false;
            }

            if (other.index != m_Index)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return m_Index.GetHashCode();
        }

        public Vertex Clone()
        {
            return new Vertex(m_Position, index);
        }
    }

public class Edge
    {
        private float m_Length;
        private float W;
        public float length
        {
            get
            {
                return m_Length;
            }
        }

        private Vertex m_Point0;
        public Vertex point0
        {
            get
            {
                return m_Point0;
            }
        }

        private Vertex m_Point1;
        public Vertex point1
        {
            get
            {
                return m_Point1;
            }
        }

       
        public Edge(Vertex point0, Vertex point1)
        {
            m_Point0 = point0;
            m_Point1 = point1;
            m_Length = (point1.position - point0.position).magnitude;
        }
        public int CompareTo(Edge other)
        {
            return this.m_Length.CompareTo(other.m_Length);
        }
        public float Length()
        {
            return m_Length;
        }

        public override bool Equals(object obj)
        {
            Edge other = obj as Edge;

            if (other != null)
            {
                // Check if the two first points overlap
                bool isSame = other.point0.Equals(point0) && other.point1.Equals(point1);

                // Check if the points overlap in cross
                isSame |= other.point1.Equals(point0) && other.point0.Equals(point1);

                return isSame;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return m_Point0.GetHashCode() + 31 * m_Point1.GetHashCode();
        }
    }

    public class Triangle
    {
        private Vertex m_Vertex0, m_Vertex1, m_Vertex2;
        private Edge m_Edge0, m_Edge1, m_Edge2;

        private Vector2 m_CircumcircleCenter;
        private Vector2 circumcircleCenter
        {
            get
            {
                if (m_CircumcircleCenter == null)
                {
                    m_CircumcircleCenter = new Vector2();
                }
                return m_CircumcircleCenter;
            }
            set
            {
                m_CircumcircleCenter = value;
            }
        }

        private float m_CircumcircleRadius;
        private float circumcircleRadius
        {
            get
            {
                return m_CircumcircleRadius;
            }

            set
            {
                m_CircumcircleRadius = value;
            }
        }

        public Vertex vertex0
        {
            get
            {
                return m_Vertex0;
            }
        }

        public Vertex vertex1
        {
            get
            {
                return m_Vertex1;
            }
        }

        public Vertex vertex2
        {
            get
            {
                return m_Vertex2;
            }
        }

        public Edge edge0
        {
            get
            {
                return m_Edge0;
            }
        }

        public Edge edge1
        {
            get
            {
                return m_Edge1;
            }
        }

        public Edge edge2
        {
            get
            {
                return m_Edge2;
            }
        }

        public Triangle(Vertex p0, Vertex p1, Vertex p2, bool clockwise)
        {
            List<Vertex> inputPoints = new List<Vertex>();

            inputPoints.Add(p0);
            inputPoints.Add(p1);
            inputPoints.Add(p2);

            inputPoints = inputPoints.OrderBy(x => x.position.x).ToList();

            m_Vertex0 = inputPoints[0].Clone();

            Vector2 up = inputPoints[2].position - inputPoints[0].position;
            up = new Vector2(-up.y, up.x);

            float distanceToPlane = Vector2.Dot(up, (inputPoints[1].position - inputPoints[0].position));

            int clockWiseShift = clockwise ? 0 : 1;

            if (distanceToPlane > 0f)
            {
                m_Vertex1 = inputPoints[1 + clockWiseShift].Clone();
                m_Vertex2 = inputPoints[2 - clockWiseShift].Clone();
            }
            else
            {
                m_Vertex1 = inputPoints[2 - clockWiseShift].Clone();
                m_Vertex2 = inputPoints[1 + clockWiseShift].Clone();
            }

            m_Edge0 = new Edge(m_Vertex0, m_Vertex1);
            m_Edge1 = new Edge(m_Vertex1, m_Vertex2);
            m_Edge2 = new Edge(m_Vertex2, m_Vertex0);

            float len0Square = (vertex0.position.x * vertex0.position.x) + (vertex0.position.y * vertex0.position.y);
            float len1Square = (vertex1.position.x * vertex1.position.x) + (vertex1.position.y * vertex1.position.y);
            float len2Square = (vertex2.position.x * vertex2.position.x) + (vertex2.position.y * vertex2.position.y);

            // Compute the circumcircle of the triangle.
            // TODO: Find better solution for this.
            Vector2 circleCenter = new Vector2();

            circleCenter.x = (len0Square * (vertex2.position.y - vertex1.position.y) + len1Square * (vertex0.position.y - vertex2.position.y) + len2Square * (vertex1.position.y - vertex0.position.y)) / (vertex0.position.x * (vertex2.position.y - vertex1.position.y) + vertex1.position.x * (vertex0.position.y - vertex2.position.y) + vertex2.position.x * (vertex1.position.y - vertex0.position.y)) / 2f;
            circleCenter.y = (len0Square * (vertex2.position.x - vertex1.position.x) + len1Square * (vertex0.position.x - vertex2.position.x) + len2Square * (vertex1.position.x - vertex0.position.x)) / (vertex0.position.y * (vertex2.position.x - vertex1.position.x) + vertex1.position.y * (vertex0.position.x - vertex2.position.x) + vertex2.position.y * (vertex1.position.x - vertex0.position.x)) / 2f;

            m_CircumcircleCenter = circleCenter;

            circumcircleRadius = Mathf.Sqrt(((vertex1.position.x - circumcircleCenter.x) * (vertex1.position.x - circumcircleCenter.x)) + ((vertex1.position.y - circumcircleCenter.y) * (vertex1.position.y - circumcircleCenter.y)));
        }

        public Triangle(Vertex p0, Vertex p1, Vertex p2) : this(p0, p1, p2, true)
        {
            // Do nothing special for defualt case
        }

        public bool PointInCurcumcircle(Vector2 vertex)
        {
            float xDiff = (vertex.x - circumcircleCenter.x);
            float yDiff = (vertex.y - circumcircleCenter.y);
            float distance = Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);

            return distance <= circumcircleRadius;
        }

        public bool PointInCurcumcircle(Vertex vertex)
        {
            bool isInCircumcircle = PointInCurcumcircle(vertex.position);

            return isInCircumcircle;
        }

        public bool Contains(Vertex vertex)
        {
            bool contains = vertex.Equals(vertex0);
            contains |= vertex.Equals(vertex1);
            contains |= vertex.Equals(vertex2);

            return contains;
        }

        public bool Contains(Edge edge)
        {
            bool contains = edge.Equals(edge0);
            contains |= edge.Equals(edge1);
            contains |= edge.Equals(edge2);

            return contains;
        }

        public List<Vertex> GetOverlappingSet(Triangle other)
        {
            List<Vertex> overlap = new List<Vertex>();

            if (other.Contains(vertex0))
            {
                overlap.Add(vertex0);
            }

            if (other.Contains(vertex1))
            {
                overlap.Add(vertex1);
            }

            if (other.Contains(vertex2))
            {
                overlap.Add(vertex2);
            }

            return overlap;
        }

        public override bool Equals(object obj)
        {
            Triangle other = obj as Triangle;

            if (other == null)
            {
                return false;
            }

            bool isSame = m_Vertex0.Equals(other.vertex0) ||
                m_Vertex0.Equals(other.vertex1) || m_Vertex0.Equals(other.vertex2);

            isSame &= m_Vertex1.Equals(other.vertex0) ||
                m_Vertex1.Equals(other.vertex1) || m_Vertex1.Equals(other.vertex2);

            isSame &= m_Vertex2.Equals(other.vertex0) ||
                m_Vertex2.Equals(other.vertex1) || m_Vertex2.Equals(other.vertex2);

            return isSame;
        }

        public override int GetHashCode()
        {
            int hash0 = vertex0.index.GetHashCode();
            int hash1 = vertex1.index.GetHashCode();
            int hash2 = vertex2.index.GetHashCode();
            int combined = ((hash0 << 3) + hash0 ^ hash1) << 5;
            combined += combined ^ hash2;

            return combined;
        }

    }

    public class Triangulation
    {
        private List<Triangle> m_Trinagles;
        public List<Triangle> triangles
        {
            get
            {
                return m_Trinagles;
            }
        }

        private Rect m_BoundingBox;
        private Rect boundingBox
        {
            get
            {
                if (m_BoundingBox == null)
                {
                    m_BoundingBox = new Rect(0, 0, 0, 0);
                }
                return m_BoundingBox;
            }
            set
            {
                m_BoundingBox = value;
            }
        }

        public Triangulation()
        {
            m_Trinagles = new List<Triangle>();
        }

        // Make triangulation from set of triangled, will not actually
        // triangulate, but only store tringles and allow insertion of points
        public Triangulation(List<Triangle> triangles)
        {

            m_Trinagles = triangles;

            // If there are triangles, create a bounding box
            if (m_Trinagles.Count > 0)
            {
                Vertex v = m_Trinagles[0].vertex0;

                float firstX = v.position.x;
                float firstY = v.position.y;

                Rect bBox = new Rect(firstX, firstY, firstX, firstY);

                foreach (Triangle triangle in m_Trinagles)
                {
                    List<Vertex> triangleVertecies = new List<Vertex>();

                    triangleVertecies.Add(triangle.vertex0);
                    triangleVertecies.Add(triangle.vertex1);
                    triangleVertecies.Add(triangle.vertex2);

                    foreach (Vertex vertex in triangleVertecies)
                    {
                        bBox.x = Mathf.Min(boundingBox.x, vertex.position.x);
                        bBox.y = Mathf.Min(boundingBox.y, vertex.position.y);
                        bBox.width = Mathf.Max(boundingBox.width, vertex.position.x);
                        bBox.height = Mathf.Max(boundingBox.height, vertex.position.y);
                    }
                }

                boundingBox = bBox;
            }
        }

        // Make triangulation from set of points
        public Triangulation(List<Vertex> points) : this()
        {
            if (points.Count < 3)
            {
                return;
            }

            // Start with empty triangulation
            List<Triangle> triangulation = new List<Triangle>();

            float firstX = points[0].position.x;
            float firstY = points[0].position.y;

            Rect bBox = new Rect(firstX, firstY, firstX, firstY);

            foreach (Vertex vertex in points)
            {
                bBox.x = Mathf.Min(boundingBox.x, vertex.position.x);
                bBox.y = Mathf.Min(boundingBox.y, vertex.position.y);
                bBox.width = Mathf.Max(boundingBox.width, vertex.position.x);
                bBox.height = Mathf.Max(boundingBox.height, vertex.position.y);
            }

            boundingBox = bBox;

            float superWidth = (boundingBox.size.x - boundingBox.position.x) * 3f + 1f;
            float superHeight = (boundingBox.size.y - boundingBox.position.y) * 3f + 1f;

            // Make super triangle
            Vertex super0 = new Vertex(boundingBox.position - new Vector2(10f, 12f) * 100f, -1);
            Vertex super1 = new Vertex(new Vector2(boundingBox.position.x + superWidth * 100f, boundingBox.position.y * 100f - .95f), -2);
            Vertex super2 = new Vertex(new Vector2(boundingBox.position.x - .95f, boundingBox.position.y + superHeight * 100f), -3);

            Triangle super = new Triangle(super0, super1, super2);

            triangulation.Add(super);

            // Iterate over each point, insert and triangulate
            foreach (Vertex vertex in points)
            {
                // Triangles invalidated at vertex insertion
                List<Triangle> badTriangles = new List<Triangle>();

                // List of edges that should be removed
                List<Edge> polygon = new List<Edge>();

                // List of edges that are incorrectly marked for rmoval
                List<Edge> badEdges = new List<Edge>();

                // Find all invalid triangles
                foreach (Triangle triangle in triangulation)
                {
                    // If the point is in the circumcircle in
                    // a trinagle, add it to the bad set
                    if (triangle.PointInCurcumcircle(vertex))
                    {
                        // Add the triangle and the edges
                        badTriangles.Add(triangle);
                        polygon.Add(triangle.edge0);
                        polygon.Add(triangle.edge1);
                        polygon.Add(triangle.edge2);
                    }
                }

                // Remove bad trinagles from current triangulation
                triangulation.RemoveAll(x => badTriangles.Contains(x));

                // Identify edges in the polygon that are shared
                // among multiple triangles
                for (int i = 0; i < polygon.Count; ++i)
                {
                    for (int j = 0; j < polygon.Count; ++j)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        // If an edge is shared with another triangle
                        // it is removed from the list and will not be
                        // retriangulated.
                        if (polygon[i].Equals(polygon[j]))
                        {
                            badEdges.Add(polygon[i]);
                            badEdges.Add(polygon[j]);
                        }
                    }
                }

                // Remove shared edges from the polygon
                polygon.RemoveAll(x => badEdges.Contains(x));

                // Retriangulate the empty polygonal space
                foreach (Edge edge in polygon)
                {
                    triangulation.Add(new Triangle(edge.point0, edge.point1, vertex));
                }
            }

            // Cleanup from the super trinagle
            for (int i = triangulation.Count - 1; i >= 0; --i)
            {
                // Check if any point is part of the super triangle
                bool isSuper = triangulation[i].Contains(super0);
                isSuper |= triangulation[i].Contains(super1);
                isSuper |= triangulation[i].Contains(super2);

                // If so, remove triangle from triangulation
                if (isSuper)
                {
                    triangulation.RemoveAt(i);
                }
            }

            m_Trinagles = triangulation;
        }

        // Used for adding an internal vertex
        // used only when the vertex falls within the triangulation
        public void AddInternal(Vertex vertex)
        {
            // Triangles invalidated at vertex insertion
            List<Triangle> badTriangles = new List<Triangle>();

            // List of edges that should be removed
            List<Edge> polygon = new List<Edge>();

            // List of edges that are incorrectly marked for rmoval
            List<Edge> badEdges = new List<Edge>();

            // Find all invalid triangles
            foreach (Triangle triangle in m_Trinagles)
            {
                // If the point is in the circumcircle in
                // a trinagle, add it to the bad set
                if (triangle.PointInCurcumcircle(vertex))
                {
                    // Add the triangle and the edges
                    badTriangles.Add(triangle);
                    polygon.Add(triangle.edge0);
                    polygon.Add(triangle.edge1);
                    polygon.Add(triangle.edge2);
                }
            }

            // Remove bad trinagles from current triangulation
            m_Trinagles.RemoveAll(x => badTriangles.Contains(x));

            // Identify edges in the polygon that are shared
            // among multiple triangles
            for (int i = 0; i < polygon.Count; ++i)
            {
                for (int j = 0; j < polygon.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    // If an edge is shared with another triangle
                    // it is removed from the list and will not be
                    // retriangulated.
                    if (polygon[i].Equals(polygon[j]))
                    {
                        badEdges.Add(polygon[i]);
                        badEdges.Add(polygon[j]);
                    }
                }
            }

            // Remove shared edges from the polygon
            polygon.RemoveAll(x => badEdges.Contains(x));

            // Retriangulate the empty polygonal space
            foreach (Edge edge in polygon)
            {
                m_Trinagles.Add(new Triangle(edge.point0, edge.point1, vertex));
            }
        }
    }
}







