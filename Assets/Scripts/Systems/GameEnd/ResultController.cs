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
    private TMP_Text descriptionText;

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
        resultText.text = GetResultText();
        descriptionText.text = GetDescriptionText();

        SetupRematch();
    }

    private string GetResultText()
    {
        return ResultData.Result switch
        {
            GameResultType.Win => "승리",
            GameResultType.Lose => "패배",
            GameResultType.Draw => "무승부",
            _ => string.Empty
        };
    }

    private string GetDescriptionText()
    {
        if (ResultData.Reason == GameEndReason.OpponentDisconnected)
        {
            return "상대방이 게임을 나갔습니다.";
        }

        return ResultData.Result switch
        {
            GameResultType.Win => "먼저 결승선에 도착했습니다.",
            GameResultType.Lose => "상대방이 먼저 도착했습니다.",
            GameResultType.Draw => "동시에 도착했습니다.",
            _ => string.Empty
        };
    }

    private void SetupRematch()
    {
        bool canRematch =
            ResultData.Reason == GameEndReason.Normal;

        rematchButton.interactable = canRematch;
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