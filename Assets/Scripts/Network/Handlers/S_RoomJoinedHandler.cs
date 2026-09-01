using block_racing_common.Game.Enums;
using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_RoomJoinedHandler
{
    public static void Handle(S_RoomJoinedPacket packet)
    {
        if (packet.Result != RoomJoinResult.Success)
        {
            RoomEvents.RaiseRoomJoinFailed(packet.Result);
            return;
        }

        RoomEvents.RaiseRoomJoined(packet.RoomId);
    }
}