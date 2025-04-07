using System;
using System.Collections;
using DG.Tweening;
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
    
    [Header("Menu")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Transform cameraTransform;
    
    public bool testGameManager = false;
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
    }
    
    private void Start()
    {
        menuUI.SetActive(true);
        AudioManager.Instance.PlayMusic(MusicType.Menu);
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
        yield return new WaitForSeconds(2f);
        dialogManager.ShowLine(dialogText[currentDay]);
        yield return new WaitUntil(() => !dialogManager.IsDisplaying());
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
        SceneManager.LoadScene(winSceneName);
    }
    private void LoseGame()
    {
        SceneManager.LoadScene(loseSceneName);
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
}
