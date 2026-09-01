using block_racing_common.Network.Packets;
using UnityEngine;

public class GameEndController : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnGameEnded += HandleGameEnded;
    }

    private void OnDisable()
    {
        GameEvents.OnGameEnded -= HandleGameEnded;
    }

    private void HandleGameEnded(S_GameEndPacket packet)
    {
        ClientLogger.Game(
            $"Game ended. Result={packet.Result}, Reason={packet.Reason}");

        ResultData.SetResult(packet.Result, packet.Reason);

        GameStateController.Instance.StopGameState();

        SceneLoader.Instance.LoadScene("Result");
    }
}