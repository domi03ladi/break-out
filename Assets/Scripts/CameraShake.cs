using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float shakeDuration = 0.4f;

    [SerializeField]
    private float shakeMagnitude = 0.1f;

    private Vector3 originalPosition;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void TriggerShake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(shakeDuration, shakeMagnitude));
    }
    public void TriggerShake(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        float randomStart = Random.Range(-1000f, 1000f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;

            float noiseX = Mathf.PerlinNoise(percentComplete * 4f + randomStart, 0f) * 2f - 1f;

            float noiseY = 0f;

            float currentMagnitude = Mathf.Lerp(magnitude, 0f, percentComplete);

            Vector3 offset = new Vector3(noiseX, noiseY, 0) * currentMagnitude;

            transform.localPosition = originalPosition + offset;

            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
