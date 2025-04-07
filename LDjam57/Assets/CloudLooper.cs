using UnityEngine;

public class CloudLooper : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float spriteWidth = 20f; // world units width of your sprite

    private Transform[] clouds;

    private void Start()
    {
        // Get child cloud transforms
        clouds = new Transform[transform.childCount];
        for (int i = 0; i < clouds.Length; i++)
        {
            clouds[i] = transform.GetChild(i);
        }
    }

    private void Update()
    {
        foreach (Transform cloud in clouds)
        {
            // Move cloud left
            cloud.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        // Check if any cloud has moved off-screen left, then loop it to the right
        for (int i = 0; i < clouds.Length; i++)
        {
            Transform cloud = clouds[i];
            if (cloud.position.x <= -spriteWidth)
            {
                // Find the rightmost cloud
                float rightmostX = GetRightmostCloudX();
                // Move this cloud to the right of it
                cloud.position = new Vector3(rightmostX + spriteWidth, cloud.position.y, cloud.position.z);
            }
        }
    }

    private float GetRightmostCloudX()
    {
        float maxX = float.MinValue;
        foreach (Transform cloud in clouds)
        {
            if (cloud.position.x > maxX)
                maxX = cloud.position.x;
        }
        return maxX;
    }
}
