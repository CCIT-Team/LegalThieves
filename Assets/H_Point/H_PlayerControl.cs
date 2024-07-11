using Fusion;

using UnityEngine;


public class H_PlayerControl : NetworkBehaviour
{
    [SerializeField] CharacterController ch;
    public float playerSpeed;


    void Start()
    {
        ch = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0, v) * playerSpeed * Runner.DeltaTime;

        ch.Move(movement);
     
    }
}

