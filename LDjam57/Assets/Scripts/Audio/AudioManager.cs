using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;
    
    public static AudioManager Instance;
    
    private EventInstance menuMusic;

    [SerializeField] private bool noMusic;
    [SerializeField] private bool noAmbient;
    
    [SerializeField] private bool isMainMenu = false;
    [SerializeField] private bool isWinScene = false;
    [SerializeField] private bool isLoseScene = false;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        eventInstances = new List<EventInstance>();
        // menuMusic = CreateEventInstance(FMODEvents.Instance.menuMusic);
    }

    private void Start()
    {
        if (noMusic) return;

        if (isMainMenu)
        {
            menuMusic.start();
        }
    }
    public void StopMainMenuMusic()
    {
        if (noMusic) return;
        
        menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
    
    private void OnDestroy()
    {
        CleanUp();
    }
    
    // public void StartMusic()
    // {
    //     if (noMusic) return;
    //     
    //     musicInstance.start();
    // }
    //
    // public void StopMusic()
    // {
    //     if (noMusic) return;
    //     
    //     musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    // }

    public void SetGhostCountParameter(float value)
    {
        FMODEvents.Instance.ghostCount = value;
        RuntimeManager.StudioSystem.setParameterByName("GhostsFollowing", value);
    }
    
    public void SetIntensityParameter(float value)
    {
        FMODEvents.Instance.intensity = value;
        RuntimeManager.StudioSystem.setParameterByName("Intensity", value);
    }
    
    // public void StartAmbient()
    // {
    //     if (noAmbient) return;
    //     ambientInstance.start();
    // }
    //
    // public void StopAmbient()
    // {
    //     if (noAmbient) return;
    //     ambientInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    // }
    
    

}
