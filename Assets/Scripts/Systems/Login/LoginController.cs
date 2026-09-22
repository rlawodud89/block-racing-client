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
        LoginEvents.OnLoginSuccess += HandleLoginSuccess;

        NetworkEvents.OnConnected += EnableLoginButton;
        NetworkEvents.OnDisconnected += DisableLoginButton;

        loginButton.interactable = NetworkManager.Instance.IsConnected;
    }

    private void OnDestroy()
    {
        LoginEvents.OnLoginSuccess -= HandleLoginSuccess;

        NetworkEvents.OnConnected -= EnableLoginButton;
        NetworkEvents.OnDisconnected -= DisableLoginButton;
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
        var packet = new C_LoginPacket();

        await NetworkManager.Instance.SendAsync(packet);
    }

    private void HandleLoginSuccess()
    {
        ClientLogger.Game(
            $"Login succeeded. PlayerId={ClientContext.PlayerId}");

        SceneLoader.Instance.LoadScene("Lobby");
    }
}