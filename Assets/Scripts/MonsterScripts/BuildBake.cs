using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class BuildBake : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh(); //bake 생성
    }

    
}
