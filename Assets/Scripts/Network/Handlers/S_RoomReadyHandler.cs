using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_RoomReadyHandler
{
    public static void Handle(S_RoomReadyPacket packet)
    {
        Debug.Log(
            $"Match Found. RoomId : {packet.RoomId}");

        MatchContext.RoomId = packet.RoomId;

        RoomEvents.RaiseGameCanceled();
    }
}