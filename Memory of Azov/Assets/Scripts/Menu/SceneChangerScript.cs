using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneChangerScript : MonoBehaviour {


    public void GoToStartGameScene()
    {
        StartCoroutine(DelaySceneLoad(1));
    }

    public void GoToHowToPlayScene()
    {
        StartCoroutine(DelaySceneLoad(3));
    }

    public void GoToCreditsScene()
    {
        StartCoroutine(DelaySceneLoad(2));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //si le damos al botón de Quit en Unity, parará de jugar
#else
        Application.Quit(); //si le damos Quit fuera de Unity, cerrará el programa
#endif
    }

    public void GoBackToMenu()
    {
        StartCoroutine(DelaySceneLoad(0));
    }

    public void GoToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoToPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    IEnumerator DelaySceneLoad(int sceneNum)
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneNum);
    }
}
