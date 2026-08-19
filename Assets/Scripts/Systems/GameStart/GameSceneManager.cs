using block_racing_common.Network.Packets;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private void OnEnable()
    {
        RoomEvents.OnGameCanceled += HandleMatchCanceled;
    }

    private void OnDisable()
    {
        RoomEvents.OnGameCanceled -= HandleMatchCanceled;
    }

    private async void Start()
    {
        try
        {
            await NetworkManager.Instance.SendAsync(new C_ReadyPacket());

            Debug.Log("Game Ready Sent");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void HandleMatchCanceled()
    {
        Debug.Log("Match canceled");

        SceneLoader.Instance.LoadScene("MatchCanceled");
    }
}