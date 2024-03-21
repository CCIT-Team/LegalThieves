using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public BoundsInt bounds;
    public List<Vector3> doors = new List<Vector3>();

    public Room(Vector3Int location, Vector3Int size)
    {
        bounds = new BoundsInt(location, size);
    }

    public void setDoors()
    {
        doors.Add(new Vector3(bounds.xMin, 0, bounds.center.z));
        doors.Add(new Vector3(bounds.xMax, 0, bounds.center.z));
        doors.Add(new Vector3(bounds.center.x, 0, bounds.zMin));
        doors.Add(new Vector3(bounds.center.x, 0, bounds.zMax));
    }
}
