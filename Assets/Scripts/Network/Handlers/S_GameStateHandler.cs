using block_racing_common.Network.Packets;
using UnityEngine;

public static class S_GameStateHandler
{
    public static void Handle(S_GameStatePacket packet)
    {
        GameStateController.Instance.ApplySnapshot(
            packet.Snapshot
        );
    }
}