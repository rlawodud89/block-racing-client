using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_OpponentExitHandler
{
    public static void Handle(S_OpponentExitPacket packet)
    {
        GameEvents.RaiseOpponentExited();
    }
}