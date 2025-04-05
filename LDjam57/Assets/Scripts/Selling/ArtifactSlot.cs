using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSlot : MonoBehaviour
{
    public Image artifactImage;
    public Image resultIcon; // Small icon to show real/souvenir
    public TMP_Text chosenPriceText;
    public Artifact artifact;
    
    private System.Action onClick;

    public void Initialize(Artifact artifact, System.Action clickCallback)
    {
        this.artifact = artifact;
        artifactImage.sprite = artifact.GetArtifactSprite();
        resultIcon.gameObject.SetActive(false);
        onClick = clickCallback;
    }

    public void SetResult(bool isReal)
    {
        resultIcon.gameObject.SetActive(true);
        resultIcon.color = isReal ? Color.green : Color.red;
        chosenPriceText.text = isReal ? artifact.GetRealPrice().ToString() : artifact.GetSouvenirPrice().ToString();
        chosenPriceText.text+= " $";
    }
    
    public void OnClick()
    {
        onClick?.Invoke();
    }
}
