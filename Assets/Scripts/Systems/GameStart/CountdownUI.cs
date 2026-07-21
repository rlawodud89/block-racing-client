using TMPro;
using UnityEngine;
using System.Collections;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    public IEnumerator StartCountdown(float seconds)
    {
        countdownText.gameObject.SetActive(true);

        float remainTime = seconds;

        while (remainTime > 0)
        {
            int displayNumber = Mathf.CeilToInt(remainTime);

            countdownText.text = displayNumber.ToString();

            remainTime -= Time.deltaTime;

            yield return null;
        }

        countdownText.text = "GO!";

        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);
    }
}