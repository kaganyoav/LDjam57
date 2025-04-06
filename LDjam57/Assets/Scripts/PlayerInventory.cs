using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Scriptable Objects/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{ 
    public List<ArtifactData> artifacts = new List<ArtifactData> { null, null, null, null, null };

    
    public void SetArtifact(ArtifactData artifact,int index)
    {
        if (index < 0 || index >= artifacts.Count)
        {
            Debug.LogError($"Index {index} is out of range for artifacts list.");
            return;
        }

        if (artifacts[index] != null)
        {
            Debug.LogWarning($"Overwriting existing artifact at index {index}.");
        }

        artifacts[index] = artifact;
    }
    
    public void RemoveArtifact(int index)
    {
        if (index < 0 || index >= artifacts.Count)
        {
            Debug.LogError($"Index {index} is out of range for artifacts list.");
            return;
        }

        if (artifacts[index] == null)
        {
            Debug.LogWarning($"No artifact to remove at index {index}.");
            return;
        }

        artifacts[index] = null;
    }
    
    public void ClearArtifacts()
    {
        for (int i = 0; i < artifacts.Count; i++)
        {
            artifacts[i] = null;
        }
    }
}
