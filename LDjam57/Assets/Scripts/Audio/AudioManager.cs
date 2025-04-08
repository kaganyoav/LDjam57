using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public enum MusicType
{
    None,
    Menu,
    Fishing,
    Selling,
    Results,
    Win,
    Lose
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private List<EventInstance> eventInstances;

    private EventInstance menuMusic;
    private EventInstance fishingMusic;
    private EventInstance sellingMusic;
    private EventInstance resultsMusic;
    private EventInstance winMusic;
    private EventInstance loseMusic;
    private EventInstance ambientInstance;

    private EventInstance currentMusicInstance;
    private MusicType currentMusic = MusicType.None;

    [SerializeField] private bool noMusic;
    [SerializeField] private bool noAmbient;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadBanksAndPlayMusic());
    }
        
    private IEnumerator LoadBanksAndPlayMusic()
    {
        // Wait for FMOD banks to load
        while (!RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null; // Wait until all banks are loaded
        }

        // Ensure FMOD is fully initialized before playing sounds
        yield return new WaitForSeconds(0.1f);
        eventInstances = new List<EventInstance>();

        menuMusic = CreateEventInstance(FMODEvents.Instance.menuMusic);
        fishingMusic = CreateEventInstance(FMODEvents.Instance.fishingMusic);
        sellingMusic = CreateEventInstance(FMODEvents.Instance.sellingMusic);
        resultsMusic = CreateEventInstance(FMODEvents.Instance.resultsMusic);
        winMusic = CreateEventInstance(FMODEvents.Instance.winMusic);
        loseMusic = CreateEventInstance(FMODEvents.Instance.loseMusic);
        ambientInstance = CreateEventInstance(FMODEvents.Instance.ambience);
    }
    

    private EventInstance CreateEventInstance(EventReference reference)
    {
        EventInstance instance = RuntimeManager.CreateInstance(reference);
        eventInstances.Add(instance);
        return instance;
    }

    
    public void PlayMusic(MusicType type)
    {
        if (noMusic || type == currentMusic) return;
        
        StopCurrentMusic();

        switch (type)
        {
            case MusicType.Menu:
                currentMusicInstance = menuMusic;
                break;
            case MusicType.Fishing:
                currentMusicInstance = fishingMusic;
                break;
            case MusicType.Selling:
                currentMusicInstance = sellingMusic;
                break;
            case MusicType.Results:
                currentMusicInstance = resultsMusic;
                break;
            case MusicType.Win:
                currentMusicInstance = winMusic;
                break;
            case MusicType.Lose:
                currentMusicInstance = loseMusic;
                break;
            default:
                return; 
        }

        currentMusic = type;
        currentMusicInstance.start();
    }

    public void StopCurrentMusic()
    {
        if (noMusic || currentMusic == MusicType.None) return;

        currentMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        currentMusic = MusicType.None;
    }

    public void StartAmbient()
    {
        if (noAmbient) return;
        ambientInstance.start();
    }
    
    public void StopAmbient()
    {
        if (noAmbient) return;
        ambientInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        Debug.Log("Playing sound: " + sound);
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    // private void OnDestroy()
    // {
    //     foreach (EventInstance e in eventInstances)
    //     {
    //         e.stop(STOP_MODE.IMMEDIATE);
    //         e.release();
    //     }
    // }
}
