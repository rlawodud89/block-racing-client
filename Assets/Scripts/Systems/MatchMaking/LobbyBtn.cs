using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBtn : MonoBehaviour
{
    public void OnClickLobbyBtn()
    {
        StartCoroutine(
            SceneLoader.ChangeScene("Lobby")
        );
    }
}
