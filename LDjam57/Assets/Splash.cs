using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField] private AnimationClip splashAnimation; // Reference to the splash animation clip
    [SerializeField] private float destroyDelay = 2f; // Time in seconds before the splash is destroyed
    private void Start()
    {
        destroyDelay = splashAnimation.length; // Set the destroy delay to the length of the animation
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay); // Adjust the delay as needed
        Destroy(gameObject);
    }
}
