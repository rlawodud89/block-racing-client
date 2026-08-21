using block_racing_common.Game.Enums;
using block_racing_common.Network.Packets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text resultText;

    [SerializeField]
    private TMP_Text normalText;
    [SerializeField]
    private TMP_Text opponentDisconnectedText;

    [SerializeField]
    private Button rematchButton;

    [SerializeField]
    private GameObject rematchPanel;
    [SerializeField]
    private TMP_Text waitText;
    [SerializeField]
    private TMP_Text exitText;

    private void OnEnable()
    {
        GameEvents.OnOpponentExited += HandleOpponentExited;
        RoomEvents.OnRoomReady += HandleRoomReady;
    }

    private void OnDisable()
    {
        GameEvents.OnOpponentExited -= HandleOpponentExited;
        RoomEvents.OnRoomReady -= HandleRoomReady;
    }

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
                normalText.gameObject.SetActive(true);
                opponentDisconnectedText.gameObject.SetActive(false);
                rematchButton.interactable = true;
                break;

            case GameEndReason.OpponentDisconnected:
                normalText.gameObject.SetActive(false);
                opponentDisconnectedText.gameObject.SetActive(true);
                rematchButton.interactable = false;
                break;
        }
    }

    public void OnClickRematchBtn()
    {
        rematchButton.interactable = false;

        var packet = new C_RematchReqeustPacket();

        NetworkManager.Instance.SendAsync(packet);

        rematchPanel.SetActive(true);
        waitText.gameObject.SetActive(true);
        exitText.gameObject.SetActive(false);
    }

    public void OnClickLobbyBtn()
    {
        var packet = new C_ExitRoomPacket();

        NetworkManager.Instance.SendAsync(packet);

        ResultData.Clear();

        SceneLoader.Instance.LoadScene("Lobby");
    }

    private void HandleOpponentExited()
    {
        rematchButton.interactable = false;

        rematchPanel.SetActive(true);
        waitText.gameObject.SetActive(false);
        exitText.gameObject.SetActive(true);
    }

    private void HandleRoomReady()
    {
        ResultData.Clear();

        SceneLoader.Instance.LoadScene("Game");
    }
}