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
        Debug.Log($"GameEndController received: {packet.Result}");

        ResultData.SetResult(packet.Result);

        Debug.Log("Loading Result Scene...");

        StartCoroutine(
            SceneLoader.ChangeScene("Result")
        );
    }
}