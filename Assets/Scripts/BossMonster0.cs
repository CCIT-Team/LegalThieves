using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster0 : MonsterBase
{
    
    // Start is called before the first frame update
    void Start()
    {

       Init();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Monster0Move());
       
    }

    IEnumerator Monster0Move() // �Ĵٺ��� ���ߴ� �ʰ�� ����
    {
        while (true)
        {
            // ���� �÷��̾��� ȭ�鿡 ���Դ��� Ȯ��
            Vector3 screenPoint = _camera.WorldToViewportPoint(transform.position);
            
            if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0)
            {
                //ȭ�鿡 ������ ����
                Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), player.position - transform.position, out RaycastHit hit, Mathf.Infinity);
                //  ���� �÷��̾� ���̿� ���� ���� ��쿡 ����

                if (hit.collider.gameObject.CompareTag("Player"))
                {

                    agent.SetDestination(transform.position);
                }
                else
                {
                    //�ٽ� �Ⱥ��� ��� �Ѿƿ�
                    agent.SetDestination(player.position);
                }
            }
            else
            {
                // ȭ�鿡 ������ ������ �÷��̾� ������ �̵�
                agent.SetDestination(player.position);
            }
            yield return new WaitForSeconds(.3f);
        }
    }
}
