using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactData", menuName = "Scriptable Objects/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    public int artifactRealPrice;
    public int artifactSouvenirPrice;
    public float artifactRarity;
    public bool isReal;
    public Sprite artifactSprite;
}
