using System.Collections;
using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    private Renderer meshRenderer;
    private Color originalColor;
    private Coroutine currentEffectCoroutine;
    private Coroutine lungeCoroutine;
    private Vector3 originalPosition;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
        originalPosition = transform.localPosition;
    }

    public void PlayHitEffect()
    {
        if (currentEffectCoroutine != null) StopCoroutine(currentEffectCoroutine);
        currentEffectCoroutine = StartCoroutine(HitRoutine());
    }

    public void PlayAttackEffect(float directionX)
    {
        if (lungeCoroutine != null) StopCoroutine(lungeCoroutine);
        lungeCoroutine = StartCoroutine(LungeRoutine(directionX));
    }

    private IEnumerator LungeRoutine(float directionX)
    {
        Vector3 targetPos = originalPosition + new Vector3(directionX * 0.5f, 0, 0);
        float duration = 0.1f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    private IEnumerator HitRoutine()
    {
        if (meshRenderer != null) meshRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);

        float duration = 0.2f;
        float magnitude = 0.1f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPosition + new Vector3(0, yOffset, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        if (meshRenderer != null) meshRenderer.material.color = originalColor;
    }
}