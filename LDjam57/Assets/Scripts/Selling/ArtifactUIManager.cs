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

    private void Start()
    {
        choiceButtons.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }
    
    public void SetSlots(List<ArtifactSlot> slots)
    {
        allSlots = slots;
    }

    public void SelectArtifactFromSlot(ArtifactSlot slot)
    {
        selectedSlot = slot;
        selectedArtifact = slot.artifact;
        artifactDisplay.sprite = selectedArtifact.GetArtifactSprite();
        choiceButtons.SetActive(true);
        realPriceText.text = selectedArtifact.GetRealPrice().ToString() + " $";
        souvenirPriceText.text = selectedArtifact.GetSouvenirPrice() + " $";
    }

    public void ChooseReal() => MakeChoice(true);
    public void ChooseSouvenir() => MakeChoice(false);

    private void MakeChoice(bool isReal)
    {
        selectedArtifact.DecidePrice(isReal);
        
        selectedSlot.SetResult(isReal);

        choiceButtons.SetActive(false);
        artifactDisplay.sprite = null;

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

        // Continue to next screen or show final score
    }
    
    public void ShowBook()
    {
        bookPanel.SetActive(true);
        bookPanel.transform.DOLocalMove(bookPanelEndPosition, 1f);
    }
    public void HideBook()
    {
        bookPanel.transform.DOLocalMove(bookPanelStartPosition, 1f).OnComplete(() =>
        {
            bookPanel.SetActive(false);
        });
    }
}