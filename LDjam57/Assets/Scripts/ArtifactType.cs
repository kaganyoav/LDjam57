using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactType", menuName = "Scriptable Objects/ArtifactType")]
public class ArtifactType : ScriptableObject
{
    [SerializeField] private List<ArtifactData> artifacts;
    [SerializeField] private List<float> odds;
    public string artifactName;
    public int artifactRealPrice;
    public int artifactSouvenirPrice;
    public float fishingBoxSize = 1f;

    [Header("Sprites")]
    [SerializeField] public Sprite iconSprite;
    [SerializeField] public Sprite drawnSprite;
    [SerializeField] public Sprite silhouetteSprite;
    
    [Header("PriceTags")]
    [SerializeField] public Vector3 realPriceTagPosition;
    [SerializeField] public Vector3 realPriceTagRotation;
    [SerializeField] public Vector3 souvenirPriceTagPosition;
    [SerializeField] public Vector3 souvenirPriceTagRotation;
    
    [Header("Sound")]
    [SerializeField] public EventReference artifactSound;
    
    public ArtifactData GetRandomArtifact()
    {
        if (artifacts.Count == 0 || artifacts.Count != odds.Count)
        {
            Debug.LogError($"ArtifactType {name} has mismatched or empty data.");
            return null;
        }

        float total = 0f;
        foreach (float weight in odds)
            total += weight;

        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < artifacts.Count; i++)
        {
            cumulative += odds[i];
            if (rand <= cumulative)
                return artifacts[i];
        }

        return artifacts[0];
    }
}