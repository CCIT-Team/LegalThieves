using System;
using UnityEngine;

namespace New_Neo_LT.Scripts.Utilities
{
    public class RelicGizmo : MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = Color.blue;
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(transform.position, new Vector3(1, 100, 1));
        }

        private void Start()
        {
            Destroy(this);
        }
    }
}