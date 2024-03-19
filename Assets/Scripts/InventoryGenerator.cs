using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryGenerator : MonoBehaviour
{
    [Header("Generator")]
    [SerializeField] private RectTransform menutileObject;
    public int cellSize;
    public int x;
    public int y;

    [Header("Item Create")]
    [SerializeField] private RectTransform itemList;
    [SerializeField] private RectTransform itemObject;

    public Vector2[,] InventoryTileArray;



    // Start is called before the first frame update

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CreateItem();

        }
    }
    /// <summary>
    /// 테스트용
    /// </summary>
    void CreateItem()
    {
        Instantiate(itemObject, itemList.position, Quaternion.identity, itemList);
    }
     
    //인벤토리 생성 후 배열에 담기
    void Start()

    {
        //아이템 리스트 위치를 인벤토리에 맞춤
        itemList.anchoredPosition = new Vector2(cellSize, cellSize) / 2;
        InventoryTileArray = new Vector2[x,y];
   
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                InventoryTileArray[i, j] = menutileObject.anchoredPosition + new Vector2(i, j) * cellSize ;
            }
        }
    }
   
}
