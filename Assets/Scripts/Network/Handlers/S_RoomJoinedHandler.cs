using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_RoomJoinedHandler
{
    public static void Handle(S_RoomJoinedPacket packet)
    {
        if (!packet.Success)
        {
            PrivateRoomEvents.RaiseRoomJoinFailed();
            return;
        }

        PrivateRoomEvents.RaiseRoomJoined(packet.RoomId);
    }
}