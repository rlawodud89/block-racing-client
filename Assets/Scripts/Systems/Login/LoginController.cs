using block_racing_common.Network.Packets;
using System.Threading.Tasks;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    private void Awake()
    {
        LoginEvents.OnLoginSuccess += LoadLobby;
    }

    private void OnDestroy()
    {
        LoginEvents.OnLoginSuccess -= LoadLobby;
    }

    public void OnClickLogin()
    {
        SendLogin();
    }

    private async void SendLogin()
    {
        var packet = new C_LoginPacket()
        {
            Nickname = "TestPlayer"
        };

        await NetworkManager.Instance.SendAsync(packet);
    }

    private void LoadLobby()
    {
        SceneLoader.Instance.LoadScene("Lobby");
    }
}