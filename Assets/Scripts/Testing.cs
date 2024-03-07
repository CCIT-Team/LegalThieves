using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private GameObject menutilePrefab;
    [SerializeField] private Transform menutileParent;
    [SerializeField] private float cellSize;
    [SerializeField] private int x;
    [SerializeField] private int y;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
            Instantiate(menutilePrefab,menutileParent.position + new Vector3(i,j) * cellSize + new Vector3(cellSize, cellSize)/2 , Quaternion.identity, menutileParent);
            }
        }
    }

}
