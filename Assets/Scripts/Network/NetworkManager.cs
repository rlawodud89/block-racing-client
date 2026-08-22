using block_racing_common.Network;
using block_racing_common.Network.Packets;
using System;
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
        Debug.Log("HandleDisconnected");

        SceneLoader.Instance.LoadScene("Title");

        WarningUI.Instance.Show("서버와의 연결이 끊겼습니다.");

        _ = ConnectLoopAsync();
    }

    private async Task ConnectLoopAsync()
    {
        if (_isConnecting)
            return;

        _isConnecting = true;

        while (_isRunning)
        {
            try
            {
                CreateSession();

                await _session.ConnectAsync("127.0.0.1", 7777);

                Debug.Log("서버 연결 성공");

                WarningUI.Instance.Show("서버에 연결되었습니다.");

                NetworkEvents.RaiseConnected();

                break;
            }
            catch (Exception ex)
            {
                Debug.Log($"서버 연결 실패: {ex.Message}");

                await Task.Delay(2000);
            }
        }

        _isConnecting = false;
    }

    public Task SendAsync(IPacket packet)
    {
        return _session.SendAsync(packet);
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