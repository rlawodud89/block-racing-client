using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStartController : MonoBehaviour
{
    private void Awake()
    {
        RoomEvents.OnRoomReady += LoadGame;
    }

    private void OnDestroy()
    {
        RoomEvents.OnRoomReady -= LoadGame;
    }

    private void LoadGame()
    {
        Debug.Log($"[MatchController] LoadGame 호출");

        SceneLoader.Instance.LoadScene("Game");
    }
}
