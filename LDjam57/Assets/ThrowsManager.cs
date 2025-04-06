using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ThrowingManager : MonoBehaviour
{
    [SerializeField] ArtifactOdds artifactOdds;
    
    [SerializeField] private GameObject powerBar;
    [SerializeField] private Image powerBarFill;
    [SerializeField] private float fillSpeed = 1.5f;
    [SerializeField] private FishingManager fishingManager;
    [SerializeField] private float resetTime = 0.5f;

    private float currentFill = 0f;
    private bool isFilling = false;
    private bool goingUp = true;

    
    private bool canReceiveInput = false;
    
    [SerializeField] private Gradient fillGradient;
    [SerializeField] private AnimationCurve easeInCurve;
    
    [SerializeField] private float minThrowX = 4f;
    [SerializeField] private float maxThrowX = -7.5f;
    
    [Header("Rope")]
    [SerializeField] private Transform ropeEnd;
    [SerializeField] private Rope2DGenerator rope2DGenerator;
    
    [Header("Magnet")]
    private Vector3 magnetStartPos;
    [SerializeField] private Transform magnetTransform;
    [SerializeField] private Rigidbody2D magnetRb;
    [SerializeField] private float seaBottomY = -2.5f;
    
    private bool isMinigameActive = false;
    
    [Header("Line")]
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private int ropeSegmentCount = 20;
    [SerializeField] private float ropeSlack = 1.5f; // how much dip in the middle
    
    private void Awake()
    {
        powerBar.SetActive(false);
        powerBarFill.fillAmount = 0f;
        magnetStartPos = magnetTransform.position;
        ropeLine.positionCount = ropeSegmentCount;
    }
    
    private void Start()
    {
        StartCoroutine(EnableInputAfterDelay(0.1f)); // Delay for safety
    }

    IEnumerator EnableInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canReceiveInput = true;
    }
    
    void Update()
    {
        if (!canReceiveInput || isMinigameActive) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            powerBar.SetActive(true);
            StartFilling();
        }

        if (isFilling)
        {
            UpdateFill();
        }

        if (Input.GetMouseButtonUp(0) && isFilling)
        {
            StopFilling();
            StartCoroutine(WaitAndDissapear());
        }
    }
    
    void LateUpdate()
    {
        UpdateRope();
    }

    IEnumerator WaitAndDissapear()
    {
        yield return new WaitForSeconds(resetTime);
        powerBar.SetActive(false);
    }

    void StartFilling()
    {
        isFilling = true;
        goingUp = true;
        currentFill = 0f;
    }

    void UpdateFill()
    {
        float delta = fillSpeed * Time.deltaTime * (goingUp ? 1 : -1);
        currentFill += delta * easeInCurve.Evaluate(currentFill);

        if (currentFill >= 1f)
        {
            currentFill = 1f;
            goingUp = false;
        }
        else if (currentFill <= 0f)
        {
            currentFill = 0f;
            goingUp = true;
        }

        powerBarFill.fillAmount = currentFill;
        powerBarFill.color = fillGradient.Evaluate(currentFill);
    }


    void StopFilling()
    {
        isFilling = false;

        float throwPower = currentFill; // 0 to 1
        // Debug.Log($"Throw Power: {throwPower}");
        
        StartFishingMinigame(throwPower);
    }

    // private void StartFishingMinigame(float throwPower)
    // {
    //     isMinigameActive = true;
    //     float throwX = Mathf.Lerp(minThrowX, maxThrowX, throwPower);
    //     ArtifactData artifactData = artifactOdds.GetArtifactData(throwPower);
    //     // Debug.Log($"Throw X: {throwX}");
    //     // Debug.Log($"Artifact Data: {artifactData.artifactType.artifactName}");
    //     DOVirtual.DelayedCall(0.5f, () =>
    //     {
    //         fishingManager.StartMiniGame(artifactData, throwX);
    //     });
    // }
    
    private void StartFishingMinigame(float throwPower)
    {
        isMinigameActive = true;
        
        // Activate and reset
        magnetTransform.gameObject.SetActive(true);
        magnetRb.linearVelocity = Vector2.zero;
        magnetRb.angularVelocity = 0;

        // Position the magnet at rope start
        magnetTransform.position = magnetStartPos;
        ropeLine.enabled = true;

        // Apply throw force
        Vector2 throwDirection = new Vector2(Mathf.Lerp(0, maxThrowX, throwPower), 1f).normalized;
        float forcePower = Mathf.Lerp(3f, 9f, throwPower); // tune as needed

        magnetRb.AddForce(throwDirection * forcePower, ForceMode2D.Impulse);

        // Optionally: start the minigame after a delay or on landing
        StartCoroutine(WaitForLanding(() =>
        {
            ArtifactData artifact = artifactOdds.GetArtifactData(throwPower);
            // fishingManager.StartMiniGame(artifact, magnetTransform.position.x);
        }));
    }

    IEnumerator WaitForLanding(System.Action onLanded)
    {
        yield return new WaitUntil(() => magnetRb.linearVelocity.magnitude < 0.1f);
        yield return new WaitForSeconds(0.3f); // small buffer
        onLanded?.Invoke();
    }
    
    [ContextMenu("Pull Magnet Back")]
    public void PullMagnetBack(System.Action onComplete = null)
    {
        // Disable physics so it doesn't fight the tween
        magnetRb.velocity = Vector2.zero;
        magnetRb.angularVelocity = 0f;
        magnetRb.isKinematic = true;

        magnetTransform
            .DOMove(magnetStartPos, 1.5f)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() =>
            {
                UpdateRope(); // Keep the rope updating manually
            })
            .OnComplete(() =>
            {
                magnetRb.isKinematic = false; // Ready for next throw
                ropeLine.enabled = false;
                magnetTransform.gameObject.SetActive(false); // Hide the magnet
                onComplete?.Invoke();
            });
    }
    
    private void UpdateRope()
    {
        if (!ropeLine.enabled || !magnetTransform.gameObject.activeSelf) return;

        Vector3 start = magnetStartPos;
        Vector3 end = magnetTransform.position;

        for (int i = 0; i < ropeSegmentCount; i++)
        {
            float t = i / (float)(ropeSegmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            float sag = Mathf.Sin(Mathf.PI * t) * ropeSlack;
            point.y -= sag;
            ropeLine.SetPosition(i, point);
        }
    }

    [ContextMenu("Reset")]
    public void PullBack()
    {
        PullMagnetBack(() =>
        {
            EnableThrowing(); // Ready for another throw
        });
    }
    
    public void EnableThrowing()
    {
        isMinigameActive = false;
    }
}