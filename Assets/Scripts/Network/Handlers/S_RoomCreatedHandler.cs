using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_RoomCreatedHandler
{
    public static void Handle(S_RoomCreatedPacket packet)
    {
        if (!packet.Success)
            return;

        RoomEvents.RaiseRoomCreated(packet.RoomCode);
    }
}