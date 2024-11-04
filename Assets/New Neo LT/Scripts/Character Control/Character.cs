using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

namespace New_Neo_LT.Scripts
{
    public enum KCCProcessorType
    {
        None = -1,
        Movement = 0,
        Jumping,
        Sprinting,
        Crouching,
        ProcesserTypeCount
    }
    
    [RequireComponent(typeof(NetworkObject)), RequireComponent(typeof(KCC)), RequireComponent(typeof(Rigidbody))]
    public class Character : NetworkBehaviour
    { 
       [Header("Character Components")]
       [SerializeField] public    KCC              kcc;
       [SerializeField] public    CharacterStats   characterStats;
       [SerializeField] protected KCCProcessor[]   kccProcessors;
       [SerializeField] protected Animator         animator;
       
       /*-------------------------------------------------------------------------------------------------------------*/
       
       protected void InitializeCharacterComponents()
       {
           kcc            ??= GetComponent<KCC>();
           kccProcessors  ??= GetComponentsInChildren<KCCProcessor>();
           animator       ??= GetComponentInChildren<Animator>();
           characterStats ??= GetComponent<CharacterStats>();
           characterStats.InitializeStats();
       }
       
       public void Teleport(Vector3 position, Quaternion rotation = default)
       {
           kcc.SetPosition(position);
           if(rotation != default)
               kcc.SetLookRotation(rotation);
       }
    }
}
