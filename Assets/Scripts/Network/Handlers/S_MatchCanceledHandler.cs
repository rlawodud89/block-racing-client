using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_MatchCanceledHandler
{
    public static void Handle(S_MatchCanceledPacket packet)
    {
        MatchEvents.RaiseMatchCanceled();
    }
}