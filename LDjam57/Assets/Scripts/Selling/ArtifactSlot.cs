using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ArtifactSlot : MonoBehaviour
{
    public Image artifactImage;
    public Image squareBackground;
    public Image priceTag; // Small icon to show real/souvenir
    public Image priceTagOutline; // Outline of the price tag
    public TMP_Text chosenPriceText;
    public Artifact artifact;
    public int slotIndex;
    
    private System.Action onClick;

    [SerializeField] public Sprite defaultSquare;
    [SerializeField] public Sprite realSquare;
    [SerializeField] public Sprite souvenirSquare;

    private Color realColor;
    private Color souvenirColor;
    
    public void Initialize(Artifact artifact,int index, System.Action clickCallback)
    {
        this.artifact = artifact;
        artifactImage.sprite = artifact.GetSilhouetteSprite();
        squareBackground.sprite = defaultSquare;
        priceTag.gameObject.SetActive(false);
        onClick = clickCallback;
        slotIndex = index;
        realColor = GameManager.Instance.colorData.realColor;
        souvenirColor = GameManager.Instance.colorData.souvenirColor;
        priceTag.transform.localScale = Vector3.zero;
    }

    public void SetResult(bool isReal)
    {
        priceTag.gameObject.SetActive(true);
        squareBackground.sprite = isReal ? realSquare : souvenirSquare;
        priceTagOutline.color = isReal ? realColor : souvenirColor;
        chosenPriceText.text = isReal ? artifact.GetRealPrice().ToString() : artifact.GetSouvenirPrice().ToString();
        chosenPriceText.color = isReal ? realColor : souvenirColor;
        GameManager.Instance.playerInventory.GuessArtifact(slotIndex, isReal);
        chosenPriceText.text+= " $";
        priceTag.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        float rnd = Random.value;
        int rotation = rnd > 0.5 ? 15 : -15;
        priceTag.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
    
    public void OnClick()
    {
        onClick?.Invoke();
    }
}
