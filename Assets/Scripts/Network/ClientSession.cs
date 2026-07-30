using block_racing_common.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using UnityEngine;

public class ClientSession
{
    private TcpClient _client;
    private NetworkStream _stream;

    private readonly ReceiveBuffer _receiveBuffer = new();
    private readonly PacketManager _packetManager;

    public bool _isConnected;

    public bool IsConnected => _isConnected;

    public ClientSession(PacketManager packetManager)
    {
        _packetManager = packetManager;
    }


    public async Task ConnectAsync(string ip, int port)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(ip, port);

        _stream = _client.GetStream();

        _isConnected = true;

        _ = ReceiveLoopAsync();
    }

    public async Task SendAsync(IPacket packet)
    {
        var writer = new PacketWriter((ushort)packet.PacketId);
        packet.Write(writer);
        byte[] buffer = writer.ToArray();
        await _stream.WriteAsync(buffer, 0, buffer.Length);
    }

    private async Task ReceiveLoopAsync()
    {
        byte[] tempBuffer = new byte[1024];

        try
        {
            while (IsConnected)
            {
                int read = await _stream.ReadAsync(tempBuffer, 0, tempBuffer.Length);

                if (read == 0)
                    break;

                _receiveBuffer.Append(tempBuffer, read);

                while (_receiveBuffer.TryReadPacket(out byte[] packetData))
                {
                    ProcessPacket(packetData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            Disconnect();
        }

    }

    private void ProcessPacket(byte[] packet)
    {
        PacketReader reader = new(packet);

        // Length skip
        ushort length = reader.ReadUInt16();

        ushort packetId = reader.ReadUInt16();

        PacketId id = (PacketId)packetId;

        _packetManager.Process(id, reader);
    }

    public void Disconnect()
    {
        _isConnected = false;

        _stream?.Close();
        _client?.Close();
    }
}