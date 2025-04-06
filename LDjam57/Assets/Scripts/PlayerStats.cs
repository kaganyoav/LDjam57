using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Currency")]
    public int currency = 0;
    
    [Header("Fishing Stats")]
    public int numOfThrows = 3;
    public int maxNumOfThrows = 5;

    public float ropeLength = 5f;
    public float maxRopeLength = 10f;
    
    public float rarityMultiplier = 1f;
    public float maxRarityMultiplier = 2f;
    
    public float realMultiplier = 1f;
    public float maxRealMultiplier = 2f;
}

