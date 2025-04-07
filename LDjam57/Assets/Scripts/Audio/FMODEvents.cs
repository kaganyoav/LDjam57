using System;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Parameters")]
    [field: SerializeField] public float ghostCount;

    [field: SerializeField] public float intensity;
    
    [field: Header("Music")]
    [field: SerializeField] public EventReference menuMusic { get; private set; }
    [field: SerializeField] public EventReference fishingMusic { get; private set; }
    [field: SerializeField] public EventReference sellingMusic { get; private set; }
    [field: SerializeField] public EventReference resultsMusic { get; private set; }
    
    [field: Header("Throw SFX")]
    [field: SerializeField] public EventReference toss { get; private set; }
    [field: SerializeField] public EventReference tossWeak { get; private set; }
    [field: SerializeField] public EventReference tossMid { get; private set; }
    [field: SerializeField] public EventReference tossStrong { get; private set; }
    [field: SerializeField] public EventReference tossWindup { get; private set; }
    
    [field: Header("Water SFX")]
    [field: SerializeField] public EventReference waterImpact { get; private set; }
    [field: SerializeField] public EventReference artifactEmerge { get; private set; }
    
    [field: Header("Magnet SFX")]
    [field: SerializeField] public EventReference magnetImpact { get; private set; }
    
    [field: Header("Book SFX")]
    [field: SerializeField] public EventReference bookOpen { get; private set; }
    [field: SerializeField] public EventReference bookClose { get; private set; }
    [field: SerializeField] public EventReference bookFlipPrev { get; private set; }
    [field: SerializeField] public EventReference bookFlipNext { get; private set; }
    
    
    [field: Header("Artifact SFX")]
    [field: SerializeField] public EventReference coinMove { get; private set; }
    [field: SerializeField] public EventReference ringMove { get; private set; }
    [field: SerializeField] public EventReference pendantMove { get; private set; }
    [field: SerializeField] public EventReference idolMove { get; private set; }
    [field: SerializeField] public EventReference maskMove { get; private set; }
    
    [field: Header("Results SFX")]
    [field: SerializeField] public EventReference souvenirSouvenir { get; private set; }
    [field: SerializeField] public EventReference souvenirReal { get; private set; }
    [field: SerializeField] public EventReference realSouvenir { get; private set; }
    [field: SerializeField] public EventReference realReal { get; private set; }
    
    [field: Header("Dialog SFX")]
    [field: SerializeField] public Dialog[] driverDialog { get; private set; }
    [field: SerializeField] public Dialog[] bossDialog { get; private set; }
    [field: SerializeField] public EventReference skipDialog { get; private set; }

    
    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

[Serializable]
public class Dialog
{
    [field: SerializeField] public EventReference[] levelDialog { get; private set; }
}
