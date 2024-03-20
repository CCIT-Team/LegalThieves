using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    protected Camera _camera; // ���� ī�޶�
    protected Transform player;
    enum MosterType { zombie, mira,scp } //���� Ÿ�� ����
    [SerializeField] MosterType monsterType;

    [SerializeField] float speed;
    [SerializeField] int health;
    
    public NavMeshAgent agent;
    

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

    }
    // Start is called before the first frame update
    
    public void init() //enum ���� ���� �ʱ�ȭ�ϴ� �Լ�
    {
        switch (monsterType)
        {
            case MosterType.zombie:
                health = 1;
                speed = 7;
                break;

            case MosterType.mira:
                health = 5;
                speed = 3;
                break;

            case MosterType.scp:
                health = 10;
                speed = 5;
                break;
        }
    }
}
