using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ThrowingManager : MonoBehaviour
{
    // ————————————————————————
    // Inspector References
    // ————————————————————————

    [Header("Dependencies")]
    [SerializeField] private ArtifactOdds artifactOdds;
    [SerializeField] private FishingManager fishingManager;

    [Header("Power Bar")]
    [SerializeField] private GameObject powerBar;
    [SerializeField] private Image powerBarFill;
    [SerializeField] private float fillSpeed = 1.5f;
    [SerializeField] private float resetTime = 0.5f;
    [SerializeField] private Gradient fillGradient;
    [SerializeField] private AnimationCurve easeInCurve;

    [Header("Throw Settings")]
    [SerializeField] private float minThrowX = 4f;
    [SerializeField] private float maxThrowX = -7.5f;
    [SerializeField] private float minThrowPower = 2f;
    [SerializeField] private float maxThrowPower = 6f;

    [Header("Rope")]
    [SerializeField] private LineRenderer ropeLine;
    [SerializeField] private int ropeSegmentCount = 20;
    [SerializeField] private float ropeSlack = 1.5f;

    [Header("Magnet")]
    [SerializeField] private Transform magnetTransform;
    [SerializeField] private Rigidbody2D magnetRb;

    [Header("Artifact Display")]
    [SerializeField] private ArtifactData currentArtifact;
    [SerializeField] private SpriteRenderer artifactSpriteRenderer;

    // ————————————————————————
    // Private State
    // ————————————————————————

    private Vector3 magnetStartPos;
    private float currentFill = 0f;
    private bool isFilling = false;
    private bool goingUp = true;
    private bool canReceiveInput = false;
    private bool isMinigameActive = false;


    // ————————————————————————
    // Unity Events
    // ————————————————————————

    private void Awake()
    {
        powerBar.SetActive(false);
        powerBarFill.fillAmount = 0f;
        magnetStartPos = magnetTransform.position;
        ropeLine.positionCount = ropeSegmentCount;
    }

    private void Start()
    {
        // StartCoroutine(EnableInputAfterDelay(0.1f));
    }

    private void Update()
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
            StartCoroutine(WaitForPowerBarToHide());
        }
    }

    private void LateUpdate()
    {
        UpdateRope();
    }


    // ————————————————————————
    // Power Bar Logic
    // ————————————————————————

    private IEnumerator EnableInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canReceiveInput = true;
    }

    private void StartFilling()
    {
        isFilling = true;
        goingUp = true;
        currentFill = 0f;
    }

    private void UpdateFill()
    {
        float delta = fillSpeed * Time.deltaTime * (goingUp ? 1 : -1);
        currentFill += delta * easeInCurve.Evaluate(currentFill);

        if (currentFill >= 1f) { currentFill = 1f; goingUp = false; }
        else if (currentFill <= 0f) { currentFill = 0f; goingUp = true; }

        powerBarFill.fillAmount = currentFill;
        powerBarFill.color = fillGradient.Evaluate(currentFill);
    }

    private void StopFilling()
    {
        isFilling = false;
        float throwPower = currentFill;
        Debug.Log($"Throw Power: {throwPower}");
        StartFishingMinigame(throwPower);
    }

    private IEnumerator WaitForPowerBarToHide()
    {
        yield return new WaitForSeconds(resetTime);
        powerBar.SetActive(false);
    }


    // ————————————————————————
    // Throw & Minigame
    // ————————————————————————

    private void StartFishingMinigame(float throwPower)
    {
        isMinigameActive = true;

        artifactSpriteRenderer.sprite = null;
        magnetTransform.gameObject.SetActive(true);

        magnetRb.linearVelocity = Vector2.zero;
        magnetRb.angularVelocity = 0;
        magnetTransform.position = magnetStartPos;

        ropeLine.enabled = true;

        Vector2 throwDirection = new Vector2(Mathf.Lerp(minThrowX, maxThrowX, throwPower), 1f).normalized;
        float forcePower = Mathf.Lerp(minThrowPower, maxThrowPower, throwPower);

        magnetRb.AddForce(throwDirection * forcePower, ForceMode2D.Impulse);

        StartCoroutine(WaitForLanding(() =>
        {
            ArtifactData artifact = artifactOdds.GetArtifactData(throwPower);
            currentArtifact = artifact;
            fishingManager.StartMiniGame(artifact, magnetTransform.position.x);
        }));
    }

    private IEnumerator WaitForLanding(System.Action onLanded)
    {
        yield return new WaitUntil(() => magnetRb.linearVelocity.magnitude < 0.1f);
        yield return new WaitForSeconds(0.3f);
        onLanded?.Invoke();
    }

    public void MinigameOver(bool withArtifact = false)
    {
        artifactSpriteRenderer.sprite = withArtifact ? currentArtifact.artifactSprite : null;
        if(!withArtifact) currentArtifact = null; 
        
        PullMagnetBack(() =>
        {
            GameManager.Instance.EndThrow(currentArtifact);
        });
    }

    public void EnableThrowing()
    {
        isMinigameActive = false;
        StartCoroutine(EnableInputAfterDelay(0.1f));
    }


    // ————————————————————————
    // Rope & Pullback
    // ————————————————————————

    public void PullMagnetBack(System.Action onComplete = null)
    {
        magnetRb.linearVelocity = Vector2.zero;
        magnetRb.angularVelocity = 0f;
        magnetRb.bodyType = RigidbodyType2D.Kinematic;

        magnetTransform
            .DOMove(magnetStartPos, 1.5f)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(UpdateRope)
            .OnComplete(() =>
            {
                magnetRb.bodyType = RigidbodyType2D.Dynamic;
                ropeLine.enabled = false;
                magnetTransform.gameObject.SetActive(false);
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
}
