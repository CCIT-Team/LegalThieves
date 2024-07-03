using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRelic : MonoBehaviour
{
    [SerializeField]
    public int goldPoint = 0;
    [SerializeField]
    public int renownPoint = 0;
    int _roomID = -1;
    public int roomID
    {
        get { return _roomID; }
        set
        {
            if (_roomID == -1)
            {
                _roomID = value;
            }
        }
    }
}
