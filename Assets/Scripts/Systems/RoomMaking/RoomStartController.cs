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
        ClientLogger.Game("Room ready. Loading Game scene.");

        SceneLoader.Instance.LoadScene("Game");
    }
}
