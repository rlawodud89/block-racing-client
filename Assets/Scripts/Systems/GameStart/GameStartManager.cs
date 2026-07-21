using block_racing_common.Network.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartSequenceController : MonoBehaviour
{
    public static GameStartSequenceController Instance { get; private set; }

    public static event Action OnGameStarted;

    [SerializeField] private CountdownUI countdownUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;


        GameEvents.OnStartGameReceived += StartGame;
    }


    private void OnDestroy()
    {
        GameEvents.OnStartGameReceived -= StartGame;

        if (Instance == this)
        {
            Instance = null;
        }
    }


    private void StartGame(S_StartGamePacket packet)
    {
        StartCoroutine(
            StartRoutine(packet.CountdownSeconds)
        );
    }


    private IEnumerator StartRoutine(float seconds)
    {
        yield return countdownUI.StartCountdown(seconds);

        OnGameStarted?.Invoke();
    }
}
