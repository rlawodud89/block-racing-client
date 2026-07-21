using block_racing_common.Network.Packets;
using UnityEngine;

public static class S_StartGameHandler
{
    public static void Handle(S_StartGamePacket packet)
    {
        Debug.Log(
            $"Game Start. RoomId : {packet.RoomId}");

        GameEvents.InvokeStartGameReceived(packet);
    }
}

