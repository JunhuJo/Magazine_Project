using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PageSwiper : MonoBehaviour
{
    public Sprite[] images; // �̹��� �迭
    public Image displayImage; // ���� �̹����� ǥ���� UI Image
    public Text endText; // ������ ���������� ǥ���� �ؽ�Ʈ

    public int currentImageIndex = 0;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private bool isDragging = false;
    private float swipeThreshold = 100f; // �������� ���� �Ӱ谪
    private float animationDuration = 0.3f; // �ִϸ��̼� ���� �ð�
    private bool isTransitioning = false; // �̹��� ��ȯ ������ ����
    private bool isWaitingForSwipe = false; // ������ ���������� �������� ��� ����
    private bool swipeOccurred = false; // �������� �߻� ����

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

        // �̹��� ũ�⸦ ȭ�� ũ�⿡ �°� ����
        AdjustImageSize();
    }

    public void StartFadeIn()
    {
        if (images.Length > 0)
        {
            displayImage.sprite = images[currentImageIndex];
            displayImage.canvasRenderer.SetAlpha(0.0f);
            AdjustImageSize(); // �� �̹����� �°� ����
            StartCoroutine(FadeInInitialImage());
        }
    }

    private void AdjustImageSize()
    {
        RectTransform rt = displayImage.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, 200); // �Ʒ��� ���� 50�ȼ�
        rt.offsetMax = new Vector2(0, -200); // ���� ���� 50�ȼ�
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

        if (isWaitingForSwipe)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                    isDragging = true;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    touchCurrentPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isDragging = false;
                    float swipeDistance = touchCurrentPos.x - touchStartPos.x;

                    if (swipeDistance < -swipeThreshold) // �������� ��������
                    {
                        // �������� ���������Ͽ� ù ��° ȭ������ �̵�
                        isWaitingForSwipe = false;
                        currentImageIndex = 0;
                        StartCoroutine(FadeOutAndIn());
                    }
                }
            }
            return; // �������� ��� ���� ���� ������ �Է��� ó������ ����
        }

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
            swipeOccurred = true;
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
        else
        {
            swipeOccurred = false;
        }
    }

    private void ShowPreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            endText.gameObject.SetActive(false); // ù �̹������� ������ ������ �ؽ�Ʈ ����
        }
        else
        {
            currentImageIndex = images.Length - 1; // ù �̹������� �������� �������� �� ������ �̹����� �̵�
        }
        StartCoroutine(FadeOutAndIn());
    }

    private void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1 && swipeOccurred)
        {
            currentImageIndex++;
            endText.gameObject.SetActive(false); // ������ �̹����� �ƴ� ��� �ؽ�Ʈ ����
        }
        else if(currentImageIndex == images.Length - 1)
        {
            StartCoroutine(ShowEndTextAndWaitForSwipe()); // �ؽ�Ʈ ǥ�� �� �������� ��� �ڷ�ƾ ����
            return;
        }
        StartCoroutine(FadeOutAndIn());
    }

    private IEnumerator ShowEndTextAndWaitForSwipe()
    {
        endText.gameObject.SetActive(true); // �ؽ�Ʈ Ȱ��ȭ
        endText.canvasRenderer.SetAlpha(0.0f); // �ؽ�Ʈ�� �ʱ� ���İ��� 0���� ����

        float elapsedTime = 0f;

        // �ؽ�Ʈ ���̵���
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / animationDuration);
            endText.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        isWaitingForSwipe = true; // �������� ��� ���·� ����
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
        AdjustImageSize(); // ���ο� �̹��� ũ�⸦ ȭ�� ũ�⿡ �°� ����
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
