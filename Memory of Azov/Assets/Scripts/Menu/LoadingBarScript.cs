using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class LoadingBarScript : MonoBehaviour {

    public GameObject loadingScreenObject;  //este objeto le paso el texto y la Slider como un solo GO
    public Slider mySlider;
    public Text loadingText;

    AsyncOperation async; 

    public void LoadScreenExample(int level) //will be used by the button we are going to press, so we must make sure it is public
    {//we use a parameter "int level" to choose the level we want to load
        StartCoroutine(LoadingScreen(level));
    }

    IEnumerator LoadingScreen(int lvl)
    {
        loadingScreenObject.SetActive(true); //we activate the loadingScreen object
        async = SceneManager.LoadSceneAsync(lvl); //we start to load the level in the background
        async.allowSceneActivation = false; //and we set this to false
        loadingText.text = "Loading...";


        while (!async.isDone)
        {
            mySlider.value = async.progress; //we set the slider value to progress value. When level is fully loaded, progress equals 0.9f
            if (async.progress == 0.9f) //if it is 0.9, i.e., fully loaded.
            {
                mySlider.value = 1f; //set the slider value to 1f.
                async.allowSceneActivation = true; //set allowSceneActivation to true, which will set async.isDone to true and we will get out of the while loop
                //and, therefore, we will switch to the new level.
                loadingText.text = mySlider.value * 100f + "%";
            }
            yield return null;
        }
    }
}
