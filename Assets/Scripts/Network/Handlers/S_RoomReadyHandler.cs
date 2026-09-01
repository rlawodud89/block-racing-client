using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_RoomReadyHandler
{
    public static void Handle(S_RoomReadyPacket packet)
    {
        MatchContext.RoomId = packet.RoomId;

        RoomEvents.RaiseRoomReady();
    }
}