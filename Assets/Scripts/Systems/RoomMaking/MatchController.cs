using block_racing_common.Network.Packets;
using System.Collections;
using TMPro;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    [SerializeField] private TMP_Text MatchButtonText;
    [SerializeField] private GameObject MatchingTime;
    [SerializeField] private TMP_Text MatchingTimeText;

    private bool isMatching = false;
    private Coroutine matchCoroutine;

    private void Awake()
    {
        RoomEvents.OnRoomReady += HandleRoomReady;
    }

    private void OnDestroy()
    {
        RoomEvents.OnRoomReady -= HandleRoomReady;
    }

    private void HandleRoomReady()
    {
        StopMatchingUI();
    }

    public void OnClickMatch()
    {
        if (!isMatching)
        {
            StartMatch();
        }
        else
        {
            CancelMatch();
        }
    }

    private void StartMatch()
    {
        var packet = new C_MatchRequestPacket
        {
            IsMatch = true
        };

        _ = NetworkManager.Instance.SendAsync(packet);

        isMatching = true;

        MatchingTime.SetActive(true);

        MatchButtonText.text = "¸ÅÄª Áß...";

        matchCoroutine = StartCoroutine(MatchTimer());
    }

    private void CancelMatch()
    {
        var packet = new C_MatchRequestPacket
        {
            IsMatch = false
        };

        _ = NetworkManager.Instance.SendAsync(packet);

        StopMatchingUI();

        if (matchCoroutine != null)
        {
            StopCoroutine(matchCoroutine);
        }
    }

    private IEnumerator MatchTimer()
    {
        int elapsedSeconds = 0;

        while (isMatching)
        {
            int minutes = elapsedSeconds / 60;
            int seconds = elapsedSeconds % 60;

            MatchingTimeText.text = $"{minutes:00}:{seconds:00}";

            yield return new WaitForSeconds(1f);

            elapsedSeconds++;
        }
    }

    private void StopMatchingUI()
    {
        isMatching = false;

        MatchingTime.SetActive(false);

        if (matchCoroutine != null)
        {
            StopCoroutine(matchCoroutine);
            matchCoroutine = null;
        }

        MatchButtonText.text = "¸ÅÄª ½ÃÀÛ";
    }
}