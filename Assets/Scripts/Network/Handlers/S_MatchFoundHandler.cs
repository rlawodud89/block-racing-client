using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_MatchFoundHandler
{
    public static void Handle(S_MatchFoundPacket packet)
    {
        Debug.Log(
            $"Match Found. RoomId : {packet.RoomId}");

        MatchContext.RoomId = packet.RoomId;

        MatchEvents.RaiseMatchFound();
    }
}