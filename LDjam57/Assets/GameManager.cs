using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string menuSceneName = "MenuScene";
    public string fishingSceneName = "Fishing";
    public string sellingSceneName = "Selling";
    public string pawnShopSceneName = "PawnShop";
    
    public static GameManager Instance { get; private set; }
    
    [Header("Player")]
    public PlayerInventory playerInventory;
    
    [Header("Throws")]
    public int numOfThrows = 5;
    public int currentThrow = 0;
    
    [Header("Managers")]
    [SerializeField] private ThrowingManager throwingManager;
    [SerializeField] private FishingManager fishingManager;
    
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
        FishingPhase();
    }

    private void FishingPhase()
    {
        currentThrow = 0;
        playerInventory.ClearArtifacts();
        throwingManager.EnableThrowing();
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
        // Handle transition to fishing phase
    }
    
    private void TransitionToSellingPhase()
    {
        // VISUAL TRANSITION
        SceneManager.LoadScene(sellingSceneName);
        Debug.Log("Transitioning to Selling Phase");
    }
}
