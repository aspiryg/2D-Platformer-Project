using UnityEngine;
using System.Collections;
public class CameraShake : MonoBehaviour
{
    // Singleton instance
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;
    private bool isShaking = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Trigger camera shake
    public void Shake(float duration = 0.15f, float magnitude = 0.1f)
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }
    }

    // Coroutine for shaking effect
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        isShaking = true;
        originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Random offset
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original position
        transform.localPosition = originalPosition;
        isShaking = false;
    }
}
