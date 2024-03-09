using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class InventoryGenerator : MonoBehaviour
{
    [SerializeField] private GameObject menutilePrefab;
    [SerializeField] private RectTransform menutileParent;
    public int cellSize;
    public int x;
    public int y;

    public GameObject[,] InventoryPanel;

    // Start is called before the first frame update
    void Start()

    {
        InventoryPanel = new GameObject[x,y];
   
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                InventoryPanel[i,j] = Instantiate(menutilePrefab,menutileParent.anchoredPosition + new Vector2(i,j) * cellSize  , Quaternion.identity, menutileParent);
            
            }
        }
      
    }
   
}
