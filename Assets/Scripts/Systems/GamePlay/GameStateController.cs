using block_racing_common.Game.Snapshots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ApplySnapshot(GameStateSnapshot snapshot)
    {
        foreach (var playerSnapshot in snapshot.Players)
        {
            // 플레이어 위치 반영
            // 차 위치 이동
            // 블록 상태 반영
        }

        // 라인 상태, FlyingBlock 등도 여기서 처리
    }
}