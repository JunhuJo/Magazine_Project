using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PageSwiper : MonoBehaviour
{
    public Sprite[] images; // �̹��� �迭
    public string[] descriptions; // �̹��� ���� �迭
    public Image displayImage; // ���� �̹����� ǥ���� UI Image
    public Text pageNumberText; // ������ ��ȣ�� ǥ���� �ؽ�Ʈ
    public Text descriptionText; // ������ ǥ���� �ؽ�Ʈ
    public Text endText; // ������ ���������� ǥ���� �ؽ�Ʈ

    public int currentImageIndex = 0;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private bool isDragging = false;
    private float swipeThreshold = 100f; // �������� ���� �Ӱ谪
    private float animationDuration = 0.15f; // �̹��� ��ȯ �ִϸ��̼� ���� �ð�
    private float descriptionAnimationDuration = 0.6f; // ���� �ؽ�Ʈ �ִϸ��̼� ���� �ð�
    private bool isTransitioning = false; // �̹��� ��ȯ ������ ����
    private bool isWaitingForSwipe = false; // ������ ���������� �������� ��� ����
    private bool swipeOccurred = false; // �������� �߻� ����
    private Coroutine descriptionCoroutine; // ���� �ؽ�Ʈ �ִϸ��̼� �ڷ�ƾ
    private Vector2 descriptionStartPos; // ���� �ؽ�Ʈ�� �ʱ� ��ġ

    // �������� ����� ������ �÷���
    public bool isSwipeEnabled = true;

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

        // pageNumberText�� descriptionText�� �������� ���� ��� �ڵ����� ����
        if (pageNumberText == null)
        {
            pageNumberText = GameObject.Find("PageNumberText").GetComponent<Text>();
        }

        if (descriptionText == null)
        {
            descriptionText = GameObject.Find("DescriptionText").GetComponent<Text>();
        }

        endText.gameObject.SetActive(false); // �ʱ⿡�� ����

        // �̹��� �迭�� ���̿� �°� descriptions �迭 �ʱ�ȭ
        if (descriptions == null || descriptions.Length != images.Length)
        {
            descriptions = new string[images.Length];
        }

        // ���� �ؽ�Ʈ�� �ʱ� ��ġ ����
        descriptionStartPos = descriptionText.GetComponent<RectTransform>().anchoredPosition;

        UpdatePageInfo(); // ������ ���� ������Ʈ

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
        rt.offsetMin = new Vector2(0, 200); // �Ʒ��� ���� 200�ȼ�
        rt.offsetMax = new Vector2(0, -200); // ���� ���� 200�ȼ�
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
        if (!isSwipeEnabled || isTransitioning) return; // ���������� ��Ȱ��ȭ�ǰų� �̹��� ��ȯ ���� ���� �Է��� ���� ����

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
                        endText.gameObject.SetActive(false); // EndText ����
                        descriptionText.gameObject.SetActive(true); // descriptionText ǥ��
                        UpdatePageInfo(); // ������ ���� ������Ʈ
                        StartCoroutine(FadeOutAndIn());
                    }
                    else if (swipeDistance > swipeThreshold) // ���������� ��������
                    {
                        // ���������� ���������Ͽ� ������ �������� �̵�
                        isWaitingForSwipe = false;
                        currentImageIndex = images.Length - 1;
                        endText.gameObject.SetActive(false); // EndText ����
                        descriptionText.gameObject.SetActive(true); // descriptionText ǥ��
                        UpdatePageInfo(); // ������ ���� ������Ʈ
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
            descriptionText.gameObject.SetActive(true); // descriptionText ǥ��
        }
        else
        {
            currentImageIndex = images.Length - 1; // ù �̹������� �������� �������� �� ������ �̹����� �̵�
        }
        UpdatePageInfo(); // ������ ���� ������Ʈ
        StartCoroutine(FadeOutAndIn());
    }

    private void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1 && swipeOccurred)
        {
            currentImageIndex++;
            endText.gameObject.SetActive(false); // ������ �̹����� �ƴ� ��� �ؽ�Ʈ ����
            descriptionText.gameObject.SetActive(true); // descriptionText ǥ��
        }
        else if (currentImageIndex == images.Length - 1)
        {
            StartCoroutine(ShowEndTextAndWaitForSwipe()); // �ؽ�Ʈ ǥ�� �� �������� ��� �ڷ�ƾ ����
            return;
        }
        UpdatePageInfo(); // ������ ���� ������Ʈ
        StartCoroutine(FadeOutAndIn());
    }

    private IEnumerator ShowEndTextAndWaitForSwipe()
    {
        descriptionText.gameObject.SetActive(false); // descriptionText ����
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

    private void UpdatePageInfo()
    {
        pageNumberText.text = "Page " + (currentImageIndex + 1) + " of " + images.Length;
        if (descriptions.Length > currentImageIndex)
        {
            if (descriptionCoroutine != null)
            {
                StopCoroutine(descriptionCoroutine);
            }

            // ���� �ؽ�Ʈ�� ��ġ�� �ʱ� ��ġ�� �缳��
            RectTransform rectTransform = descriptionText.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = descriptionStartPos;

            // ����Ƽ �����Ϳ��� �Էµ� ���ڿ����� \n�� ���� �ٹٲ� ���ڷ� ��ȯ
            string formattedDescription = descriptions[currentImageIndex].Replace("\\n", "\n");
            Debug.Log("Formatted Description: " + formattedDescription); // ����� ���

            descriptionText.text = formattedDescription;
            descriptionCoroutine = StartCoroutine(FadeInDescription(formattedDescription));
        }
        else
        {
            descriptionText.text = ""; // ������ ���� ��� �� �ؽ�Ʈ ����
        }
    }

    private IEnumerator FadeInDescription(string text)
    {
        descriptionText.text = text;
        RectTransform rectTransform = descriptionText.GetComponent<RectTransform>();
        Vector2 startPos = descriptionStartPos;
        Vector2 endPos = startPos + new Vector2(0, 20); // 20 �ȼ� ���� �̵�
        float elapsedTime = 0f;

        descriptionText.canvasRenderer.SetAlpha(0.0f); // �ʱ� ���İ��� 0���� ����
        rectTransform.anchoredPosition = startPos;

        while (elapsedTime < descriptionAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / descriptionAnimationDuration);
            float yPos = Mathf.Lerp(startPos.y, endPos.y, elapsedTime / descriptionAnimationDuration);
            rectTransform.anchoredPosition = new Vector2(startPos.x, yPos);
            descriptionText.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
    }
}
