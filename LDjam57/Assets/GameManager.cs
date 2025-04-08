using System;
using System.Collections;
using DG.Tweening;
using FMODUnity;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string fishingSceneName = "Fishing";
    public string sellingSceneName = "Selling";
    public string pawnShopSceneName = "PawnShop";
    public string winSceneName = "WinScene";
    public string loseSceneName = "LoseScene";
    
    public static GameManager Instance { get; private set; }
    
    [Header("Player")]
    public PlayerInventory playerInventory;
    
    [Header("Throws")]
    public int numOfThrows = 5;
    public int currentThrow = 0;

    [Header("Day")]
    [SerializeField] private TMP_Text dayText;
    public int currentDay = 0;
    public int maxDays = 7;
    
    [Header("Currency")] 
    [SerializeField] private TMP_Text currencyText;
    public int playerCurrency = 0;
    public int goalCurrency = 1000;
    
    [Header("Managers")]
    [SerializeField] private ThrowingManager throwingManager;
    [SerializeField] private FishingManager fishingManager;
    
    [Header("Color")]
    [SerializeField] public ColorData colorData;
    
    [Header("Dialog")]
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private string[] dialogText;
    [SerializeField] private string winnerDialogText;
    [SerializeField] private string loserDialogText;

    [Header("Menu")] [SerializeField] private GameObject Black;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Salamanca")]
    [SerializeField] private Animator salamncaAnimator;
    
    [Header("END")]
    [SerializeField] private GameObject endUI;
    
    public bool testGameManager = false;
    public bool gameWon = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        StartCoroutine(LoadBanksAndPlayMusic());
    }
        
    private IEnumerator LoadBanksAndPlayMusic()
    {
        if (testGameManager) yield break;
        // Wait for FMOD banks to load
        while (!RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null; // Wait until all banks are loaded
        }

        // Ensure FMOD is fully initialized before playing sounds
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayMusic(MusicType.Menu);
    }
    
    private void Start()
    {
        if(testGameManager) return;
        menuUI.SetActive(true);
        StartCoroutine(LoadBanksAndPlayMusic());
    }
    

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        AudioManager.Instance.StopCurrentMusic();
        menuUI.SetActive(false);
        FishingPhase();
    }
    
    //FISHING PHASE
    private void FishingPhase()
    {
        currentThrow = 0;
        playerInventory.ClearArtifacts();
        UpdateDayText();
        StartCoroutine(BeginningOfDayCoroutine());
    }

    private IEnumerator BeginningOfDayCoroutine()
    {
        AudioManager.Instance.StartAmbient();
        //ENEMY BUSH ANIMATION
        yield return new WaitForSeconds(1f);
        salamncaAnimator.SetTrigger("show");
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bush, transform.position);
        yield return new WaitForSeconds(1.48f);
        salamncaAnimator.SetBool("talking", true);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.cartelDialog[currentDay], transform.position);
        dialogManager.ShowLine(dialogText[currentDay]);
        yield return new WaitUntil(() => !dialogManager.IsDisplaying());
        salamncaAnimator.SetBool("talking", false);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bush, transform.position);
        yield return new WaitForSeconds(1.48f);
        Vector3 cameraStartPos = new Vector3(0, 0, -10);
        Transform cam = Camera.main.transform;
        cam.DOMove(cameraStartPos, 5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            throwingManager.StartDay(currentDay);
            AudioManager.Instance.PlayMusic(MusicType.Fishing);
        });
    }

    public void EndThrow(ArtifactData artifactData)
    {
        if (currentThrow >= numOfThrows)
        {
            Debug.Log("No more throws left.");
            return;
        }
        playerInventory.SetArtifact(artifactData, currentThrow);

        currentThrow++;
        
        if (currentThrow == numOfThrows)
        {
            Debug.Log("All throws completed.");
            TransitionToSellingPhase();
            // Handle end of fishing phase
        }
        else
        {
            throwingManager.EnableThrowing();
        }
    }
    
    private void TransitionToFishingPhase()
    {
        dayText.gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnFishingSceneLoaded;
        SceneManager.LoadScene(fishingSceneName);
    }

    private void OnFishingSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == fishingSceneName)
        {
            throwingManager = FindObjectOfType<ThrowingManager>();
            fishingManager = FindObjectOfType<FishingManager>();
            salamncaAnimator = throwingManager.salamncaAnimator;

            FishingPhase();

            // Important: Unsubscribe to avoid multiple calls!
            SceneManager.sceneLoaded -= OnFishingSceneLoaded;
        }
    }
    
    
    //SELLING PHASE
    private void TransitionToSellingPhase()
    {
        AudioManager.Instance.StopAmbient();
        dayText.gameObject.SetActive(false);
        SceneManager.sceneLoaded += OnSellingSceneLoaded;
        SceneManager.LoadScene(sellingSceneName);
        Debug.Log("Transitioning to Selling Phase");
    }

    private void OnSellingSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sellingSceneName)
        {
            // If you have any selling-related managers, find them here.
            AudioManager.Instance.PlayMusic(MusicType.Selling);
            SceneManager.sceneLoaded -= OnSellingSceneLoaded;
        }
    }
    
    public void EndSellingPhase()
    {
        AudioManager.Instance.StopCurrentMusic();
        // Handle end of selling phase
        TransitionToPawnShopPhase();
    }
    
    
    //PAWNSHOP PHASE
    private void TransitionToPawnShopPhase()
    {
        SceneManager.sceneLoaded += OnPawnShopSceneLoaded;
        SceneManager.LoadScene(pawnShopSceneName);
        Debug.Log("Transitioning to Pawn Shop Phase");
    }

    private void OnPawnShopSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == pawnShopSceneName)
        {
            // If you have any pawn shop-related managers, find them here.
            AudioManager.Instance.PlayMusic(MusicType.Results);
            SceneManager.sceneLoaded -= OnPawnShopSceneLoaded;
        }
    }
    
    public void EndPawnShopPhase()
    {
        AudioManager.Instance.StopCurrentMusic();
        // Handle end of pawn shop phase
        currentDay++;
        if (playerCurrency >= goalCurrency)
        {
            WinGame();
        }
        else if (currentDay >= maxDays)
        {
            LoseGame();
        }
        else
        {
            TransitionToFishingPhase();
        }
    }

    private void WinGame()
    {
        TransitionToEnd(true);
    }
    private void LoseGame()
    {
        TransitionToEnd(false);
    }
    
    public void TransitionToEnd(bool win)
    {
        gameWon = win;
        SceneManager.sceneLoaded += OnEndLoaded;
        SceneManager.LoadScene(fishingSceneName);
    }

    private void OnEndLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == fishingSceneName)
        {
            throwingManager = FindObjectOfType<ThrowingManager>();
            fishingManager = FindObjectOfType<FishingManager>();
            salamncaAnimator = throwingManager.salamncaAnimator;
            if (gameWon)
            {
                StartCoroutine(WinCoroutine());
            }
            else
            {
                StartCoroutine(LoseCoroutine());
            }

            // Important: Unsubscribe to avoid multiple calls!
            SceneManager.sceneLoaded -= OnEndLoaded;
        }
    }
    
    private IEnumerator WinCoroutine()
    {
        AudioManager.Instance.StartAmbient();
        //ENEMY BUSH ANIMATION
        yield return new WaitForSeconds(1f);
        salamncaAnimator.SetTrigger("show");
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bush, transform.position);
        yield return new WaitForSeconds(1.48f);
        salamncaAnimator.SetBool("talking", true);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.cartelDialog[currentDay], transform.position);
        dialogManager.ShowLine(dialogText[currentDay]);
        yield return new WaitUntil(() => !dialogManager.IsDisplaying());
        salamncaAnimator.SetBool("talking", false);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bush, transform.position);
        yield return new WaitForSeconds(1.48f);
        AudioManager.Instance.StopAmbient();
        AudioManager.Instance.PlayMusic(MusicType.Win);
        endUI.SetActive(true);
    }
    
    private IEnumerator LoseCoroutine()
    {
        AudioManager.Instance.StartAmbient();
        //ENEMY BUSH ANIMATION
        yield return new WaitForSeconds(1f);
        salamncaAnimator.SetTrigger("show");
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bush, transform.position);
        yield return new WaitForSeconds(1.48f);
        salamncaAnimator.SetBool("talking", true);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.cartelDialog[currentDay], transform.position);
        dialogManager.ShowLine(dialogText[currentDay]);
        yield return new WaitUntil(() => !dialogManager.IsDisplaying());
        salamncaAnimator.SetBool("talking", false);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bush, transform.position);
        yield return new WaitForSeconds(1.48f);
        AudioManager.Instance.StopAmbient();
        AudioManager.Instance.PlayMusic(MusicType.Win);
        endUI.SetActive(true);
    }
    
    //Currency
    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        currencyText.text = "$" + playerCurrency.ToString();
    }
    
    //Day
    public void UpdateDayText()
    {
        dayText.text = "Day " + (currentDay+1);
    }

    public void QuitGame()
    {
        // Save any game data if necessary
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
