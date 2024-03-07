using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    [SerializeField]
    ROOMTYPE roomType;

    [SerializeField]
    List<Transform> doorTransforms;

    private bool isvisited = false;

    public GameObject debugObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckRoom()
    {
        if(!isvisited)
        {
            isvisited = true;
            for (int i = 0; i < doorTransforms.Count; i++)
            {

            }
        }
    }
}

public enum ROOMTYPE { None = -1,Room, Path}
