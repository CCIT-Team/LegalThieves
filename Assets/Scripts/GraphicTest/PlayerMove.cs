using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [SerializeField] private float dragMultiply;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runMultiply;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform cameraHolder;

    private bool isRun;
    private bool isGround;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one PlayerMove! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        Rigidbody player = gameObject.GetComponent<Rigidbody>();
        Vector3 playerVelocity = player.velocity;
        float speedMultiply;

        isRun = Input.GetKey(KeyCode.LeftShift);

        if (isRun)
        {
            speedMultiply = runMultiply;
        }
        else
        {
            speedMultiply = 1f;
        }

        if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && isGround)
        {
            gameObject.GetComponent<Rigidbody>().drag = playerVelocity.magnitude * dragMultiply;
        }
        else gameObject.GetComponent<Rigidbody>().drag = 0f;

        if (Input.GetKey(KeyCode.W) && playerVelocity.magnitude < moveSpeed * speedMultiply)
        {
            player.AddForce(transform.forward * player.mass * moveSpeed * speedMultiply);
        }

        if (Input.GetKey(KeyCode.S) && playerVelocity.magnitude < moveSpeed * speedMultiply)
        {
            player.AddForce(- transform.forward * player.mass * moveSpeed * speedMultiply);
        }

        if (Input.GetKey(KeyCode.A) && playerVelocity.magnitude < moveSpeed * speedMultiply)
        {
            player.AddForce(- transform.right * player.mass * moveSpeed * speedMultiply);
        }

        if (Input.GetKey(KeyCode.D) && playerVelocity.magnitude < moveSpeed * speedMultiply)
        {
            player.AddForce(transform.right * player.mass * moveSpeed * speedMultiply);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            player.AddForce(Vector3.up * player.mass * jumpForce);
        }

        float rotationX = Input.GetAxis("Mouse Y") * rotationSpeed;
        float rotationY = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0f, rotationY, 0f);
        cameraHolder.Rotate(-rotationX, 0f, 0f);
    }

    private void OnCollisionEnter()
    {
        isGround = true;
    }

    private void OnCollisionExit()
    {
        isGround = false;
    }

    public float GetPlayerVelocity()
    {
        return GetComponent<Rigidbody>().velocity.magnitude;
    }
}
