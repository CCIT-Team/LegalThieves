using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// 활성 플레이어 목록(NetworkRunner.ActivePlayers)을 반복하여 스폰 및 디스폰 콜백을 호출하는 헬퍼 클래스입니다.
/// 각 플레이어 연결(PlayerRef)과 플레이어 오브젝트(컴포넌트 T가 있는 네트워크 오브젝트)의 1:1 매핑을 기대합니다.
/// </summary>
public static class PlayerManager<T> where T : SimulationBehaviour
{
    // 임시로 플레이어 참조를 저장할 리스트
    private static List<PlayerRef> _tempSpawnPlayers = new List<PlayerRef>();
    // 임시로 생성된 플레이어 오브젝트를 저장할 리스트
    private static List<T> _tempSpawnedPlayers = new List<T>();

    /// <summary>
    /// 플레이어 연결 상태를 업데이트하고, 필요한 경우 스폰 및 디스폰 콜백을 호출합니다.
    /// </summary>
    /// <param name="runner">네트워크 런너 객체</param>
    /// <param name="spawnPlayer">스폰 콜백 함수</param>
    /// <param name="despawnPlayer">디스폰 콜백 함수</param>
    public static void UpdatePlayerConnections(NetworkRunner runner, Action<PlayerRef> spawnPlayer, Action<PlayerRef, T> despawnPlayer)
    {
        _tempSpawnPlayers.Clear(); // 임시 스폰 리스트 초기화
        _tempSpawnedPlayers.Clear(); // 임시 스폰된 리스트 초기화

        // 1. 모든 연결된 플레이어를 가져와서 스폰 대기 상태로 표시합니다.
        _tempSpawnPlayers.AddRange(runner.ActivePlayers);

        // 2. 타입 T의 컴포넌트를 가진 모든 플레이어 오브젝트를 가져옵니다.
        runner.GetAllBehaviours(_tempSpawnedPlayers);

        // 3. 이미 존재하는 플레이어 오브젝트의 PlayerRef를 스폰 대기 리스트에서 제거합니다.
        for (int i = 0; i < _tempSpawnedPlayers.Count; ++i)
        {
            T player = _tempSpawnedPlayers[i];
            PlayerRef playerRef = player.Object.InputAuthority;

            _tempSpawnPlayers.Remove(playerRef);

            // 4. 유효하지 않은 플레이어(연결 끊김)라면 디스폰 콜백을 호출합니다.
            if (runner.IsPlayerValid(playerRef) == false)
            {
                try
                {
                    despawnPlayer(playerRef, player);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception); // 예외가 발생하면 디버그 로그에 출력
                }
            }
        }

        // 5. 새로 연결된 모든 플레이어에 대해 스폰 콜백을 실행합니다.
        for (int i = 0; i < _tempSpawnPlayers.Count; ++i)
        {
            try
            {
                spawnPlayer(_tempSpawnPlayers[i]);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception); // 예외가 발생하면 디버그 로그에 출력
            }
        }

        // 6. 리스트 초기화
        _tempSpawnPlayers.Clear();
        _tempSpawnedPlayers.Clear();
    }
}

