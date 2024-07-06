using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PageSwiper : MonoBehaviour
{
    public Sprite[] images; // �̹��� �迭
    public Image displayImage; // ���� �̹����� ǥ���� UI Image
    public Text endText; // ������ ���������� ǥ���� �ؽ�Ʈ

    private int currentImageIndex = 0;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private bool isDragging = false;
    private float swipeThreshold = 100f; // �������� ���� �Ӱ谪
    private float animationDuration = 0.3f; // �ִϸ��̼� ���� �ð�
    private bool isTransitioning = false; // �̹��� ��ȯ ������ ����

    private void Start()
    {
        // displayImage�� �������� ���� ��� �ڵ����� ����
        if (displayImage == null)
        {
            displayImage = GetComponent<Image>();
        }

        // endText�� �������� ���� ��� �ڵ����� ����
        if (endText == null)
        {
            endText = GameObject.Find("EndText").GetComponent<Text>();
        }

        endText.gameObject.SetActive(false); // �ʱ⿡�� ����
    }

    public void StartFadeIn()
    {
        if (images.Length > 0)
        {
            displayImage.sprite = images[currentImageIndex];
            displayImage.canvasRenderer.SetAlpha(0.0f);
            StartCoroutine(FadeInInitialImage());
        }
    }

    private IEnumerator FadeInInitialImage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / animationDuration);
            displayImage.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }
    }

    void Update()
    {
        if (isTransitioning) return; // �̹��� ��ȯ ���� ���� �Է��� ���� ����

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        touchCurrentPos = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    HandleSwipe();
                    break;
            }
        }
    }

    private void HandleSwipe()
    {
        float swipeDistance = touchCurrentPos.x - touchStartPos.x;

        if (Mathf.Abs(swipeDistance) > swipeThreshold)
        {
            if (swipeDistance > 0)
            {
                // ���������� ��������
                ShowPreviousImage();
            }
            else
            {
                // �������� ��������
                ShowNextImage();
            }
        }
    }

    private void ShowPreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            endText.gameObject.SetActive(false); // ù �̹������� ������ ������ �ؽ�Ʈ ����
            StartCoroutine(FadeOutAndIn());
        }
        else
        {
            currentImageIndex = images.Length - 1; // ù �̹������� �������� �������� �� ������ �̹����� �̵�
            StartCoroutine(FadeOutAndIn());
        }
    }

    private void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1)
        {
            currentImageIndex++;
            endText.gameObject.SetActive(false); // ������ �̹����� �ƴ� ��� �ؽ�Ʈ ����
        }
        else
        {
            endText.gameObject.SetActive(true); // ������ �̹����� ��� �ؽ�Ʈ ǥ��
        }
        StartCoroutine(FadeOutAndIn());
    }

    private IEnumerator FadeOutAndIn()
    {
        isTransitioning = true; // �̹��� ��ȯ ����

        float elapsedTime = 0f;

        // ���� �̹����� ���̵� �ƿ�
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / animationDuration);
            displayImage.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        // �̹����� ��ü
        displayImage.sprite = images[currentImageIndex];
        displayImage.canvasRenderer.SetAlpha(0.0f);

        // ���ο� �̹����� ���̵� ��
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / animationDuration);
            displayImage.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        isTransitioning = false; // �̹��� ��ȯ �Ϸ�
    }
}
