using block_racing_common.Network;
using block_racing_common.Network.Packets;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private PacketManager _packetManager;
    private ClientSession _session;

    private void Awake()
    {
        InitializeNetwork();

        _ = ConnectServerAsync();
    }


    private async Task ConnectServerAsync()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeNetwork();

            await _session.ConnectAsync("127.0.0.1", 7777);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeNetwork()
    {
        _packetManager = new PacketManager();

        _session = new ClientSession(
            _packetManager);
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
        _session?.Disconnect();

        if (Instance == this)
        {
            Instance = null;
        }
    }
}