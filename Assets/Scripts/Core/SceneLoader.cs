using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static IEnumerator ChangeScene(string nextScene)
    {
        Scene currentScene = SceneManager.GetActiveScene();


        // 다음 씬 로드
        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                nextScene,
                LoadSceneMode.Additive
            );


        while (!loadOperation.isDone)
        {
            yield return null;
        }


        Scene next =
            SceneManager.GetSceneByName(nextScene);


        // Active Scene 변경
        SceneManager.SetActiveScene(next);


        // 이전 씬 제거
        if (currentScene.name != "Bootstrap")
        {
            AsyncOperation unloadOperation =
                SceneManager.UnloadSceneAsync(currentScene);


            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}