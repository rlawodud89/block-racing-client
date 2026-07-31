using block_racing_common.Network;
using block_racing_common.Network.Packets;
using System;
using System.Collections.Generic;


public class PacketManager
{
    private readonly Dictionary<PacketId, Action<PacketReader>> _handlers
        = new();

    public PacketManager()
    {
        Register<S_LoginPacket>(PacketId.S_Login, S_LoginHandler.Handle);
        Register<S_MatchFoundPacket>(PacketId.S_MatchFound, S_MatchFoundHandler.Handle);
        Register<S_StartGamePacket>(PacketId.S_StartGame, S_StartGameHandler.Handle);
        Register<S_GameStatePacket>(PacketId.S_GameState, S_GameStateHandler.Handle);
        Register<S_GameEndPacket>(PacketId.S_GameEnd, S_GameEndHandler.Handle);
    }

    public void Register<T>(
        PacketId id,
        Action<T> handler)
        where T : IPacket, new()
    {
        _handlers[id] = reader =>
        {
            T packet = new();

            packet.Read(reader);

            handler(packet);
        };
    }

    public void Process(PacketId id, PacketReader reader)
    {
        if (_handlers.TryGetValue(id, out var handler))
        {
            handler(reader);
        }
        else
        {
            Console.WriteLine($"Unknown Packet : {id}");
        }
    }
}