using TMPro;
using UnityEngine;

public class Artifact
{
    private readonly ArtifactData artifactData;

    public bool isReal;
    public bool guessedAsReal;
    public bool playerHasGuessed;
    public Sprite artifactSprite;
    
    public Artifact(ArtifactData data)
    {
        artifactData = data;
        isReal = data.isReal;
        guessedAsReal = false;
        playerHasGuessed = false;
    }
    
    public string GetArtifactName()
    {
        return artifactData.artifactType.artifactName;
    }
    public int GetRealPrice()
    {
        return artifactData.artifactType.artifactRealPrice;
    }

    public int GetSouvenirPrice()
    {
        return artifactData.artifactType.artifactSouvenirPrice;
    }
    
    public Sprite GetArtifactSprite()
    {
        return artifactData.artifactSprite;
    }
    
    public void DecidePrice(bool selectedReal)
    {
        guessedAsReal = selectedReal;
        playerHasGuessed = true;
    }
}
