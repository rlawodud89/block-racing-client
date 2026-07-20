using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    private void Start()
    {
        LoadTitle();
    }


    private void LoadTitle()
    {
        StartCoroutine(
            SceneLoader.ChangeScene("Title")
        );
    }
}