using UnityEngine;

public class ScrollControlledHook : MonoBehaviour
{
    public float scrollSpeed = 5f;         // Speed of movement
    public float minY = -4f;               // Minimum Y position
    public float maxY = 4f;                // Maximum Y position

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput > 0f)
        {
            Vector3 position = transform.position;
            position.y += scrollInput * scrollSpeed;
            position.y = Mathf.Clamp(position.y, minY, maxY); // Keep within bounds
            transform.position = position;
        }
    }
}
