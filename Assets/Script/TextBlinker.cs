using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinker : MonoBehaviour
{
    public Text targetText;
    public float fadeDuration = 1f; // 페이드 인 아웃 효과 지속 시간
    public float stayDuration = 0.5f; // 텍스트가 완전히 보이는 상태로 유지되는 시간

    private Coroutine fadeCoroutine;

    void OnEnable()
    {
        // Enable 시 코루틴 시작
        StartFading();
    }

    void OnDisable()
    {
        // Disable 시 코루틴 정지
        StopFading();
    }

    public void StartFading()
    {
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeText());
        }
    }

    public void StopFading()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // 텍스트의 알파 값을 원래 상태로 복원
        if (targetText != null)
        {
            Color color = targetText.color;
            color.a = 1f;
            targetText.color = color;
        }
    }

    private IEnumerator FadeText()
    {
        while (true)
        {
            // 페이드 인
            yield return StartCoroutine(Fade(0f, 1f));
            // 완전히 보이는 상태로 유지
            yield return new WaitForSeconds(stayDuration);
            // 페이드 아웃
            yield return StartCoroutine(Fade(1f, 0f));
            // 완전히 사라진 상태로 유지
            yield return new WaitForSeconds(stayDuration);
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = targetText.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            targetText.color = color;
            yield return null;
        }
        color.a = endAlpha;
        targetText.color = color;
    }
}
