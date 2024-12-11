using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESKillType
{
    Null = -1,
    Torch,
    Flashlight,
    Count
}

public abstract class SkillBase : MonoBehaviour
{
        public virtual void UseSkill(){}
}
