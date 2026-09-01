using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_GameEndHandler
{
    public static void Handle(S_GameEndPacket packet)
    {
        ClientLogger.Game(
            $"Game ended. Result={packet.Result}, Reason={packet.Reason}");

        GameEvents.InvokeGameEnded(packet);
    }
}