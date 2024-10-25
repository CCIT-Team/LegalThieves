using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class AstarPath : MonoBehaviour
{
    Node[,] grid;
    int cellSize;
    Vector2Int areaSize;
    public AstarPath(Vector2Int _areaSize, int _cellSize )
    {
        areaSize = _areaSize;
        grid = new Node[_areaSize.x, _areaSize.y];
        _cellSize = cellSize;
    }

    public void SetNode(List<Vector3> _points )
    {
        for (int y = 0; y < grid.GetLength(0); y++) // За
        {
            for (int x = 0; x < grid.GetLength(1); x++) // ї­
            {
     
            }
          
        }
    }
}
public class Node
{
    Vector2Int nodePos;
    float weight;
    float g;
    float h;
    Node(Vector2Int pos) 
    {
    nodePos = pos; 
    }

    public void SetWeight(float w)
    {
        weight = w;
    }
    public float GetWeight() {
        return weight;
    }
    public void SetG(Vector2Int startPos ) {
        float fg = Vector2Int.Distance(nodePos, startPos);
        g = fg;
    }

    public void SetH(Vector2Int goalPos)
    {
        float fh = Vector2Int.Distance(nodePos, goalPos);
        h = fh;
    }
    public void CalculateWeight(Node a, Vector2Int startPos, Vector2Int goalPos)
    {
        SetG(startPos);
        SetH(goalPos);
        weight = g + h;
    }
   
}

