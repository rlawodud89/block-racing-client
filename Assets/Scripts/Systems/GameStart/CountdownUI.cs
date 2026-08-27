using TMPro;
using UnityEngine;
using System.Collections;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    private const int TickPerSecond = 20;

    private long _startTick;

    public void StartCountdown(long startTick)
    {
        _startTick = startTick;

        countdownText.gameObject.SetActive(true);
    }

    public void UpdateCountdown(long currentTick)
    {
        long remainTicks = _startTick - currentTick;

        if (remainTicks > 0)
        {
            int remainSeconds =
                Mathf.CeilToInt(
                    remainTicks / (float)TickPerSecond
                );

            countdownText.text = remainSeconds.ToString();

            return;
        }

        countdownText.text = "GO!";
    }

    public void Hide()
    {
        countdownText.gameObject.SetActive(false);
    }
}