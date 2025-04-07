using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactOdds", menuName = "Scriptable Objects/ArtifactOdds")]
public class ArtifactOdds : ScriptableObject
{

    [SerializeField] private List<float> closeRangeOdds = new List<float> { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
    [SerializeField] private List<ArtifactType> closeRangeTypes;

    [SerializeField] private List<float> middleRangeOdds = new List<float> { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
    [SerializeField] private List<ArtifactType> middleRangeTypes;

    [SerializeField] private List<float> farRangeOdds = new List<float> { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f };
    [SerializeField] private List<ArtifactType> farRangeTypes;


    [SerializeField] private ArtifactData defaultArtifact;

    public ArtifactData GetArtifactData(float throwPower)
    {
        List<float> odds;
        List<ArtifactType> types;

        if (throwPower < 0.5f)
        {
            odds = closeRangeOdds;
            types = closeRangeTypes;
        }
        else if (throwPower < 0.8f)
        {
            odds = middleRangeOdds;
            types = middleRangeTypes;
        }
        else
        {
            odds = farRangeOdds;
            types = farRangeTypes;
        }

        ArtifactType selectedType = GetRandomWeightedType(odds, types);
        return selectedType != null ? selectedType.GetRandomArtifact() : defaultArtifact;
    }

    private ArtifactType GetRandomWeightedType(List<float> odds, List<ArtifactType> types)
    {
        if (odds.Count != types.Count || odds.Count == 0)
        {
            Debug.LogError("Artifact odds and type lists are mismatched or empty!");
            return null;
        }

        float total = 0f;
        foreach (float chance in odds)
            total += chance;

        float randomValue = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < odds.Count; i++)
        {
            cumulative += odds[i];
            if (randomValue <= cumulative)
                return types[i];
        }

        return types[0];
    }
}
