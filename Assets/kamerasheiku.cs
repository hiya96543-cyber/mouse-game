using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CameraFollow cameraFollow;

    private Vector3 shakeOffset;

    private void Awake()
    {
        cameraFollow = GetComponent<CameraFollow>();
    }

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0);

            transform.position += shakeOffset;

            elapsed += Time.deltaTime;

            yield return null;

            transform.position -= shakeOffset;
        }

        shakeOffset = Vector3.zero;
    }
}