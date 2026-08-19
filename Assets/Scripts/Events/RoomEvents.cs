using System;

public static class RoomEvents
{
    // Private Room 생성 / 입장
    public static event Action<string> OnRoomCreated;
    public static event Action<int> OnRoomJoined;
    public static event Action OnRoomJoinFailed;

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

    public static void RaiseRoomJoinFailed()
    {
        OnRoomJoinFailed?.Invoke();
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