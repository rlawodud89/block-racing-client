using block_racing_common.Game.Enums;
using TMPro;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text resultText;

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
    }

    public void OnClickLobbyBtn()
    {
        ResultData.Clear();

        StartCoroutine(
            SceneLoader.ChangeScene("Lobby")
        );
    }
}