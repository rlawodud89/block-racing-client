using block_racing_common.Network.Packets;
using UnityEngine;

public static class S_StartGameHandler
{
    public static void Handle(S_StartGamePacket packet)
    {
        ClientLogger.Game(
            $"Game start received. RoomId={packet.RoomId}");

        GameEvents.InvokeStartGameReceived(packet);
    }
}

