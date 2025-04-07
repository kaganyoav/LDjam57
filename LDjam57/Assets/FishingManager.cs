using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FishingManager : MonoBehaviour
{
    [Header("Bar")]
    [SerializeField] private Transform barTransform;
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
    [SerializeField] private SpriteRenderer artifactBoxSpriteRenderer;
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
    
    [SerializeField] private ThrowingManager throwingManager;

    private void Awake()
    {
        barTransform.gameObject.SetActive(false);
    }

    public void StartMiniGame(ArtifactData artifactData, float catchX)
    {
        //set artifact features according to player stats
        barTransform.gameObject.SetActive(true);
        barTransform.position = new Vector3(catchX - 0.5f, barTransform.position.y, barTransform.position.z);
        // artifactSpriteRenderer.sprite = artifactData.artifactSprite;
        artifactPosition = 0.5f;
        artifactDestination = 0.7f;
        artifactTimer = UnityEngine.Random.value * timerMultiplier;
        magnetPosition = 0.1f;
        magnetProgress = 0;
        failTimer = 10f;
        float artifactSize = artifactData.artifactType.fishingBoxSize;
        ResizeArtifact(artifactSize);
        pause = false;
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

        bool isInMagnetZone = artifactPosition >= min && artifactPosition <= max;

        if (isInMagnetZone)
        {
            magnetProgress += magnetPower * Time.deltaTime;
        }
        else
        {
            magnetProgress -= magnetProgressDegradationPower * Time.deltaTime;
            failTimer -= Time.deltaTime;

            if (failTimer <= 0)
            {
                Debug.Log("Fail timer expired");
                EndMiniGame(false);
            }
        }

        if (magnetProgress > 1)
        {
            Debug.Log("Success");
            EndMiniGame(true);
        }

        magnetProgress = Mathf.Clamp(magnetProgress, 0, 1);
        progressBar.value = magnetProgress;

        // 🌈 Smoothly transition color
        Color targetColor = isInMagnetZone ? Color.green : Color.red;
        artifactSpriteRenderer.color = Color.Lerp(artifactSpriteRenderer.color, targetColor, Time.deltaTime * 10f);
    }
    
    private void EndMiniGame(bool win)
    {
        pause = true;
        progressBar.value = win ? 1 : 0;
        barTransform.gameObject.SetActive(false);
        throwingManager.MinigameOver(win);
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
        
        magnetPosition = Mathf.Clamp(magnetPosition, 0, 1);
        
        magnetTransform.position = Vector3.Lerp(bottomPivot.position, topPivot.position, magnetPosition);
    }

    private void Artifact()
    {
        // artifactTimer -= Time.deltaTime;
        // if(artifactTimer <= 0)
        // {
        //     artifactTimer = UnityEngine.Random.value * timerMultiplier;
        //     artifactDestination = UnityEngine.Random.value;
        // }
        // artifactPosition = Mathf.SmoothDamp(artifactPosition, artifactDestination, ref artifactSpeed, smoothMotion);
        // artifactTransform.position = Vector3.Lerp(bottomPivot.position, topPivot.position, artifactPosition);
         artifactTimer -= Time.deltaTime;
        if (artifactTimer <= 0)
        {
            artifactTimer = UnityEngine.Random.Range(0.2f, 0.6f); // faster change
            float variation = UnityEngine.Random.Range(0.3f, 1f);
            float direction = UnityEngine.Random.value > 0.5f ? 1f : -1f;
            artifactDestination = Mathf.Clamp01(artifactPosition + direction * variation);
        }

        artifactPosition = Mathf.SmoothDamp(artifactPosition, artifactDestination, ref artifactSpeed, smoothMotion);
        artifactTransform.position = Vector3.Lerp(bottomPivot.position, topPivot.position, artifactPosition);
    }
}
