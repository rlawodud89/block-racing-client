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
        RoomEvents.OnRoomJoinFailed += HandleRoomJoinFailed;
    }

    private void OnDestroy()
    {
        RoomEvents.OnRoomCreated -= HandleRoomCreated;
        RoomEvents.OnRoomJoined -= HandleRoomJoined;
        RoomEvents.OnRoomJoinFailed -= HandleRoomJoinFailed;
    }

    public void OnClickCreateRoomBtn()
    {
        var packet = new C_CreateRoomPacket();

        _ = NetworkManager.Instance.SendAsync(packet);
    }

    public void OnClickCreateRoomExitBtn()
    {
        var packet = new C_LeaveRoomPacket();

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

    private void HandleRoomJoinFailed()
    {
        Debug.Log("Room Join Failed.");

        // "존재하지 않는 방입니다." 같은 메시지 표시
    }
}