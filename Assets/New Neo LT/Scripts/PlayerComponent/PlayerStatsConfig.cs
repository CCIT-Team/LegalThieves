using UnityEngine;

namespace New_Neo_LT.Scripts
{
    [CreateAssetMenu(fileName = "PlayerStatsConfig", menuName = "Player Stats Config")]
    public class PlayerStatsConfig : StatConfig
    {
        [Header("Player Stats")]
        [SerializeField] public float   maxPitch;
        [SerializeField] public float   lookSensitivity;
        [SerializeField] public Vector3 jumpImpulse;
        [SerializeField] public float   interactionRange;
    }
    
    public class StatConfig : ScriptableObject
    {
        [Header("Character Stats")]
        [SerializeField] public int   maxHealth;
        [SerializeField] public int   maxStamina;
        [SerializeField] public float moveSpeed;
        [SerializeField] public float sprintSpeed;
        [SerializeField] public float crouchMoveSpeed;
        [SerializeField] public float crouchingSpeed;
        [SerializeField] public float speedMultiplier = 1f;
    }
}