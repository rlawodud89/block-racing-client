using block_racing_common.Game.Enums;
using block_racing_common.Network.Packets;
using TMPro;
using UnityEngine;

public class PrivateRoomController : MonoBehaviour
{
    [Header("Create Room")]
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private TMP_Text roomCodeText;

    [Header("Join Room")]
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField roomCodeInput;

    private void Awake()
    {
        RoomEvents.OnRoomCreated += HandleRoomCreated;
        RoomEvents.OnRoomJoined += HandleRoomJoined;
        RoomEvents.OnRoomCreateFailed += HandleRoomCreateFailed;
        RoomEvents.OnRoomJoinFailed += HandleRoomJoinFailed;
    }

    private void OnDestroy()
    {
        RoomEvents.OnRoomCreated -= HandleRoomCreated;
        RoomEvents.OnRoomJoined -= HandleRoomJoined;
        RoomEvents.OnRoomCreateFailed -= HandleRoomCreateFailed;
        RoomEvents.OnRoomJoinFailed -= HandleRoomJoinFailed;
    }

    public void OnClickCreateRoomBtn()
    {
        var packet = new C_CreateRoomPacket();

        _ = NetworkManager.Instance.SendAsync(packet);
    }

    public void OnClickCreateRoomExitBtn()
    {
        var packet = new C_CloseRoomPacket();

        _ = NetworkManager.Instance.SendAsync(packet);

        roomCodeText.text = string.Empty;
        createRoomPanel.SetActive(false);
    }

    public void OnClickJoinRoomBtn()
    {
        joinRoomPanel.SetActive(true);
    }

    public void OnClickJoinRequestBtn()
    {
        string roomCode = roomCodeInput.text.Trim().ToUpperInvariant();

        if (string.IsNullOrEmpty(roomCode))
            return;

        var packet = new C_JoinRoomPacket
        {
            RoomCode = roomCode
        };

        _ = NetworkManager.Instance.SendAsync(packet);
    }

    public void OnClickJoinRoomExitBtn()
    {
        roomCodeInput.text = string.Empty;
        joinRoomPanel.SetActive(false);
    }


    private void HandleRoomCreated(string roomCode)
    {
        createRoomPanel.SetActive(true);
        roomCodeText.text = $"방 입장 코드\n{roomCode}";
    }

    private void HandleRoomJoined(int roomId)
    {
        Debug.Log($"Room Joined. RoomId={roomId}");

        joinRoomPanel.SetActive(false);
    }

    private void HandleRoomCreateFailed(RoomCreateResult result)
    {
        Debug.Log($"Room Create Failed. Result={result}");

        switch (result)
        {
            case RoomCreateResult.AlreadyInRoom:
                WarningUI.Instance?.Show("이미 방에 참가하고 있습니다.");
                break;

            case RoomCreateResult.AlreadyQueued:
                WarningUI.Instance?.Show("이미 매칭 대기 중입니다.");
                break;

            case RoomCreateResult.RoomLimitExceeded:
                WarningUI.Instance?.Show("더 이상 방을 생성할 수 없습니다.");
                break;

            default:
                WarningUI.Instance?.Show("방 생성에 실패했습니다.");
                break;
        }
    }

    private void HandleRoomJoinFailed(RoomJoinResult result)
    {
        Debug.Log($"Room Join Failed. Result={result}");

        switch (result)
        {
            case RoomJoinResult.AlreadyInRoom:
                WarningUI.Instance?.Show("이미 방에 참가하고 있습니다.");
                break;

            case RoomJoinResult.AlreadyQueued:
                WarningUI.Instance?.Show("이미 매칭 대기 중입니다.");
                break;

            case RoomJoinResult.RoomNotFound:
                WarningUI.Instance?.Show("존재하지 않는 방입니다.");
                break;

            case RoomJoinResult.RoomFull:
                WarningUI.Instance?.Show("이미 방이 가득 찼습니다.");
                break;

            default:
                WarningUI.Instance?.Show("방 입장에 실패했습니다.");
                break;
        }
    }
}