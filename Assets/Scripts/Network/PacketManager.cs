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