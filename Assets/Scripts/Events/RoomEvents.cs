using block_racing_common.Game.Enums;
using System;

public static class RoomEvents
{
    // Private Room 생성 / 입장
    public static event Action<string> OnRoomCreated;
    public static event Action<int> OnRoomJoined;

    public static event Action<RoomCreateResult> OnRoomCreateFailed;
    public static event Action<RoomJoinResult> OnRoomJoinFailed;

    // Room 상태
    public static event Action OnRoomReady;
    public static event Action OnGameCanceled;


    public static void RaiseRoomCreated(string roomCode)
    {
        OnRoomCreated?.Invoke(roomCode);
    }

    public static void RaiseRoomJoined(int roomId)
    {
        OnRoomJoined?.Invoke(roomId);
    }

    public static void RaiseRoomCreateFailed(RoomCreateResult result)
    {
        OnRoomCreateFailed?.Invoke(result);
    }

    public static void RaiseRoomJoinFailed(RoomJoinResult result)
    {
        OnRoomJoinFailed?.Invoke(result);
    }

    public static void RaiseRoomReady()
    {
        OnRoomReady?.Invoke();
    }

    public static void RaiseGameCanceled()
    {
        OnGameCanceled?.Invoke();
    }
}