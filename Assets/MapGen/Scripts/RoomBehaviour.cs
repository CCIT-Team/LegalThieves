using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    //    public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
    //    public GameObject[] doors;

    //public void UpdateRoom(bool[] status)
    //{
    //    for (int i = 0; i < status.Length; i++)
    //    {
    //        doors[i].SetActive(status[i]);
    //        walls[i].SetActive(!status[i]);
    //    }
    //}
    // Start is called before the first frame update
    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Empty")
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}