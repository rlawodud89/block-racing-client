using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    private void Awake()
    {
        Application.runInBackground = true;
    }

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