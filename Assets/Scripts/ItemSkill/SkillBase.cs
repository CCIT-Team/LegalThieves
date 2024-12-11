using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] string skillName;
    [SerializeField] Sprite skillIcon;
    [SerializeField] bool IsEnable;
    [SerializeField] AudioSource skillSFX;

        public virtual void UseSkill(){}
}
