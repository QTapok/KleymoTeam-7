using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float fadeDuration = 0.25f;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SetAlpha(0f);
    }

    public void PlayTransition(System.Action onMidpoint)
    {
        if (isTransitioning) return;
        StartCoroutine(Routine(onMidpoint));
    }

    private IEnumerator Routine(System.Action onMidpoint)
    {
        isTransitioning = true;
        yield return Fade(0f, 1f);   // затемнение
        onMidpoint?.Invoke();         // смена локации
        yield return new WaitForSeconds(0.05f);
        yield return Fade(1f, 0f);   // проявление
        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / fadeDuration));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float a)
    {
        var c = fadeOverlay.color;
        c.a = a;
        fadeOverlay.color = c;
        fadeOverlay.raycastTarget = a > 0.01f; // блокируем клики во время перехода
    }
}