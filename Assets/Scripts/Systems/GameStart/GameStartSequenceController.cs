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

    private long _startTick;
    private bool _isGameStarted;

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
        _startTick = packet.StartTick;
        _isGameStarted = false;

        countdownUI.StartCountdown(_startTick);
    }

    public void ApplyTick(long currentTick)
    {
        if (_isGameStarted)
            return;


        countdownUI.UpdateCountdown(currentTick);

        if (currentTick < _startTick)
            return;


        _isGameStarted = true;

        AudioManager.Instance.PlayGameStart();

        OnGameStarted?.Invoke();

        StartCoroutine(HideCountdownRoutine());
    }

    private IEnumerator HideCountdownRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        countdownUI.Hide();
    }
}
