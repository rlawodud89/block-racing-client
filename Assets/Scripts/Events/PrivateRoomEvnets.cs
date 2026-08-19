using System;

public static class PrivateRoomEvents
{
    public static event Action<string> OnRoomCreated;
    public static event Action<int> OnRoomJoined;
    public static event Action OnRoomJoinFailed;

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
}