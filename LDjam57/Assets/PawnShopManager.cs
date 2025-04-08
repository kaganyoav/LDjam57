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
    [SerializeField] private Image artifactDisplay;

    [Header("Colors")]
    [SerializeField] private Color realPriceColor;
    [SerializeField] private Color souvenirPriceColor;

    [SerializeField] private TMP_Text yourPriceText;
    [SerializeField] private TMP_Text realPriceText;
    [SerializeField] private TMP_Text receivedText;

    [Header("Next Button")]
    [SerializeField] private Button nextButton;

    private int totalMoney;
    private bool continuePressed = false;
    
    [Header("Clipboard")]
    [SerializeField] private GameObject clipboard;
    [SerializeField] private Vector3 clipboardStartPosition;
    [SerializeField] private Vector3 clipboardEndPosition;
    [SerializeField] private float clipboardMoveDuration = 1.5f;
    [SerializeField] private GameObject extraPage;
    [SerializeField] private Image extraPageImage;
    [SerializeField] private Vector3 extraPageStartPosition;
    [SerializeField] private Vector3 extraPageEndPosition;
    [SerializeField] private float extraPageMoveDuration = 1f;

    private void Start()
    {
        realPriceColor = GameManager.Instance.colorData.realColor;
        souvenirPriceColor = GameManager.Instance.colorData.souvenirColor;
        totalMoney = 0;

        nextButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextPressed);

        StartCoroutine(PawnShopCoroutine());
    }

    IEnumerator PawnShopCoroutine()
    {
        nextButton.gameObject.SetActive(true);
        // yield return new WaitUntil(() => continuePressed);
        
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
            extraPageImage.sprite = data.artifactType.drawnSprite;
            artifactDisplay.color = new Color(1, 1, 1, 1);
            yourPriceText.text = "";
            realPriceText.text = "";
            receivedText.text = "";
            nextButton.gameObject.SetActive(false);
            continuePressed = false;
            
            clipboard.transform.DOLocalMove(clipboardEndPosition, clipboardMoveDuration).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(clipboardMoveDuration);
            
            // Fade in artifact
            // artifactDisplay.DOFade(1f, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // Show player's price guess
            int guessedPrice = playerGuess
                ? data.artifactType.artifactRealPrice
                : data.artifactType.artifactSouvenirPrice;

            yourPriceText.text = guessedPrice + " $";
            // AudioManager.Instance.PlayOneShot(FMODEvents.Instance.namedPrice, transform.position);
            // yourPriceOutline.color = playerGuess ? realPriceColor : souvenirPriceColor;
            yourPriceText.color = playerGuess ? realPriceColor : souvenirPriceColor;
                
            yield return new WaitForSeconds(1f);

            // Show real price
            int realPrice = data.isReal
                ? data.artifactType.artifactRealPrice
                : data.artifactType.artifactSouvenirPrice;
            
            realPriceText.text = realPrice + " $";
            // AudioManager.Instance.PlayOneShot(FMODEvents.Instance.namedPrice, transform.position);

            // realPriceOutline.color = data.isReal ? realPriceColor : souvenirPriceColor;
            realPriceText.color = data.isReal ? realPriceColor : souvenirPriceColor;

            yield return new WaitForSeconds(1f);
            
            // Show received money
            int recived = CalculateReceivedMoney(data.isReal, playerGuess, data);
            receivedText.text = recived + " $";
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.namedPrice, transform.position);
            receivedText.color = data.isReal ? realPriceColor : souvenirPriceColor;
            yield return new WaitForSeconds(1f);
            
            // Show reaction
            DetermineOutcome(data.isReal, playerGuess, data);
            
            extraPage.transform.DOLocalMove(extraPageEndPosition, extraPageMoveDuration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(extraPageMoveDuration);
            
            // Wait for player to press "Next"
            nextButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => continuePressed);
            extraPage.transform.DOLocalMove(extraPageStartPosition, extraPageMoveDuration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(extraPageMoveDuration);
            clipboard.transform.DOLocalMove(clipboardStartPosition, clipboardMoveDuration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(clipboardMoveDuration);
        }

        nextButton.gameObject.SetActive(false);
        // Optionally: load a summary or transition to next scene here
        GameManager.Instance.EndPawnShopPhase();
    }

    private int CalculateReceivedMoney(bool dataIsReal, bool playerGuess, ArtifactData data)
    {
        int received = 0;
        if (playerGuess && dataIsReal)
        {
            received = data.artifactType.artifactRealPrice;
        }
        else if (playerGuess && !dataIsReal)
        {
            received = 0;
        }
        else if (!playerGuess && dataIsReal)
        {
            received = data.artifactType.artifactSouvenirPrice;
        }
        else // !playerGuess && !dataIsReal
        {
            received = data.artifactType.artifactSouvenirPrice;
        }
        return received;
    }

    private void DetermineOutcome(bool isReal, bool playerGuess, ArtifactData data)
    {
        // Real Real
        if (playerGuess && isReal)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.realReal, transform.position);
            GameManager.Instance.AddCurrency(data.artifactType.artifactRealPrice);
        }
        
        // Real Fake
        else if (playerGuess && !isReal)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.realSouvenir, transform.position);
        }
        
        // Fake Real
        else if (!playerGuess && isReal)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.souvenirReal, transform.position);
            GameManager.Instance.AddCurrency(data.artifactType.artifactSouvenirPrice);
        }
        
        // Fake Fake
        else 
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.souvenirSouvenir, transform.position);
            GameManager.Instance.AddCurrency(data.artifactType.artifactSouvenirPrice);
        }

    }

    private void ShowReaction(bool isReal, bool playerGuess)
    {
        if (playerGuess && isReal)
        {
        }
        else if (playerGuess && !isReal)
        {
        }
        else if (!playerGuess && isReal)
        {
        }
        else // !playerGuess && !isReal
        {
        }

    }
    

    private void OnNextPressed()
    {
        continuePressed = true;
        // nextButton.gameObject.SetActive(false);
    }
}
