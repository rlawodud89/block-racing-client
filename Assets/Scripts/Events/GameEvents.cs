using System;
using block_racing_common.Network.Packets;

public static class GameEvents
{
    public static Action<S_StartGamePacket> OnStartGameReceived;

    public static void InvokeStartGameReceived(S_StartGamePacket packet)
    {
        OnStartGameReceived?.Invoke(packet);
    }
}