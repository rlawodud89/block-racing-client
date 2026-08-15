using block_racing_common.Network.Packets;
using UnityEngine;

public class GameEndController : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("GameEndController OnEnable");

        GameEvents.OnGameEnded += HandleGameEnded;
    }

    private void OnDisable()
    {
        Debug.Log("GameEndController OnDisable");

        GameEvents.OnGameEnded -= HandleGameEnded;
    }

    private void HandleGameEnded(S_GameEndPacket packet)
    {
        Debug.Log($"GameEndController received: {packet.Result}, {packet.Reason}");

        ResultData.SetResult(packet.Result, packet.Reason);

        Debug.Log("Loading Result Scene...");

        SceneLoader.Instance.LoadScene("Result");
    }
}