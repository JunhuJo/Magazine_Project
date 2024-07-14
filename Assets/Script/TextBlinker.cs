using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinker : MonoBehaviour
{
    public Text targetText;
    public float fadeDuration = 1f; // ���̵� �� �ƿ� ȿ�� ���� �ð�
    public float stayDuration = 0.5f; // �ؽ�Ʈ�� ������ ���̴� ���·� �����Ǵ� �ð�

    private Coroutine fadeCoroutine;

    void OnEnable()
    {
        // Enable �� �ڷ�ƾ ����
        StartFading();
    }

    void OnDisable()
    {
        // Disable �� �ڷ�ƾ ����
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

        // �ؽ�Ʈ�� ���� ���� ���� ���·� ����
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
            // ���̵� ��
            yield return StartCoroutine(Fade(0f, 1f));
            // ������ ���̴� ���·� ����
            yield return new WaitForSeconds(stayDuration);
            // ���̵� �ƿ�
            yield return StartCoroutine(Fade(1f, 0f));
            // ������ ����� ���·� ����
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
