using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactData", menuName = "Scriptable Objects/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public ArtifactType artifactType;
    public bool isReal;
    public Sprite artifactSprite;
}
