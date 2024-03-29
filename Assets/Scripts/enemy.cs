using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        HealthGauge.health -= 10f;
    }
}
