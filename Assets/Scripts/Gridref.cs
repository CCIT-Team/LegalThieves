using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Grid : MonoBehaviour
{
    // Start is called before the first frame update
    
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;

   

    public Grid(int width, int height, float cellSize){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width,height];
        debugTextArray = new TextMesh[width, height];

        for(int x=0; x < gridArray.GetLength(0);x++){
            for (int y=0; y<gridArray.GetLength(1);y++){
                debugTextArray[x,y] = CreateWorldText(gridArray[x,y].ToString(), GameObject.Find("Grid").transform, GetWorldPosition(x,y) + new Vector3(cellSize, cellSize)/2, 30, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x,y+1),Color.white,100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1,y),Color.white,100f);
                SetValue( x,y);
            }
        }
        Debug.DrawLine(GetWorldPosition(width,0), GetWorldPosition(width,height),Color.white,100f);
        Debug.DrawLine(GetWorldPosition(0,height), GetWorldPosition(width,height),Color.white,100f);
        
        

    }

    public TextMesh CreateWorldText ( string text, Transform parent , Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor,TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000){
        GameObject gameObject = new("World_Text", typeof(TextMesh));
                Transform transform = gameObject.transform;
                transform.SetParent(parent,false);
                transform.localPosition = localPosition;
                TextMesh textMesh = gameObject.GetComponent<TextMesh>();
                textMesh.anchor = textAnchor;
                textMesh.alignment = textAlignment;
                textMesh.text = text;
                textMesh.fontSize = fontSize;
                textMesh.color = color;
                textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
                return textMesh;
    }
    private Vector3 GetWorldPosition(int x, int y ){
        return new Vector3(x,y) * cellSize;
    }

    public void SetValue(int x, int y){
        if(x>=0&&y>=0 && x<width && y<height){
        
        debugTextArray[x,y].text = x+","+y;
        }
    }
    
}
