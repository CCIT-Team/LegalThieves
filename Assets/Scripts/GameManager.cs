using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager 클래스는 네트워크 환경에서 플레이어 객체를 생성하고 관리하는 역할을 수행.
/// </summary>
public class GameManager : NetworkBehaviour
{
    public NetworkObject PlayerPrefab; // 생성할 플레이어의 프리팹

    /// <summary>
    /// 네트워크 객체가 생성될 때 호출되는 메서드
    /// </summary>
    public override void Spawned()
    {
        // 공유 모드(Shared Mode)에서, 모든 플레이어는 자신의 플레이어 객체를 생성.
        if (Runner.GameMode == GameMode.Shared)
        {
            SpawnPlayer(Runner.LocalPlayer); // 로컬 플레이어에 대해 플레이어 객체 생성
        }
    }

    /// <summary>
    /// 네트워크 고정 업데이트에서 주기적으로 호출되는 메서드
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer == true)
        {
            // 클라이언트-서버 구조에서는 서버만이 플레이어 객체를 생성
            // PlayerManager는 NetworkRunner.ActivePlayers 리스트를 기반으로 플레이어 연결 상태를 업데이트하고 필요할 때 객체를 생성/제거
            PlayerManager<Player>.UpdatePlayerConnections(Runner, SpawnPlayer, DespawnPlayer);
        }
    }

    /// <summary>
    /// 특정 플레이어를 위한 플레이어 객체 생성 메서드
    /// </summary>
    /// <param name="playerRef">플레이어 참조</param>
    private void SpawnPlayer(PlayerRef playerRef)
    {
        // 각 플레이어가 고유한 위치에 생성되도록 위치를 설정
        Vector3 spawnPosition = new Vector3((playerRef.RawEncoded % Runner.Config.Simulation.PlayerCount) * 3, 1, 0);

        // 올바른 입력 권한을 가지고 플레이어 객체를 생성
        NetworkObject player = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, playerRef);

        // 생성된 인스턴스를 플레이어 객체로 설정하여 다른 곳에서도 쉽게 접근 가능하게 함
        Runner.SetPlayerObject(playerRef, player);

        // 관심 영역 관리(Interest Management)가 활성화된 경우, 각 플레이어가 자신의 객체에 항상 관심을 가지도록 설정
        Runner.SetPlayerAlwaysInterested(playerRef, player, true);
    }

    /// <summary>
    /// 특정 플레이어의 객체를 제거하는 메서드
    /// </summary>
    /// <param name="playerRef">플레이어 참조</param>
    /// <param name="player">제거할 플레이어 객체</param>
    private void DespawnPlayer(PlayerRef playerRef, Player player)
    {
        // 플레이어 객체를 간단히 제거. 추가적인 정리는 필요하지 않음
        Runner.Despawn(player.Object);
    }
}
