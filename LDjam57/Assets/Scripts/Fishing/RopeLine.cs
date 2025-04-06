using System.Collections;
using UnityEngine;

public class RopeLine : MonoBehaviour
{
    [SerializeField] private Rope2DGenerator rope2DGenerator;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float throwSpeed = 0.05f; // Speed of throw

    private int currentVisibleSegments = 0;
    private bool isActivated = false;

    private void Awake()
    {
        rope2DGenerator = GetComponent<Rope2DGenerator>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 0;
    }
    
    private IEnumerator ThrowRope()
    {
        currentVisibleSegments = 0;
        while (currentVisibleSegments < rope2DGenerator.segments.Length)
        {
            currentVisibleSegments++;
            lineRenderer.positionCount = currentVisibleSegments;

            for (int i = 0; i < currentVisibleSegments; i++)
            {
                lineRenderer.SetPosition(i, rope2DGenerator.segments[i].position);
            }

            yield return new WaitForSeconds(throwSpeed);
        }
    }

    private void Update()
    {
        // Keep updating positions in case rope is moving dynamically
        for (int i = 0; i < currentVisibleSegments; i++)
        {
            lineRenderer.SetPosition(i, rope2DGenerator.segments[i].position);
        }
    }
    
    public void SetRopeActive(bool isActive)
    {
        isActivated = isActive;
        if (isActive)
        {
            lineRenderer.enabled = true;
            StartCoroutine(ThrowRope());
        }
        else
        {
            lineRenderer.enabled = false;
            StopAllCoroutines();
            currentVisibleSegments = 0;
        }
    }
}