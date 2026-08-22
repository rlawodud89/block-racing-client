using block_racing_common.Network.Packets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [SerializeField]
    private Button loginButton;

    private void Awake()
    {
        LoginEvents.OnLoginSuccess += LoadLobby;

        NetworkEvents.OnConnected += EnableLoginButton;
        NetworkEvents.OnDisconnected += DisableLoginButton;

        loginButton.interactable = NetworkManager.Instance.IsConnected;
    }

    private void OnDestroy()
    {
        LoginEvents.OnLoginSuccess -= LoadLobby;

        NetworkEvents.OnConnected += EnableLoginButton;
        NetworkEvents.OnDisconnected += DisableLoginButton;
    }

    private void EnableLoginButton()
    {
        loginButton.interactable = true;
    }

    private void DisableLoginButton()
    {
        loginButton.interactable = false;
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