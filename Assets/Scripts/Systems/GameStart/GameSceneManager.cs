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
            AudioManager.Instance.PlayGameSceneEnter();

            await NetworkManager.Instance.SendAsync(new C_ReadyPacket());

            ClientLogger.Packet("C_Ready packet sent.");
        }
        catch (System.Exception ex)
        {
            ClientLogger.Error(
                $"Failed to send game ready packet.\n{ex}");
        }
    }

    private void HandleMatchCanceled()
    {
        ClientLogger.Game(
            "Game canceled. Loading MatchCanceled scene.");

        SceneLoader.Instance.LoadScene("MatchCanceled");
    }
}