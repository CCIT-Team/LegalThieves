using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Ȱ�� �÷��̾� ���(NetworkRunner.ActivePlayers)�� �ݺ��Ͽ� ���� �� ���� �ݹ��� ȣ���ϴ� ���� Ŭ�����Դϴ�.
/// �� �÷��̾� ����(PlayerRef)�� �÷��̾� ������Ʈ(������Ʈ T�� �ִ� ��Ʈ��ũ ������Ʈ)�� 1:1 ������ ����մϴ�.
/// </summary>
public static class PlayerManager<T> where T : SimulationBehaviour
{
    // �ӽ÷� �÷��̾� ������ ������ ����Ʈ
    private static List<PlayerRef> _tempSpawnPlayers = new List<PlayerRef>();
    // �ӽ÷� ������ �÷��̾� ������Ʈ�� ������ ����Ʈ
    private static List<T> _tempSpawnedPlayers = new List<T>();

    /// <summary>
    /// �÷��̾� ���� ���¸� ������Ʈ�ϰ�, �ʿ��� ��� ���� �� ���� �ݹ��� ȣ���մϴ�.
    /// </summary>
    /// <param name="runner">��Ʈ��ũ ���� ��ü</param>
    /// <param name="spawnPlayer">���� �ݹ� �Լ�</param>
    /// <param name="despawnPlayer">���� �ݹ� �Լ�</param>
    public static void UpdatePlayerConnections(NetworkRunner runner, Action<PlayerRef> spawnPlayer, Action<PlayerRef, T> despawnPlayer)
    {
        _tempSpawnPlayers.Clear(); // �ӽ� ���� ����Ʈ �ʱ�ȭ
        _tempSpawnedPlayers.Clear(); // �ӽ� ������ ����Ʈ �ʱ�ȭ

        // 1. ��� ����� �÷��̾ �����ͼ� ���� ��� ���·� ǥ���մϴ�.
        _tempSpawnPlayers.AddRange(runner.ActivePlayers);

        // 2. Ÿ�� T�� ������Ʈ�� ���� ��� �÷��̾� ������Ʈ�� �����ɴϴ�.
        runner.GetAllBehaviours(_tempSpawnedPlayers);

        // 3. �̹� �����ϴ� �÷��̾� ������Ʈ�� PlayerRef�� ���� ��� ����Ʈ���� �����մϴ�.
        for (int i = 0; i < _tempSpawnedPlayers.Count; ++i)
        {
            T player = _tempSpawnedPlayers[i];
            PlayerRef playerRef = player.Object.InputAuthority;

            _tempSpawnPlayers.Remove(playerRef);

            // 4. ��ȿ���� ���� �÷��̾�(���� ����)��� ���� �ݹ��� ȣ���մϴ�.
            if (runner.IsPlayerValid(playerRef) == false)
            {
                try
                {
                    despawnPlayer(playerRef, player);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception); // ���ܰ� �߻��ϸ� ����� �α׿� ���
                }
            }
        }

        // 5. ���� ����� ��� �÷��̾ ���� ���� �ݹ��� �����մϴ�.
        for (int i = 0; i < _tempSpawnPlayers.Count; ++i)
        {
            try
            {
                spawnPlayer(_tempSpawnPlayers[i]);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception); // ���ܰ� �߻��ϸ� ����� �α׿� ���
            }
        }

        // 6. ����Ʈ �ʱ�ȭ
        _tempSpawnPlayers.Clear();
        _tempSpawnedPlayers.Clear();
    }
}

