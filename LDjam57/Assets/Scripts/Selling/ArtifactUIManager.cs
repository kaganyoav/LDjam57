using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ArtifactUIManager : MonoBehaviour
{
    [SerializeField] private Image artifactDisplay;
    [SerializeField] private GameObject choiceButtons;
    [SerializeField] private TMP_Text realPriceText;
    [SerializeField] private TMP_Text souvenirPriceText;

    [SerializeField] private Button continueButton;

    private Artifact selectedArtifact;
    private ArtifactSlot selectedSlot;

    [SerializeField] private List<ArtifactSlot> allSlots;
    
    [Header("Book")]
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private Vector3 bookPanelStartPosition;
    [SerializeField] private Vector3 bookPanelEndPosition;
    [SerializeField] private Button showBookButton;
    [SerializeField] private Button hideBookButton;
    [SerializeField] private float bookPanelMoveDuration = 1f;
    
    
    [Header("Timer")]
    [SerializeField] private float totalTime = 90f; // time in seconds
    [SerializeField] private TMP_Text timerText;    // optional UI to display time

    private float remainingTime;
    private bool timerRunning = false;
    private bool timeExpired = false;
    
    private void Start()
    {
        choiceButtons.SetActive(false);
        continueButton.gameObject.SetActive(false);

        remainingTime = totalTime;
        timerRunning = true;
        timeExpired = false;
    }
    
    private void Update()
    {
        if (timerRunning && !timeExpired)
        {
            remainingTime -= Time.deltaTime;

            // Update UI text (optional)
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(remainingTime).ToString() + "s";

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                timeExpired = true;
                timerRunning = false;
                OnTimeExpired();
            }
        }
    }
    
    public void SetSlots(List<ArtifactSlot> slots)
    {
        allSlots = slots;
    }

    public void SelectArtifactFromSlot(ArtifactSlot slot)
    {
        if (timeExpired) return;
        
        selectedSlot = slot;
        selectedArtifact = slot.artifact;
        artifactDisplay.sprite = selectedArtifact.GetArtifactSprite();
        choiceButtons.SetActive(true);
        realPriceText.text = selectedArtifact.GetRealPrice().ToString() + " $";
        souvenirPriceText.text = selectedArtifact.GetSouvenirPrice().ToString() + " $";
    }

    public void ChooseReal() => MakeChoice(true);
    public void ChooseSouvenir() => MakeChoice(false);

    private void MakeChoice(bool isReal)
    {
        if (timeExpired) return;

        selectedArtifact.DecidePrice(isReal);
        selectedSlot.SetResult(isReal);

        choiceButtons.SetActive(false);

        if (AllArtifactsGuessed())
            continueButton.gameObject.SetActive(true);
    }

    private bool AllArtifactsGuessed()
    {
        return allSlots.All(slot => slot.artifact.playerHasGuessed);
    }

    public void OnContinue()
    {
        foreach (var slot in allSlots)
        {
            bool correct = slot.artifact.guessedAsReal == slot.artifact.isReal;
            Debug.Log($"{slot.artifact.GetArtifactName()}: {(correct ? "Correct" : "Wrong")}");
            // Optional: Show result next to item
        }
        
        GameManager.Instance.EndSellingPhase();
        // Continue to next screen or show final score
    }
    
    public void ShowBook()
    {
        showBookButton.gameObject.SetActive(false);
        hideBookButton.gameObject.SetActive(true);
        bookPanel.transform.DOLocalMove(bookPanelEndPosition, bookPanelMoveDuration).SetEase(Ease.OutFlash);
    }
    public void HideBook()
    {
        hideBookButton.gameObject.SetActive(false);
        showBookButton.gameObject.SetActive(true);
        bookPanel.transform.DOLocalMove(bookPanelStartPosition, bookPanelMoveDuration).SetEase(Ease.InFlash);
    }
    
    private void OnTimeExpired()
    {
        Debug.Log("Time is up!");

        choiceButtons.SetActive(false);
        // Optionally gray out the UI or lock all slots
        foreach (var slot in allSlots)
        {
            if (!slot.artifact.playerHasGuessed)
            {
                slot.artifact.DecidePrice(false);
                slot.SetResult(false);
            }
        }
        
        // You can also auto-continue or highlight unanswered artifacts
        continueButton.gameObject.SetActive(true); // if you want to let them continue anyway
    }
}