using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_GameEndHandler
{
    public static void Handle(S_GameEndPacket packet)
    {
        Debug.Log($"Game Ended. Result: {packet.Result}");

        GameEvents.InvokeGameEnded(packet);
    }
}