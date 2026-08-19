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
        Register<S_RoomReadyPacket>(PacketId.S_RoomReady, S_RoomReadyHandler.Handle);
        Register<S_StartGamePacket>(PacketId.S_StartGame, S_StartGameHandler.Handle);
        Register<S_GameStatePacket>(PacketId.S_GameState, S_GameStateHandler.Handle);
        Register<S_GameEndPacket>(PacketId.S_GameEnd, S_GameEndHandler.Handle);
        Register<S_GameCanceledPacket>(PacketId.S_GameCanceled, S_GameCanceledHandler.Handle);
        Register<S_RoomCreatedPacket>(PacketId.S_RoomCreated, S_RoomCreatedHandler.Handle);
        Register<S_RoomJoinedPacket>(PacketId.S_RoomJoined, S_RoomJoinedHandler.Handle);
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