using System.Collections.Generic;
using UnityEngine;

public class ArtifactInventoryLoader : MonoBehaviour
{
    [SerializeField] private ArtifactSlot slotPrefab;
    [SerializeField] private List<Transform> slotContainers;
    [SerializeField] private List<ArtifactData> artifactDataList;
    [SerializeField] private ArtifactUIManager uiManager;

    private List<Artifact> artifacts = new();
    private List<ArtifactSlot> slots = new();

    // void Start()
    // {
    //     int i = 0;
    //     foreach (ArtifactData data in artifactDataList)
    //     {
    //         if(data == null)
    //         {
    //             Debug.LogWarning($"ArtifactData at index {i} is null. Skipping.");
    //             continue;
    //         }
    //         Artifact artifact = new Artifact(data);
    //         artifacts.Add(artifact);
    //
    //         ArtifactSlot slot = Instantiate(slotPrefab, slotContainers[i]);
    //         slot.Initialize(artifact, () => uiManager.SelectArtifactFromSlot(slot));
    //         slots.Add(slot);
    //         i++;
    //     }
    //
    //     uiManager.SetSlots(slots);
    // }
    public void Start()
    {
        LoadArtifacts(GameManager.Instance.playerInventory.artifacts);
    }
    public void LoadArtifacts(List<ArtifactData> newArtifactDataList)
    {
        foreach (ArtifactSlot slot in slots)
        {
            Destroy(slot.gameObject);
        }
        
        artifacts.Clear();
        slots.Clear();

        int i = 0;
        foreach (ArtifactData data in newArtifactDataList)
        {
            if(data == null)
            {
                Debug.LogWarning($"ArtifactData at index {i} is null. Skipping.");
                continue;
            }
            Artifact artifact = new Artifact(data);
            artifacts.Add(artifact);

            ArtifactSlot slot = Instantiate(slotPrefab, slotContainers[i]);
            slot.Initialize(artifact, () => uiManager.SelectArtifactFromSlot(slot));
            slots.Add(slot);
            i++;
        }

        uiManager.SetSlots(slots);
    }
}
