using block_racing_common.Game.Enums;
using TMPro;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text resultText;
    [SerializeField]
    private TMP_Text opponentDisconnectedText;

    private void Start()
    {
        switch (ResultData.Result)
        {
            case GameResultType.Win:
                resultText.text = "½Â¸®";
                break;

            case GameResultType.Lose:
                resultText.text = "ÆÐ¹è";
                break;

            case GameResultType.Draw:
                resultText.text = "¹«½ÂºÎ";
                break;
        }

        switch (ResultData.Reason)
        {
            case GameEndReason.Normal:
                opponentDisconnectedText.gameObject.SetActive(false);
                break;
            case GameEndReason.OpponentDisconnected:
                opponentDisconnectedText.gameObject.SetActive(true);
                break;
        }
    }

    public void OnClickLobbyBtn()
    {
        ResultData.Clear();

        SceneLoader.Instance.LoadScene("Lobby");
    }
}