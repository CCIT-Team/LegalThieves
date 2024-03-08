using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class InventoryGenerator : MonoBehaviour
{
    [SerializeField] private GameObject menutilePrefab;
    [SerializeField] private Transform menutileParent;
    public int cellSize;
    [SerializeField] private int x;
    [SerializeField] private int y;
    
    public GameObject[] InventoryPanel = new GameObject[45];
    
    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
            InventoryPanel[index++] = Instantiate(menutilePrefab,menutileParent.position + new Vector3(i,j) * cellSize  , Quaternion.identity, menutileParent);
            
            }
        }
      
    }
   
}
