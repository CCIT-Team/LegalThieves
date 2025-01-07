using New_Neo_LT.Scripts.PlayerComponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player))
        {
            Debug.Log(other.gameObject.name + "'s points Down");
            player.AddGoldPoint(-300);
            player.AddRenownPoint(-300);
            gameObject.SetActive(false);
        }
    }
}
