using UnityEngine;

[CreateAssetMenu(menuName = "Item/New Item")]
public class RelicData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public int id;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;
}
