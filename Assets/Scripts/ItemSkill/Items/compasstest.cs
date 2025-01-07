using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class compasstest : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public Transform Entrans;
    Mesh pathMesh;
    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKey(KeyCode.V))
            PathFinding();
    }

    private void PathFinding()
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, Entrans.position, NavMesh.AllAreas, path))
        {
            // 경로가 유효하면 Mesh 생성
            UpdatePathMesh(path);
        }

    }
    float lineLength;

    void UpdatePathMesh(NavMeshPath path)
    {
        lineRenderer.positionCount = path.corners.Length;

        lineRenderer.SetPositions(path.corners);

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            var temp =  Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
            lineLength += temp;

            
        }

        var average = lineLength /  lineRenderer.positionCount;
        var normal = average / lineLength;
        lineRenderer.textureScale = new Vector2(1-normal,1);
    }
}