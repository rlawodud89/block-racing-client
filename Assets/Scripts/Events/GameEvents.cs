using System;
using block_racing_common.Network.Packets;

public static class GameEvents
{
    public static Action<S_StartGamePacket> OnStartGameReceived;
    public static Action<S_GameEndPacket> OnGameEnded;

    public static event Action OnOpponentExited;


    public static void InvokeStartGameReceived(S_StartGamePacket packet)
    {
        OnStartGameReceived?.Invoke(packet);
    }

    public static void InvokeGameEnded(S_GameEndPacket packet)
    {
        OnGameEnded?.Invoke(packet);
    }

    public static void RaiseOpponentExited()
    {
        OnOpponentExited?.Invoke();
    }
}