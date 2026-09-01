using block_racing_common.Network;
using block_racing_common.Network.Packets;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private PacketManager _packetManager;
    private ClientSession _session;

    private bool _isConnecting;
    private bool _isRunning = true;

    public bool IsConnected =>
        _session != null && _session.IsConnected;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeNetwork();

        NetworkEvents.OnDisconnected += HandleDisconnected;

        _ = ConnectLoopAsync();
    }

    private void InitializeNetwork()
    {
        _packetManager = new PacketManager();
    }

    private void CreateSession()
    {
        _session = new ClientSession(_packetManager);
    }

    private void HandleDisconnected()
    {
        ClientLogger.Network(
            "Disconnected from server.");

        SceneLoader.Instance.LoadScene("Title");

        WarningUI.Instance.Show("서버와의 연결이 끊겼습니다.");

        _ = ConnectLoopAsync();
    }

    private async Task ConnectLoopAsync()
    {
        if (_isConnecting)
            return;

        _isConnecting = true;

        try
        {
            while (_isRunning)
            {
                try
                {
                    CreateSession();

                    await _session.ConnectAsync(
                        "127.0.0.1",
                        7777);

                    ClientLogger.Network(
                        "Connection established.");

                    WarningUI.Instance.Show("서버에 연결되었습니다.");

                    NetworkEvents.RaiseConnected();

                    break;
                }
                catch (SocketException ex)
                {
                    ClientLogger.Warning(
                        $"Connection attempt failed. " +
                        $"Retrying in 2 seconds.\n{ex}");

                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    ClientLogger.Error(
                        $"Unexpected error during connection. {ex}");

                    break;
                }
            }
        }
        finally
        {
            _isConnecting = false;
        }
    }

    public Task SendAsync(IPacket packet)
    {
        return _session.SendAsync(packet);
    }

    public void UpdateHeartbeat()
    {
        _session?.UpdateHeartbeat();
    }

    public void Shutdown()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _isRunning = false;

        NetworkEvents.OnDisconnected -= HandleDisconnected;

        _session?.Disconnect();

        if (Instance == this)
        {
            Instance = null;
        }
    }
}