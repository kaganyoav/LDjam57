using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PawnShopManager : MonoBehaviour
{
    [Header("Scales")] [SerializeField] private float reactionScale = 1f;
    [SerializeField] private float artifactScale = 0.25f;
    [SerializeField] private float priceScale = 0.5f;

    
    [Header("UI Panels")]
    [SerializeField] private GameObject pawnShopUI;
    [SerializeField] private Image artifactDisplay;

    [Header("Colors")]
    [SerializeField] private Color realPriceColor;
    [SerializeField] private Color souvenirPriceColor;

    [Header("Player Price UI")]
    [SerializeField] private GameObject yourPricePanel;
    [SerializeField] private Image yourPriceOutline;
    [SerializeField] private TMP_Text yourPriceText;

    [Header("Real Price UI")]
    [SerializeField] private GameObject realPricePanel;
    [SerializeField] private Image realPriceOutline;
    [SerializeField] private TMP_Text realPriceText;

    [Header("Reaction")]
    [SerializeField] private SpriteRenderer reactionImage;
    [SerializeField] private Sprite souvenirSouvenirSprite;
    [SerializeField] private Sprite souvenirRealSprite;
    [SerializeField] private Sprite realRealSprite;
    [SerializeField] private Sprite realSouvenirSprite;

    [Header("Player Money")]
    [SerializeField] private TMP_Text moneyText;

    [Header("Next Button")]
    [SerializeField] private Button nextButton;

    private int totalMoney;
    private bool continuePressed = false;

    private void Start()
    {
        realPriceColor = GameManager.Instance.colorData.realColor;
        souvenirPriceColor = GameManager.Instance.colorData.souvenirColor;
        totalMoney = 0;
        pawnShopUI.SetActive(false);
        UpdateMoneyText();

        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextPressed);

        StartCoroutine(PawnShopCoroutine());
    }

    IEnumerator PawnShopCoroutine()
    {
        nextButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => continuePressed);
        
        pawnShopUI.SetActive(true);
        
        int i = 0;
        foreach (ArtifactData data in GameManager.Instance.playerInventory.artifacts)
        {
            if (data == null)
            {
                Debug.LogWarning("ArtifactData is null. Skipping.");
                continue;
            }

            bool playerGuess = GameManager.Instance.playerInventory.playerGuesses[i];
            i++;

            // Reset UI
            artifactDisplay.sprite = data.artifactSprite;
            artifactDisplay.color = new Color(1, 1, 1, 0);
            yourPricePanel.transform.localScale = Vector3.zero;
            realPricePanel.transform.localScale = Vector3.zero;
            reactionImage.transform.localScale = Vector3.zero;
            yourPriceText.text = "";
            realPriceText.text = "";
            nextButton.gameObject.SetActive(false);
            continuePressed = false;

            // Fade in artifact
            artifactDisplay.DOFade(1f, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // Show player's price guess
            int guessedPrice = playerGuess
                ? data.artifactType.artifactRealPrice
                : data.artifactType.artifactSouvenirPrice;

            yourPriceText.text = guessedPrice + " $";
            yourPriceOutline.color = playerGuess ? realPriceColor : souvenirPriceColor;
            yourPriceText.color = playerGuess ? realPriceColor : souvenirPriceColor;
            yourPricePanel.transform.DOScale(priceScale, 0.4f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1f);

            // Show real price
            int realPrice = data.isReal
                ? data.artifactType.artifactRealPrice
                : data.artifactType.artifactSouvenirPrice;

            realPriceText.text = realPrice + " $";
            realPriceOutline.color = data.isReal ? realPriceColor : souvenirPriceColor;
            realPriceText.color = data.isReal ? realPriceColor : souvenirPriceColor;
            realPricePanel.transform.DOScale(priceScale, 0.4f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1f);

            // Show reaction
            bool correct = data.isReal == playerGuess;
            ShowReaction(data.isReal, playerGuess);
            
            // Add money
            if (correct)
            {
                totalMoney += realPrice;
                UpdateMoneyText();
            }

            // Wait for player to press "Next"
            nextButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => continuePressed);
        }

        Debug.Log("Pawn Shop Complete. Total: " + totalMoney);
        nextButton.gameObject.SetActive(false);
        // Optionally: load a summary or transition to next scene here
    }

    private void ShowReaction(bool isReal, bool playerGuess)
    {
        if (playerGuess && isReal)
        {
            reactionImage.sprite = realRealSprite;
        }
        else if (playerGuess && !isReal)
        {
            reactionImage.sprite = realSouvenirSprite;
        }
        else if (!playerGuess && isReal)
        {
            reactionImage.sprite = souvenirRealSprite;
        }
        else // !playerGuess && !isReal
        {
            reactionImage.sprite = souvenirSouvenirSprite;
        }

        reactionImage.transform.DOScale(reactionScale, 0.4f).From(0f).SetEase(Ease.OutBack);
    }

    private void UpdateMoneyText()
    {
        moneyText.text = "Money: " + totalMoney + " $";
    }

    private void OnNextPressed()
    {
        continuePressed = true;
        nextButton.gameObject.SetActive(false);
    }
}
