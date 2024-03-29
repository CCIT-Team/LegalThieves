using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputHandler _inputHandler;
    Rigidbody _rigi;

    public Camera cameraObject;
    public float movementSpeed = 5f;
    float mouseSpeed = 5f;

    Vector3 moveDirection;
    Vector3 normalVector;


    float mouseX;
    float mouseY;

    private void Start()
    {
        _rigi = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<InputHandler>();
    }

    public void Update()
    {
        var delta = Time.deltaTime;

        _inputHandler.TickInput(delta);

        CameraRotation(delta);

        moveDirection = cameraObject.transform.forward * _inputHandler.vertical;
        moveDirection += cameraObject.transform.right * _inputHandler.horizontal;
        moveDirection.y = 0;
        moveDirection.Normalize();

        float speed = movementSpeed;
        moveDirection *= speed;

        Vector3 porjectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        _rigi.velocity = porjectedVelocity;
        transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }

    private void CameraRotation(float delta)
    {
        mouseX += _inputHandler.mouseX * mouseSpeed * delta;
        mouseY -= _inputHandler.mouseY * mouseSpeed * delta;

        mouseY = Mathf.Clamp(mouseY, -90, 90);

        Vector3 cameraPosition = transform.position;
        cameraPosition.y += 0.5f;
        cameraObject.transform.position = cameraPosition;
        cameraObject.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}
