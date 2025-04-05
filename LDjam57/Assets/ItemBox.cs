using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float fallSpeed = 2f;

    public Transform topLimit;
    public Transform bottomLimit;

    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;

    private bool magnetOnMe = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = inactiveColor;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (magnetOnMe)
        {
            sr.color = activeColor;
            pos.y += floatSpeed * Time.deltaTime;
        }
        else
        {
            sr.color = inactiveColor;
            // pos.y -= fallSpeed * Time.deltaTime;
        }

        float minY = bottomLimit.position.y;
        float maxY = topLimit.position.y;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Magnet"))
        {
            magnetOnMe = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Magnet"))
        {
            magnetOnMe = false;
        }
    }
}