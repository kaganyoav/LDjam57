using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private Transform topPivot;
    [SerializeField] private Transform bottomPivot;
    
    [SerializeField] private Transform artifactTransform;

    
    [Header("Artifact")]
    private float artifactPosition;
    private float artifactDestination;

    private float artifactTimer;
    [SerializeField] private float timerMultiplier = 3f;

    private float artifactSpeed;
    [SerializeField] private float smoothMotion = 1f;

    [SerializeField] private SpriteRenderer artifactSpriteRenderer;
    
    [Header("Magnet")]
    [SerializeField] private Transform magnetTransform;
    private float magnetPosition;
    [SerializeField] private float magnetSize = 0.1f;
    [SerializeField] private float magnetPower = 5f;
    private float magnetProgress;
    private float magnetPullVelocity;
    [SerializeField] private float magnetPullPower = 0.01f;
    [SerializeField] private float magnetGravityPower = 0.005f;
    [SerializeField] private float magnetProgressDegradationPower = 0.1f;
    
    [Header("Progress")]
    [SerializeField] private Slider progressBar;
    private bool pause = false;

    [SerializeField] private float failTimer = 10f;
    
    [SerializeField] private float difficultyMultiplier = 1f;
    
    private void Start()
    {
        ResizeArtifact(difficultyMultiplier);
    }

    private void Update()
    {
        if(pause) return;
        Artifact();
        Magnet();
        CheckProgress();
    }


    private void CheckProgress()
    {
        float min = magnetPosition - magnetSize / 2;
        float max = magnetPosition + magnetSize / 2;
        
        if (artifactPosition >= min && artifactPosition <= max)
        {
            magnetProgress += magnetPower * Time.deltaTime;
        }
        else
        {
            magnetProgress -= magnetProgressDegradationPower * Time.deltaTime;
            failTimer -= Time.deltaTime;
            if (failTimer <= 0)
            {
                Lose();
            }
        }

        if (magnetProgress > 1)
        {
            Win();
        }

        magnetProgress = Mathf.Clamp(magnetProgress, 0, 1);
        
        progressBar.value = magnetProgress;
    }

    private void Win()
    {
        progressBar.value = 1;
        pause = true;
    }

    private void Lose()
    {
        progressBar.value = 0;
        pause = true;
    }
    
    private void ResizeArtifact(float y = 1f)
    {
        Vector3 localScale = artifactTransform.transform.localScale;
        localScale.y = y;
        artifactTransform.transform.localScale = localScale;
    }

    private void Magnet()
    {
        if (Input.GetMouseButton(0))
        {
            magnetPullVelocity += magnetPullPower * Time.deltaTime;
        }
        magnetPullVelocity -= magnetGravityPower * Time.deltaTime;
        magnetPosition += magnetPullVelocity;
        
        if(magnetPosition - magnetSize/2 <= 0 && magnetPullVelocity < 0)
        {
            magnetPullVelocity = 0;
        }
        if(magnetPosition + magnetSize/2  >= 1f  && magnetPullVelocity > 0)
        {
            magnetPullVelocity = 0;
        }
        
        magnetPosition = Mathf.Clamp(magnetPosition, magnetSize/2, 1-magnetSize/2);
        
        magnetTransform.position = Vector3.Lerp(bottomPivot.position, topPivot.position, magnetPosition);
    }

    private void Artifact()
    {
        artifactTimer -= Time.deltaTime;
        if(artifactTimer <= 0)
        {
            artifactTimer = UnityEngine.Random.value * timerMultiplier;
            artifactDestination = UnityEngine.Random.value;
        }
        artifactPosition = Mathf.SmoothDamp(artifactPosition, artifactDestination, ref artifactSpeed, smoothMotion);
        artifactTransform.position = Vector3.Lerp(bottomPivot.position, topPivot.position, artifactPosition);
    }
}
