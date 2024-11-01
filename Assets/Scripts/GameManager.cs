using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager Ŭ������ ��Ʈ��ũ ȯ�濡�� �÷��̾� ��ü�� �����ϰ� �����ϴ� ������ ����.
/// </summary>
public class GameManager : NetworkBehaviour
{
    public NetworkObject PlayerPrefab; // ������ �÷��̾��� ������

    /// <summary>
    /// ��Ʈ��ũ ��ü�� ������ �� ȣ��Ǵ� �޼���
    /// </summary>
    public override void Spawned()
    {
        // ���� ���(Shared Mode)����, ��� �÷��̾�� �ڽ��� �÷��̾� ��ü�� ����.
        if (Runner.GameMode == GameMode.Shared)
        {
            SpawnPlayer(Runner.LocalPlayer); // ���� �÷��̾ ���� �÷��̾� ��ü ����
        }
    }

    /// <summary>
    /// ��Ʈ��ũ ���� ������Ʈ���� �ֱ������� ȣ��Ǵ� �޼���
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer == true)
        {
            // Ŭ���̾�Ʈ-���� ���������� �������� �÷��̾� ��ü�� ����
            // PlayerManager�� NetworkRunner.ActivePlayers ����Ʈ�� ������� �÷��̾� ���� ���¸� ������Ʈ�ϰ� �ʿ��� �� ��ü�� ����/����
            PlayerManager<Player>.UpdatePlayerConnections(Runner, SpawnPlayer, DespawnPlayer);
        }
    }

    /// <summary>
    /// Ư�� �÷��̾ ���� �÷��̾� ��ü ���� �޼���
    /// </summary>
    /// <param name="playerRef">�÷��̾� ����</param>
    private void SpawnPlayer(PlayerRef playerRef)
    {
        // �� �÷��̾ ������ ��ġ�� �����ǵ��� ��ġ�� ����
        Vector3 spawnPosition = new Vector3((playerRef.RawEncoded % Runner.Config.Simulation.PlayerCount) * 3, 1, 0);

        // �ùٸ� �Է� ������ ������ �÷��̾� ��ü�� ����
        NetworkObject player = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, playerRef);

        // ������ �ν��Ͻ��� �÷��̾� ��ü�� �����Ͽ� �ٸ� �������� ���� ���� �����ϰ� ��
        Runner.SetPlayerObject(playerRef, player);

        // ���� ���� ����(Interest Management)�� Ȱ��ȭ�� ���, �� �÷��̾ �ڽ��� ��ü�� �׻� ������ �������� ����
        Runner.SetPlayerAlwaysInterested(playerRef, player, true);
    }

    /// <summary>
    /// Ư�� �÷��̾��� ��ü�� �����ϴ� �޼���
    /// </summary>
    /// <param name="playerRef">�÷��̾� ����</param>
    /// <param name="player">������ �÷��̾� ��ü</param>
    private void DespawnPlayer(PlayerRef playerRef, Player player)
    {
        // �÷��̾� ��ü�� ������ ����. �߰����� ������ �ʿ����� ����
        Runner.Despawn(player.Object);
    }
}
