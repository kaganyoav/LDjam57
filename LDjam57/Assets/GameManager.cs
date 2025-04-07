using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string menuSceneName = "MenuScene";
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
        if (testGameManager) return;
        TransitionToFishingPhase();
    }
    
    //FISHING PHASE

    private void FishingPhase()
    {

        currentThrow = 0;
        playerInventory.ClearArtifacts();
        UpdateDayText();
        throwingManager.StartDay(currentDay);
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
            SceneManager.sceneLoaded -= OnSellingSceneLoaded;
        }
    }
    
    public void EndSellingPhase()
    {
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
            SceneManager.sceneLoaded -= OnPawnShopSceneLoaded;
        }
    }
    
    public void EndPawnShopPhase()
    {
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
