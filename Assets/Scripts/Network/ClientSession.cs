using block_racing_common.Network;
using block_racing_common.Network.Packets;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSession
{
    private TcpClient _client;
    private NetworkStream _stream;

    private readonly ReceiveBuffer _receiveBuffer = new();
    private readonly PacketManager _packetManager;

    private bool _isConnected;
    private bool _disconnectRaised;

    private DateTime _lastHeartbeatTime;

    private const int HeartbeatTimeout = 5000;
    private const int HeartbeatCheckInterval = 1000;

    public bool IsConnected => _isConnected;

    public ClientSession(PacketManager packetManager)
    {
        _packetManager = packetManager;
    }


    public async Task ConnectAsync(string ip, int port)
    {
        try
        {
            _client = new TcpClient();

            await _client.ConnectAsync(ip, port);

            _stream = _client.GetStream();

            _isConnected = true;
            _disconnectRaised = false;

            _lastHeartbeatTime = DateTime.UtcNow;

            ClientLogger.Network(
                   $"Connected to server. Address={ip}:{port}");

            _ = ReceiveLoopAsync();
            _ = HeartbeatTimeoutLoopAsync();
        }
        catch (SocketException ex)
        {
            ClientLogger.Warning(
                $"Failed to connect to server. " +
                $"Address={ip}:{port}. " +
                $"Retrying...\n{ex.Message}");

            Disconnect();

            throw;
        }
        catch (Exception ex)
        {
            ClientLogger.Warning(
                $"Unexpected connection failure. " +
                $"Address={ip}:{port}\n{ex}");

            Disconnect();

            throw;
        }

    }

    public async Task SendAsync(IPacket packet)
    {
        if (!_isConnected)
            return;

        try
        {
            var writer =
                new PacketWriter((ushort)packet.PacketId);

            packet.Write(writer);

            byte[] buffer = writer.ToArray();

            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            if (!_isConnected)
                return;

            ClientLogger.Error(
                $"Failed to send packet. " +
                $"Packet={packet.PacketId}\n{ex}");

            HandleUnexpectedDisconnect();
        }
    }

    private async Task ReceiveLoopAsync()
    {
        byte[] tempBuffer = new byte[1024];

        try
        {
            while (_isConnected)
            {
                int read = await _stream.ReadAsync(
                    tempBuffer,
                    0,
                    tempBuffer.Length
                );

                if (read == 0)
                {
                    if (_isConnected)
                    {
                        ClientLogger.Warning(
                            "Server closed the connection.");

                        HandleUnexpectedDisconnect();
                    }

                    break;
                }

                _receiveBuffer.Append(tempBuffer, read);

                while (_receiveBuffer.TryReadPacket(out byte[] packetData))
                {
                    ProcessPacket(packetData);
                }
            }
        }
        catch (Exception ex)
        {
            if (_isConnected)
            {
                ClientLogger.Warning(
                    $"Receive loop failed.\n{ex}");

                HandleUnexpectedDisconnect();
            }
        }
        finally
        {
            Disconnect();
        }
    }

    private void ProcessPacket(byte[] packet)
    {
        try
        {
            PacketReader reader = new(packet);

            // Length skip
            ushort length = reader.ReadUInt16();

            ushort packetId = reader.ReadUInt16();

            PacketId id = (PacketId)packetId;

            _packetManager.Process(id, reader);
        }
        catch (Exception ex)
        {
            ClientLogger.Error(
                $"Failed to process received packet.\n{ex}");

            HandleUnexpectedDisconnect();
        }
    }

    private async Task HeartbeatTimeoutLoopAsync()
    {
        while (_isConnected)
        {
            await Task.Delay(HeartbeatCheckInterval);

            if (!_isConnected)
                break;

            if (DateTime.UtcNow - _lastHeartbeatTime
                > TimeSpan.FromMilliseconds(HeartbeatTimeout))
            {
                ClientLogger.Warning(
                    "Heartbeat timeout.");

                HandleUnexpectedDisconnect();
                break;
            }
        }
    }

    public void UpdateHeartbeat()
    {
        _lastHeartbeatTime = DateTime.UtcNow;

        _ = SendAsync(new C_HeartbeatPacket());
    }

    public void Disconnect()
    {
        if (!_isConnected)
            return;

        _isConnected = false;

        _stream?.Close();
        _stream = null;

        _client?.Close();
        _client = null;
    }

    private void RaiseDisconnected()
    {
        if (_disconnectRaised)
            return;

        _disconnectRaised = true;

        ClientLogger.Network(
            "Disconnected event raised.");

        NetworkEvents.RaiseDisconnected();
    }


    private void HandleUnexpectedDisconnect()
    {
        Disconnect();
        RaiseDisconnected();
    }
}