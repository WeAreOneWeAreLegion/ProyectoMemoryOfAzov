using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager> {

    public enum TypeOfTag { Player, Enemy, Wall, Door, DoorTrigger, Bell, HittableObject, FakeWall }
    public enum ButtonRequest { A, B, X, Y, RB, LB, RT, LT }

    #region Public Variables
    [Header("\t--Game Designers Variables--")]
    [Tooltip("Muestra los fps, en caso contrario los esconde automaticamente")]
    public bool showFPS;

    [Header("Materials Variables")]
    [Tooltip("Distancia extra para no hacer invisible los objetos cuando estan a la misma distancia frontal que el personaje principal")]
    [Range(0,20)] public float transparencyOffsetForward = 1f;
    [Tooltip("Distancia extra para no hacer invisible los objetos cuando estan a la misma distancia lateral que el personaje principal")]
    [Range(0,20)] public float transparencyOffsetLateral = 1f;
    [Tooltip("La cantidad de transparencia a tener los objetos que la camara esconde")]
    [Range(0,1)] public float objectsHidenByCameraTransparency = 0.2f;
    [Tooltip("La cantidad de transparencia a tener los muros que la camara esconde")]
    [Range(0,1)] public float wallsHidenByCameraTransparency = 0f;

    [Header("\t    --Own Script Variables--")]
    [Header("Player Variables")]
    [Tooltip("Referencia del player")]
    public PlayerController player;

    [Header("Main Light")]
    [Tooltip("Referencia de la luz direccional")]
    public Light directionalLight;

    [Header("Rooms Variables")]
    [Tooltip("Referencia a todas las habitacions")]
    public List<GameObject> roomsList = new List<GameObject>();
    [Tooltip("Referencia a todas las puertas")]
    public List<ConectionScript> doorsList = new List<ConectionScript>();
    [Tooltip("Referencia a todas las puertas")]
    public List<FakeWallScript> fakeWallsList = new List<FakeWallScript>();

    [Header("HUD Variables")]
    //Main Canvas
    [Tooltip("Referencia al canvas principal")]
    public Canvas mainCanvas;
    //References
    [Tooltip("Referencia al hud del player")]
    public PlayerHUD playerHUD;
    [Tooltip("Referencia al hud del enemigo")]
    public GameObject enemyHUDPrefab;
    [Tooltip("Referencia al hud de pausa")]
    public GameObject pausePanel;
    //Componens
    [Tooltip("Referencia al texto de vida")]
    public Text hpText;
    [Tooltip("Referencia a la sombra del texto de vida")]
    public Text hpTextShadow;
    [Tooltip("Referencia al texto de gemas")]
    public Text gemsText;
    [Tooltip("Referencia a la sombra del texto de gemas")]
    public Text gemsTextShadow;
    [Tooltip("Referencia al texto de control vertical actual")]
    public Text currentYControlMode;
    [Tooltip("Referencia al texto de fps")]
    public Text fpsText;

    [Header("Tags List")]
    [Tooltip("0.Player, 1.Enemy, 2.Wall, 3.Door 4.DoorTrigger 5.Bell 6.HittableObjets 7.FakeWall")]
    public List<string> tagList = new List<string>();
    #endregion

    public List<GameObject> allgems = new List<GameObject>();

    [Header("\t--Pause Menu Variables--")]
    [Tooltip("")]
    public GameObject pauseMenuGO;
    public GameObject restartConfirmationPanel;
    public GameObject settingsPanel;
    public GameObject menuConfirmationPanel;
    public GameObject quitConfirmationPanel;
    //public GameObject finalRestartConfirmationPanel;
    //public GameObject finalMenuConfirmationPanel;
    public bool confirmationPanelOpen = false;

    #region Private Variables
    private bool combateMode;
    private float deltaTime;
    private bool isGamePaused;

    //Persistance variables
    private int maxNumOfGems = 6;
    private int currentNumOfGems = 0;
    private int currentNumOfGhosts = 0;
    private float gameTimeStart;
    private int currentHealthLost = 0;

    private List<EnemyHUD> enemyHUDList = new List<EnemyHUD>();
    private List<EnemyHUD> enemyHUDWaitingList = new List<EnemyHUD>();

    //Pause Panel Variables
    //private int actionIndex = 0; ------------ Provisional, don't erase
    #endregion

    private void Awake()
    {
        Time.timeScale = 1;
        gameTimeStart = Time.timeSinceLevelLoad;
    }

    private void Start()
    {
        pausePanel.SetActive(false);

        //Set-up HUD
        if (!showFPS)
            fpsText.gameObject.SetActive(false);
        else
            fpsText.gameObject.SetActive(true);

    }

    private void Update()
    {
        if (showFPS)
            ShowFPS();

        if (InputsManager.Instance.GetStartButtonDown())
            PauseGame();

        if (isGamePaused)
            PauseActions();
    }

    #region Door Methods
    public void UnblockPlayerDoors()
    {
        combateMode = false;

        player.CalmMode();

        Ray ray;
        RaycastHit hit;

        ray = new Ray(GetPlayer().position, Vector3.down);
        Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

        foreach (ConectionScript d in doorsList)
        {
            if (d.IsDoorFromRoom(hit.transform.parent.gameObject))
            {
                d.UnblockDoor();
            }
        }

        foreach (FakeWallScript f in fakeWallsList)
        {
            if (f.IsDoorFromRoom(hit.transform.parent.gameObject))
            {
                f.UnblockDoor();
            }
        }
    }

    public void ShowAndHideDoors()
    {
        Ray ray;
        RaycastHit hit;

        ray = new Ray(GetPlayer().position, Vector3.down);
        Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

        foreach (ConectionScript d in doorsList)
        {
            if (d.IsDoorFromRoom(hit.transform.parent.gameObject))
            {
                d.ShowVisualDoor();
            }
            else
            {
                d.HideVisualDoor();
            }
        }
    }

    public void ShowAllDoors()
    {
        foreach (ConectionScript d in doorsList)
        {
            d.ShowVisualDoor();
        }
    }

    public void BlockPlayerDoors()
    {
        combateMode = true;

        player.CombateMode();

        Ray ray;
        RaycastHit hit;

        ray = new Ray(GetPlayer().position, Vector3.down);
        Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("FloorLayer"));

        foreach (ConectionScript d in doorsList)
        {
            if (d.IsDoorFromRoom(hit.transform.parent.gameObject))
            {
                d.BlockDoor();
            }
        }
    }
    #endregion

    #region Light Methods
    public void SwitchMainLight()
    {
        directionalLight.enabled = !directionalLight.enabled;
        if (directionalLight.enabled)
            RenderSettings.ambientIntensity = 1;
        else
            RenderSettings.ambientIntensity = 0;
    }
    #endregion

    #region HUD Methods
    public void ModifyHp(int currentHp)
    {
        hpText.text = currentHp.ToString();
        hpTextShadow.text = hpText.text;
    }

    public void ActivePlayerHUD(ButtonRequest req)
    {
        playerHUD.ShowSpecificButton(req);
    }

    public void CreateEnemyHUD(Transform target, int initialHp)
    {
        if (enemyHUDWaitingList.Count == 0)
        {
            GameObject go = Instantiate(enemyHUDPrefab, mainCanvas.transform) as GameObject;
            EnemyHUD e = go.GetComponent<EnemyHUD>();
            e.SetUp(target, initialHp);
            enemyHUDList.Add(e);
        }
        else
        {
            EnemyHUD e = enemyHUDWaitingList[0];
            enemyHUDWaitingList.Remove(e);
            e.gameObject.SetActive(true);
            e.SetUp(target, initialHp);
            enemyHUDList.Add(e);
        }
    }

    public void ModifyEnemyHp(Transform target, int currentHp)
    {
        enemyHUDList.Find(x => x.GetTarget() == target).ModifyHp(currentHp);
    }

    public void DestroyEnemyHUD(Transform target)
    {
        enemyHUDList.ForEach(x =>
        {
            if (x.GetTarget() == target)
            {
                x.gameObject.SetActive(false);
                enemyHUDWaitingList.Add(x);
                enemyHUDList.Remove(x);
            }
        });
    }

    public void DisablePlayerHUD()
    {
        playerHUD.HideImage();
    }
    #endregion

    #region Persistance Modify Methods
    public void IncreaseMaxGemsQuantity(GameObject spawner)
    {
        //allgems.Add(spawner);
        //maxNumOfGems++;
        gemsText.text = currentNumOfGems.ToString() + "/" + maxNumOfGems.ToString();
        gemsTextShadow.text = gemsText.text;
    }

    public void IncreaseNumOfGems()
    {
        currentNumOfGems++;
        gemsText.text = currentNumOfGems.ToString()+"/"+maxNumOfGems.ToString();
        gemsTextShadow.text = gemsText.text;

        if (currentNumOfGems >= maxNumOfGems)
        {
            CallPlayerVictory();
        }
    }

    public void IncreseNumOfGhostsCaptured()
    {
        currentNumOfGhosts++;
    }

    public void IncreaseHealthLost(int healthLost)
    {
        currentHealthLost += healthLost;
    }
    #endregion

    #region Getter Methods
    public bool GetIsPlayerPanelActive()
    {
        return playerHUD.GetIsActive();
    }

    public bool GetIsInCombateMode()
    {
        return combateMode;
    }

    public bool IsDirectLightActivated()
    {
        return directionalLight.enabled;
    }

    public string GetTagOfDesiredType(TypeOfTag t)
    {
        return tagList[(int)t];
    }

    public Transform GetPlayer()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        return player.transform;
    }

    public Vector2 GetCanvasResolution()
    {
        return mainCanvas.GetComponent<CanvasScaler>().referenceResolution;
    }
    #endregion

    //Setters

    #region FPS Method
    private void ShowFPS()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        //float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "FPS: " + ((int)fps).ToString();
    }
    #endregion

    #region Pause Methods
    private void PauseActions()
    {

    }

    private void PauseGame()
    {
        if (!confirmationPanelOpen)
        {
            if (isGamePaused)
            {
                Time.timeScale = 1;
                pausePanel.SetActive(false);
                pauseMenuGO.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                pausePanel.SetActive(true);
                pauseMenuGO.SetActive(true);
            }

            isGamePaused = !isGamePaused;
        }
    }

    public void Resume()
    {
        isGamePaused = false;
        pausePanel.SetActive(false);
        pauseMenuGO.SetActive(false);
        Time.timeScale = 1;
    }

    #region Restart Button
    public void RestartScene()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void ShowRestartConfirmationPanel()
    {
        confirmationPanelOpen = true;
        restartConfirmationPanel.SetActive(true);
        pauseMenuGO.SetActive(false);
    }

    public void HideRestartConfirmationPanel()
    {
        confirmationPanelOpen = false;
        restartConfirmationPanel.SetActive(false);
        pauseMenuGO.SetActive(true);
    }
    #endregion

    #region Menu Button
    public void LoadMenu()
    {
        isGamePaused = false;
        SceneManager.LoadScene(0);
    }

    public void ShowMenuConfirmationPanel()
    {
        confirmationPanelOpen = true;
        menuConfirmationPanel.SetActive(true);
        pauseMenuGO.SetActive(false);
    }

    public void HideMenuConfirmationPanel()
    {
        confirmationPanelOpen = false;
        menuConfirmationPanel.SetActive(false);
        pauseMenuGO.SetActive(true);
    }
    #endregion

    #region Settings Button
    public void ShowSettingsPanel()
    {
        confirmationPanelOpen = true;
        settingsPanel.SetActive(true);
        pauseMenuGO.SetActive(false);
    }

    public void HideSettingsPanel()
    {
        confirmationPanelOpen = false;
        settingsPanel.SetActive(false);
        pauseMenuGO.SetActive(true);
    }
    #endregion

    #region Quit Button
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //si le damos al botón de Quit en Unity, parará de jugar
#else
        Application.Quit(); //si le damos Quit fuera de Unity, cerrará el programa
#endif
    }


    public void ShowQuitConfirmationPanel()
    {
        confirmationPanelOpen = true;
        quitConfirmationPanel.SetActive(true);
        pauseMenuGO.SetActive(false);
    }

    public void HideQuitConfirmationPanel()
    {
        confirmationPanelOpen = false;
        quitConfirmationPanel.SetActive(false);
        pauseMenuGO.SetActive(true);
    }
    #endregion
    #endregion

    #region Game State Methods
    public void CallPlayerDeath()
    {
        //Game Over
        Debug.Log("You have lost");
        Debug.Log("Quantity of ghost hunted: " + currentNumOfGhosts);
        Debug.Log("Quantity of diamond eggs found: " + currentNumOfGems);
        Debug.Log("Quantity of health lost: " + currentHealthLost);
        Debug.Log("Time played: " + (Time.timeSinceLevelLoad - gameTimeStart).ToString());
        Time.timeScale = 0;
    }

    public void CallPlayerVictory()
    {
        //Victory
        Debug.Log("A winner is you");
        Debug.Log("Quantity of ghost hunted: "+currentNumOfGhosts);
        Debug.Log("Quantity of diamond eggs found: "+currentNumOfGems);
        Debug.Log("Quantity of health lost: "+currentHealthLost);
        Debug.Log("Time played: "+(Time.timeSinceLevelLoad - gameTimeStart).ToString());
        Time.timeScale = 0;
    }
    #endregion
}
