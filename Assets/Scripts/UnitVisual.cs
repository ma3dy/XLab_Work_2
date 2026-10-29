using System.Collections;
using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    private Renderer[] meshRenderers;
    private Color[] originalColors;
    private Coroutine currentEffectCoroutine;

    private WaitForSeconds hitDelay = new WaitForSeconds(0.2f);

    void Awake()
    {
        meshRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalColors[i] = meshRenderers[i].material.color;
        }
    }

    public void PlayHitEffect()
    {
        if (currentEffectCoroutine != null) StopCoroutine(currentEffectCoroutine);
        currentEffectCoroutine = StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        foreach (var renderer in meshRenderers)
        {
            if (renderer != null) renderer.material.color = new Color(0.3f, 0.3f, 0.3f);
        }

        yield return hitDelay;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] != null) meshRenderers[i].material.color = originalColors[i];
        }
    }

    public void PlayDeathEffect()
    {
        gameObject.SetActive(false);
    }
}