using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Rope2DGenerator : MonoBehaviour
{
    [SerializeField, Range(2, 50)] int segmentCount = 10;
    public Transform startPoint;
    public Transform endPoint;
    public Rigidbody2D segmentPrefab;
    public float spawnDelay = 0.05f; // time between segment spawns
    public float segmentMadePercentage;
    
    [HideInInspector] public Transform[] segments;
    
    
    [Header("Magnet Settings")]
    [SerializeField] private GameObject magnetPrefab;
    private GameObject magnetInstance;
    
    [Header("Line")]
    [SerializeField] RopeLine ropeLine;

    public void ThrowRope()
    {
        StartCoroutine(GenerateRope());
    }

    IEnumerator GenerateRope()
    {
        segments = new Transform[segmentCount];
        Vector2 previousPosition = startPoint.position;
        Rigidbody2D prevRb = null;

        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 1 && magnetPrefab != null)
            {
                magnetInstance = Instantiate(magnetPrefab, startPoint.position, Quaternion.identity);
                ropeLine.SetRopeActive(true);
            }
            
            Vector2 pos = Vector2.Lerp(startPoint.position, endPoint.position, (float)i / (segmentCount - 1));
            Rigidbody2D newSegment = Instantiate(segmentPrefab, pos, Quaternion.identity, transform);
            // newSegment.gravityScale += 1+ (i/segmentCount); // Increase gravity scale for each segment
            segments[i] = newSegment.transform;

            if (prevRb != null)
            {
                HingeJoint2D joint = newSegment.gameObject.GetComponent<HingeJoint2D>();
                joint.connectedBody = prevRb;
            }
            else
            {
                // First segment connects to startPoint
                HingeJoint2D joint = newSegment.gameObject.GetComponent<HingeJoint2D>();
                joint.connectedBody = startPoint.GetComponent<Rigidbody2D>();
            }

            prevRb = newSegment;
            segmentMadePercentage = (float)(i + 1) / segmentCount;
            yield return new WaitForSeconds(spawnDelay);
        }
        //
        // // Optionally connect last segment to endPoint
        // HingeJoint2D endJoint = segments[segmentCount - 1].gameObject.AddComponent<HingeJoint2D>();
        // endJoint.connectedBody = endPoint.GetComponent<Rigidbody2D>();
        //
        // HingeJoint2D lastJoint = endPoint.gameObject.GetComponent<HingeJoint2D>();
        // lastJoint.connectedBody = segments[segmentCount - 1].GetComponent<Rigidbody2D>();
    }
    
    Vector2 GetSegmentPosition(int index)
    {
        float t = (float)index / (segmentCount - 1);
        return Vector2.Lerp(startPoint.position, endPoint.position, t);
    }
    
    private void Update()
    {
        // While rope is building, update magnet's position
        if (magnetInstance != null && segments != null)
        {
            for (int i = segmentCount - 1; i >= 0; i--)
            {
                if (segments[i] != null)
                {
                    magnetInstance.transform.position = segments[i].position;
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(startPoint == null || endPoint == null)
            return;
        Gizmos.color = Color.green;
        for(int i = 0; i < segmentCount; i++)
        {
            Vector2 pos = GetSegmentPosition(i);
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }
}

