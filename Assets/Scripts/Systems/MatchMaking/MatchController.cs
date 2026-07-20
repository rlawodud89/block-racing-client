using block_racing_common.Network.Packets;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    private void Awake()
    {
        MatchEvents.OnMatchFound += LoadGame;
    }

    private void OnDestroy()
    {
        MatchEvents.OnMatchFound -= LoadGame;
    }

    public void OnClickMatch()
    {
        var packet = new C_MatchRequestPacket();

        _ = NetworkManager.Instance.SendAsync(packet);

        Debug.Log("C_MatchRequestPacket sent");
    }

    private void LoadGame()
    {
        StartCoroutine(
            SceneLoader.ChangeScene("Game")
            );
    }
}