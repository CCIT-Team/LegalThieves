using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [Networked, Capacity(4)] public NetworkArray<NetworkObject> PlayerList => default;


    [SerializeField]
    private GameObject playerPrefab;
    private int playerIndex = 0;
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void SpawnPlayer()
    {
       
            // �������� ���͸� �����ϰ� ��� Ŭ���̾�Ʈ�� ����ȭ
       
            PlayerList.Set(playerIndex++, Runner.Spawn(playerPrefab, new Vector3(0, 5, 0), Quaternion.identity));
        
    }

   

    public void CreatePlayerManager()
    {
     
        
            if (null == instance)
            {
                instance = this;
             
            }
            else
            {
                Destroy(this.gameObject);
            }
        
    }
}
