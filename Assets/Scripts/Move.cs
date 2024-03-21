using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    public Vector3 inputVec;
    public float speed = 5f;

    Rigidbody rigid;
    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 moveDirection = inputVec;
        transform.position += Camera.main.transform.TransformDirection(inputVec) * speed * Time.deltaTime;
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector3>();
    }
}
