using block_racing_common.Game.Enums;
using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_RoomCreatedHandler
{
    public static void Handle(S_RoomCreatedPacket packet)
    {
        if (packet.Result != RoomCreateResult.Success)
        {
            ClientLogger.Warning(
                $"Failed to create room. Result={packet.Result}");

            RoomEvents.RaiseRoomCreateFailed(packet.Result);
            return;
        }

        ClientLogger.Game(
            $"Private room created. RoomCode={packet.RoomCode}");

        RoomEvents.RaiseRoomCreated(packet.RoomCode);
    }
}