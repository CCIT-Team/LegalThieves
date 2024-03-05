using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{

    const float tileSizeWidth = 32;
    const float tileSizeHeight = 32;

    // Start is called before the first frame update
    RectTransform rectTransform;
    void Start()
    {
        rectTransform=GetComponent<RectTransform>();

    }

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    public Vector2Int GetGridPosition(Vector2 mousePosition){
        positionOnTheGrid.x = mousePosition.x -rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
       
        tileGridPosition.x = (int)(positionOnTheGrid.x/tileSizeWidth);
        
        tileGridPosition.y = (int)(positionOnTheGrid.x/tileSizeHeight);
        return tileGridPosition;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
