using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSlot : MonoBehaviour
{
    public Image artifactImage;
    public Image priceTag; // Small icon to show real/souvenir
    public Image priceTagOutline; // Outline of the price tag
    public TMP_Text chosenPriceText;
    public Artifact artifact;
    public int slotIndex;
    
    private System.Action onClick;

    public void Initialize(Artifact artifact,int index, System.Action clickCallback)
    {
        this.artifact = artifact;
        artifactImage.sprite = artifact.GetSilhouetteSprite();
        priceTag.gameObject.SetActive(false);
        onClick = clickCallback;
        slotIndex = index;
    }

    public void SetResult(bool isReal)
    {
        priceTag.gameObject.SetActive(true);
        priceTagOutline.color = isReal ? Color.green : Color.red;
        chosenPriceText.text = isReal ? artifact.GetRealPrice().ToString() : artifact.GetSouvenirPrice().ToString();
        chosenPriceText.color = isReal ? Color.green : Color.red;
        GameManager.Instance.playerInventory.GuessArtifact(slotIndex, isReal);
        chosenPriceText.text+= " $";
        
    }
    
    public void OnClick()
    {
        onClick?.Invoke();
    }
}
