using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour {

    //la hacemos pública porque la queremos hacer accesible para otros scripts
    //la hacemos estática porque no queremos que haga referencia a este script. Queremos que los otros scripts puedan checkear fácilmente si estamos en pause o no
    public static bool gameIsPaused = false;
    
    public GameObject pauseMenuUIGO;
    public GameObject settingsPanelUIGO;
    public GameObject menuConfirmationPanelUIGO;
    public GameObject restartConfirmationPanelUIGO;
    public GameObject quitConfirmationPanelUIGO;
    public GameObject finalRestartConfirmationPanelUIGO;
    public GameObject finalMenuConfirmationPanelUIGO;
    public bool confirmationPanelOpen = false;

    public AudioMixerSnapshot paused;
    public AudioMixerSnapshot unpaused;
    public AudioMixer masterAudioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;

    private bool muteToggleIsChecked = false;
    private float musicVolume;
    private float sfxVolume;


    void Start()
    {
        gameIsPaused = false;
    }

    void Update ()
    {
        if (!confirmationPanelOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gameIsPaused)
                {
                    Resume();
                    unpaused.TransitionTo(0.01f);
                }
                else
                {
                    Pause();
                    paused.TransitionTo(0.01f);
                }
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideSettingsPanel();
                HideRestartConfirmationPanel();
                HideMenuConfirmationPanel();
                HideQuitConfirmationPanel();
            }
        }

        musicVolume = musicSlider.value;
        sfxVolume = sfxSlider.value;
    }

    //queremos ocultar todos los elementos de la UI del menú de pausa
    //queremos que el tiempo vuelva a la normalidad, por eso timeScale = 1
    public void Resume()
    {
        pauseMenuUIGO.SetActive(false); //para activar el gameObject de pause menu
        GameIsUnpaused();
    }

    //queremos mostrar todos los elementos de la UI del menú de pausa
    //queremos congelar el tiempo, por eso timeScale = 0
    void Pause() 
    {
        pauseMenuUIGO.SetActive(true); //para desactivar el gameObject de pause menu
        GameIsPaused();
    }


    public void LoadMenu()
    {
        GameIsUnpaused();
        masterAudioMixer.ClearFloat("MusicVolume");
        masterAudioMixer.ClearFloat("SFXVolume");
        //StartCoroutine(DelaySceneLoad(0));
        SceneManager.LoadScene(0);
    }


    public void ShowMenuConfirmationPanel()
    {
        confirmationPanelOpen = true;
        menuConfirmationPanelUIGO.SetActive(true);
        pauseMenuUIGO.SetActive(false);
    }


    public void HideMenuConfirmationPanel()
    {
        confirmationPanelOpen = false;
        menuConfirmationPanelUIGO.SetActive(false);
        pauseMenuUIGO.SetActive(true);
    }


    public void ShowFinalMenuConfirmationPanel()
    {
        confirmationPanelOpen = true;
        finalMenuConfirmationPanelUIGO.SetActive(true);
        pauseMenuUIGO.SetActive(false);
    }


    public void HideFinalMenuConfirmationPanel()
    {
        confirmationPanelOpen = false;
        finalMenuConfirmationPanelUIGO.SetActive(false);
    }


    public void ShowSettingsPanel()
    {
        confirmationPanelOpen = true;
        settingsPanelUIGO.SetActive(true);
        pauseMenuUIGO.SetActive(false);
    }


    public void HideSettingsPanel()
    {
        confirmationPanelOpen = false;
        settingsPanelUIGO.SetActive(false);
        pauseMenuUIGO.SetActive(true);
    }


    public void RestartScene()
    {
        GameIsUnpaused();
        masterAudioMixer.ClearFloat("MusicVolume");
        masterAudioMixer.ClearFloat("SFXVolume");
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }


    public void ShowRestartConfirmationPanel()
    {
        confirmationPanelOpen = true;
        restartConfirmationPanelUIGO.SetActive(true);
        pauseMenuUIGO.SetActive(false);
    }


    public void HideRestartConfirmationPanel()
    {
        confirmationPanelOpen = false;
        restartConfirmationPanelUIGO.SetActive(false);
        pauseMenuUIGO.SetActive(true);
    }


    public void ShowFinalRestartConfirmationPanel()
    {
        confirmationPanelOpen = true;
        finalRestartConfirmationPanelUIGO.SetActive(true);
        pauseMenuUIGO.SetActive(false);
    }

    public void HideFinalRestartConfirmationPanel()
    {
        confirmationPanelOpen = false;
        finalRestartConfirmationPanelUIGO.SetActive(false);
    }


    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //si le damos al botón de Quit en Unity, parará de jugar
#else
        Application.Quit(); //si le damos Quit fuera de Unity, cerrará el programa
#endif
    }


    public void ShowQuitConfirmationPanel()
    {
        confirmationPanelOpen = true;
        quitConfirmationPanelUIGO.SetActive(true);
        pauseMenuUIGO.SetActive(false);
    }


    public void HideQuitConfirmationPanel()
    {
        confirmationPanelOpen = false;
        quitConfirmationPanelUIGO.SetActive(false);
        pauseMenuUIGO.SetActive(true);
    }


    void GameIsUnpaused()
    {
        //masterAudioMixer.ClearFloat("MasterVolume");        al salir de pausa, el Mastervolume se desmutea érroneamente
        Time.timeScale = 1; //volvemos a poner el tiempo normal
        gameIsPaused = false;
        unpaused.TransitionTo(0.01f); //subimos el volumen de la música cuando estemos en pausa
    }


    void GameIsPaused()
    {
        Time.timeScale = 0; //paramos el tiempo
        gameIsPaused = true;
        paused.TransitionTo(0.01f); //bajamos el volumen de la música cuando estemos en pausa
    }


    IEnumerator DelaySceneLoad(int sceneNum)
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneNum);
    }


    public void SetSFXVolumeLevel()
    {
        masterAudioMixer.SetFloat("SFXVolume", sfxVolume);
    }

    public void SetMusicVolumeLevel()
    {
        masterAudioMixer.SetFloat("MusicVolume", musicVolume);
    }

    public void SetMasterVolumeLevel()
    {
        muteToggleIsChecked = !muteToggleIsChecked;

        if (muteToggleIsChecked) //si está checked (true), muteamos el sonido en todo el juego
        {
            masterAudioMixer.SetFloat("MasterVolume", -80f);
        }
        else //si no está checked (false), devolvemos el volumen a como estaba antes
        {
            masterAudioMixer.ClearFloat("MasterVolume"); //esto lo usamos para devolver al AudioMixer a su valor anterior (en este caso, al estar pausado, es -10)
            //Al usar los Exposed Parameters y settearles un valor específico, me cargo por completo los Snapshots. Con ClearFloat, es como hacer un Reset y así reactivo de nuevo los Snapshots
            //masterAudioMixer.SetFloat("MasterVolume", masterVolume);
        }
    }
}
