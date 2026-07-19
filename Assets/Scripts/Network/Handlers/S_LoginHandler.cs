using block_racing_common.Network.Packets;
using UnityEngine;

public static class S_LoginHandler
{
    public static void Handle(S_LoginPacket packet)
    {
        Debug.Log(
            $"Login Success PlayerId : {packet.PlayerId}");
    }
}
