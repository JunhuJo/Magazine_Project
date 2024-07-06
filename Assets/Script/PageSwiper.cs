using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PageSwiper : MonoBehaviour
{
    public Sprite[] images; // �̹��� �迭
    public Image displayImage; // ���� �̹����� ǥ���� UI Image
    public Image nextImage; // ���� �̹����� ǥ���� UI Image

    private int currentImageIndex = 0;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private bool isDragging = false;
    private float swipeThreshold = 100f; // �������� ���� �Ӱ谪
    private float animationDuration = 0.3f; // �ִϸ��̼� ���� �ð�

    void Start()
    {
        if (images.Length > 0)
        {
            displayImage.sprite = images[currentImageIndex];
            nextImage.sprite = images[currentImageIndex];
            nextImage.gameObject.SetActive(false); // �ʱ⿡�� ����
        }
    }

    void Update()
    {
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
                        float deltaX = touchCurrentPos.x - touchStartPos.x;
                        displayImage.rectTransform.anchoredPosition = new Vector2(deltaX, displayImage.rectTransform.anchoredPosition.y);
                        nextImage.rectTransform.anchoredPosition = new Vector2(deltaX + Mathf.Sign(deltaX) * displayImage.rectTransform.rect.width, nextImage.rectTransform.anchoredPosition.y);
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
        else
        {
            // �������� �Ÿ��� �Ӱ谪���� ������ ���� ��ġ�� �ǵ��ư���
            StartCoroutine(SmoothMove(displayImage.rectTransform.anchoredPosition.x, 0, nextImage.rectTransform.anchoredPosition.x, displayImage.rectTransform.rect.width));
        }
    }

    private void ShowPreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            nextImage.sprite = images[currentImageIndex];
            nextImage.gameObject.SetActive(true);
            StartCoroutine(SmoothTransition(true));
        }
        else
        {
            StartCoroutine(SmoothMove(displayImage.rectTransform.anchoredPosition.x, 0, nextImage.rectTransform.anchoredPosition.x, displayImage.rectTransform.rect.width));
        }
    }

    private void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1)
        {
            currentImageIndex++;
            nextImage.sprite = images[currentImageIndex];
            nextImage.gameObject.SetActive(true);
            StartCoroutine(SmoothTransition(false));
        }
        else
        {
            StartCoroutine(SmoothMove(displayImage.rectTransform.anchoredPosition.x, 0, nextImage.rectTransform.anchoredPosition.x, -displayImage.rectTransform.rect.width));
        }
    }

    private IEnumerator SmoothTransition(bool isRightSwipe)
    {
        float elapsedTime = 0f;
        float startX = displayImage.rectTransform.anchoredPosition.x;
        float endX = isRightSwipe ? displayImage.rectTransform.rect.width : -displayImage.rectTransform.rect.width;
        float nextStartX = nextImage.rectTransform.anchoredPosition.x;
        float nextEndX = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float newX = Mathf.Lerp(startX, endX, elapsedTime / animationDuration);
            float newNextX = Mathf.Lerp(nextStartX, nextEndX, elapsedTime / animationDuration);
            displayImage.rectTransform.anchoredPosition = new Vector2(newX, displayImage.rectTransform.anchoredPosition.y);
            nextImage.rectTransform.anchoredPosition = new Vector2(newNextX, nextImage.rectTransform.anchoredPosition.y);
            yield return null;
        }

        displayImage.sprite = nextImage.sprite;
        displayImage.rectTransform.anchoredPosition = new Vector2(0, displayImage.rectTransform.anchoredPosition.y);
        nextImage.rectTransform.anchoredPosition = new Vector2(isRightSwipe ? -displayImage.rectTransform.rect.width : displayImage.rectTransform.rect.width, nextImage.rectTransform.anchoredPosition.y);
        nextImage.gameObject.SetActive(false);
    }

    private IEnumerator SmoothMove(float startX, float endX, float nextStartX, float nextEndX)
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float newX = Mathf.Lerp(startX, endX, elapsedTime / animationDuration);
            float newNextX = Mathf.Lerp(nextStartX, nextEndX, elapsedTime / animationDuration);
            displayImage.rectTransform.anchoredPosition = new Vector2(newX, displayImage.rectTransform.anchoredPosition.y);
            nextImage.rectTransform.anchoredPosition = new Vector2(newNextX, nextImage.rectTransform.anchoredPosition.y);
            yield return null;
        }

        displayImage.rectTransform.anchoredPosition = new Vector2(endX, displayImage.rectTransform.anchoredPosition.y);
        nextImage.rectTransform.anchoredPosition = new Vector2(nextEndX, nextImage.rectTransform.anchoredPosition.y);
        nextImage.gameObject.SetActive(false);
    }
}
