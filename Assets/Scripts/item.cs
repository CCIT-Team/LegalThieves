using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;


public class Item : MonoBehaviour
{
    public enum Type { Artifact, ArtifactLocation };
    public Type type;
    public float price;  // ������ ����
    public int year;   // ������ �߰ߵ� �⵵
    public float weight;  // ������ ����



    void Update()
    {

    }
}
