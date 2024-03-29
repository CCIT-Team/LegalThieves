using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;


public class Item : MonoBehaviour
{
    public enum Type { Artifact, ArtifactLocation };
    public Type type;
    public float price;  // 유물의 가격
    public int year;   // 유물이 발견된 년도
    public float weight;  // 유물의 무게



    void Update()
    {

    }
}
