using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputHandler inputHandler;
    Rigidbody rigi;

    public Camera cameraObject;
    public float movementSpeed = 5f;
    float mouseSpeed = 5f;

    Vector3 moveDirection;
    Vector3 normalVector;


    float mouseX;
    float mouseY;

    private void Start()
    {
        rigi = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void Update()
    {
        float delta = Time.deltaTime;

        inputHandler.TickInput(delta);

        CameraRotation(delta);

        moveDirection = cameraObject.transform.forward * inputHandler.vertical;
        moveDirection += cameraObject.transform.right * inputHandler.horizontal;
        moveDirection.y = 0;
        moveDirection.Normalize();

        float speed = movementSpeed;
        moveDirection *= speed;

        Vector3 porjectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigi.velocity = porjectedVelocity;
        transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }

    private void CameraRotation(float delta)
    {
        mouseX += inputHandler.mouseX * mouseSpeed * delta;
        mouseY -= inputHandler.mouseY * mouseSpeed * delta;

        mouseY = Mathf.Clamp(mouseY, -90, 90);

        Vector3 cameraPosition = transform.position;
        cameraPosition.y += 0.5f;
        cameraObject.transform.position = cameraPosition;
        cameraObject.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}
