using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MenuMusicScript : MonoBehaviour {

    public AudioSource backgroundMusic;

    //public Slider volumeSlider;

    void Awake()
    {
        backgroundMusic = gameObject.GetComponent<AudioSource>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        /*GameObject[] sliders = GameObject.FindGameObjectsWithTag("Slider");
        if (sliders.Length > 2)
        {
            Destroy(sliders[0]);
        }

        if (volumeSlider)
        {
            volumeSlider.value = AudioListener.volume;
        }*/
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            Destroy(this.gameObject);
            //backgroundMusic.Stop();
            //Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            //Cursor.lockState = CursorLockMode.None;
        }
    }

    /*public void adjustVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }*/
}
