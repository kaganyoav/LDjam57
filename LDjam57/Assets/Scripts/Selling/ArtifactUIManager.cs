using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;


public class ArtifactUIManager : MonoBehaviour
{
    [Header("Artifact")]
    [SerializeField] private Vector3 artifactDisplayStartPosition;
    [SerializeField] private Vector3 artifactDisplayEndPosition;
    [SerializeField] private Image artifactDisplay;
    
    [Header("Buttons")]
    [SerializeField] private GameObject choiceButtons;
    [SerializeField] private Transform realButton;
    [SerializeField] private Transform souvenirButton;
    [SerializeField] private TMP_Text realPriceText;
    [SerializeField] private TMP_Text souvenirPriceText;

    [SerializeField] private Button continueButton;

    private Artifact selectedArtifact;
    private ArtifactSlot selectedSlot;

    [SerializeField] private List<ArtifactSlot> allSlots;
    private int currentSlotIndex = 0;
    private int totalSlots = 0;
    
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
        if (slots.Count > 0)
        {
            totalSlots = slots.Count;
            currentSlotIndex = 0;
            SelectArtifactFromSlot(slots[0]);
        }
    }

    public void SelectArtifactFromSlot(ArtifactSlot slot)
    {
        if (timeExpired) return;
        
        currentSlotIndex = slot.slotIndex;
        
        // choiceButtons.SetActive(false);
        choiceButtons.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.OutFlash);
        //play sound
        if (selectedArtifact != null)
        {
            EventReference sound = selectedArtifact.GetSound();
            AudioManager.Instance.PlayOneShot(sound, transform.position);
        }
        artifactDisplay.transform.DOLocalMove(artifactDisplayStartPosition, 1.2f).SetEase(Ease.InFlash).onComplete = () =>
        {
            selectedSlot = slot;
            selectedArtifact = slot.artifact;
            artifactDisplay.sprite = selectedArtifact.GetArtifactSprite();
            //play sound
            EventReference sound = selectedArtifact.GetSound();
            AudioManager.Instance.PlayOneShot(sound, transform.position);
            
            artifactDisplay.transform.DOLocalMove(artifactDisplayEndPosition, 1.2f).SetEase(Ease.OutBack, 0.5f).onComplete = () =>
            {
                choiceButtons.SetActive(true);
                realPriceText.text = selectedArtifact.GetRealPrice().ToString() + " $";
                souvenirPriceText.text = selectedArtifact.GetSouvenirPrice().ToString() + " $";
                choiceButtons.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutFlash);
            };
        };
    }

    public void ChooseReal() => MakeChoice(true);
    public void ChooseSouvenir() => MakeChoice(false);

    private void MakeChoice(bool isReal)
    {
        if (timeExpired) return;

        //play sound
        if (isReal)
        {
            EventReference sound = FMODEvents.Instance.chooseReal;
            AudioManager.Instance.PlayOneShot(sound, transform.position);
        }
        else
        {
            EventReference sound = FMODEvents.Instance.chooseSouvenir;
            AudioManager.Instance.PlayOneShot(sound, transform.position);
        }

        selectedArtifact.DecidePrice(isReal);
        selectedSlot.SetResult(isReal);

        // choiceButtons.SetActive(false);

        if (AllArtifactsGuessed())
            continueButton.gameObject.SetActive(true);
        
        SelectArtifactFromSlot(allSlots[(currentSlotIndex+1)%totalSlots]);
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
        //play sound
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bookOpen, transform.position);
        bookPanel.transform.DOLocalMove(bookPanelEndPosition, bookPanelMoveDuration).SetEase(Ease.OutFlash);
    }
    public void HideBook()
    {
        hideBookButton.gameObject.SetActive(false);
        showBookButton.gameObject.SetActive(true);
        //play sound
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bookClose, transform.position);
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