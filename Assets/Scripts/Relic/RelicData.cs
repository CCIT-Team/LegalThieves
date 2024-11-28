using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicData", menuName = "Scriptable Object/Relic", order = int.MaxValue)]
public class RelicData : ScriptableObject
{
    [SerializeField]
    MeshFilter meshFilter;

    [SerializeField]
    Mesh mesh;
}
