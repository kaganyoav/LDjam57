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

    void Start()
    {
        int i = 0;
        foreach (ArtifactData data in artifactDataList)
        {
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
