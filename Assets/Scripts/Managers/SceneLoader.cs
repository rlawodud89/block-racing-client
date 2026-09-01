using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private bool _isLoading;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string nextScene)
    {
        StartCoroutine(ChangeScene(nextScene));
    }

    private IEnumerator ChangeScene(string nextScene)
    {
        if (_isLoading)
        {
            ClientLogger.Warning(
                $"Scene change already in progress. " +
                $"Ignored={nextScene}");

            yield break;
        }

        _isLoading = true;

        // 씬 전환 시 기존 WarningUI 닫기
        WarningUI.Instance?.Hide();

        Scene currentScene = SceneManager.GetActiveScene();

        // 이미 로드되어 있는지 확인
        Scene existingScene =
            SceneManager.GetSceneByName(nextScene);

        if (existingScene.IsValid() && existingScene.isLoaded)
        {
            ClientLogger.Warning(
                $"Scene already loaded. Scene={nextScene}");

            _isLoading = false;
            yield break;
        }

        ClientLogger.Game(
            $"Loading scene. " +
            $"From={currentScene.name}, To={nextScene}");

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                nextScene,
                LoadSceneMode.Additive
            );

        if (loadOperation == null)
        {
            ClientLogger.Error(
                $"Failed to start scene loading. " +
                $"Scene={nextScene}");

            _isLoading = false;
            yield break;
        }

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        Scene next =
            SceneManager.GetSceneByName(nextScene);

        if (!next.IsValid() || !next.isLoaded)
        {
            ClientLogger.Error(
                $"Scene loading failed. " +
                $"Scene={nextScene}");

            _isLoading = false;
            yield break;
        }

        // Active Scene 변경
        SceneManager.SetActiveScene(next);

        // 이전 씬 제거
        if (currentScene.name != "Bootstrap" &&
            currentScene.IsValid() &&
            currentScene.isLoaded)
        {
            AsyncOperation unloadOperation =
                SceneManager.UnloadSceneAsync(currentScene);

            if (unloadOperation != null)
            {
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }
        }

        _isLoading = false;

        ClientLogger.Game(
            $"Scene change complete. " +
            $"Scene={nextScene}");
    }
}