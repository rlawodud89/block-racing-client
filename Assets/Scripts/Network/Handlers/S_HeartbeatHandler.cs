using block_racing_common.Network.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S_HeartbeatHandler
{
    public static void Handle(S_HeartbeatPacket packet)
    {
        Debug.Log("S_Heartbeat 수신");

        _ = NetworkManager.Instance.SendAsync(new C_HeartbeatPacket());
    }
}