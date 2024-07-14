using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PageSwiper : MonoBehaviour
{
    public Sprite[] images; // 이미지 배열
    public string[] descriptions; // 이미지 설명 배열
    public Image displayImage; // 현재 이미지를 표시할 UI Image
    public Text pageNumberText; // 페이지 번호를 표시할 텍스트
    public Text descriptionText; // 설명을 표시할 텍스트
    public Text endText; // 마지막 페이지에서 표시할 텍스트

    public int currentImageIndex = 0;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private bool isDragging = false;
    private float swipeThreshold = 100f; // 스와이프 감지 임계값
    private float animationDuration = 0.15f; // 이미지 전환 애니메이션 지속 시간
    private float descriptionAnimationDuration = 0.6f; // 설명 텍스트 애니메이션 지속 시간
    private bool isTransitioning = false; // 이미지 전환 중인지 여부
    private bool isWaitingForSwipe = false; // 마지막 페이지에서 스와이프 대기 상태
    private bool swipeOccurred = false; // 스와이프 발생 여부
    private Coroutine descriptionCoroutine; // 설명 텍스트 애니메이션 코루틴
    private Vector2 descriptionStartPos; // 설명 텍스트의 초기 위치

    // 스와이프 기능을 제어할 플래그
    public bool isSwipeEnabled = true;

    private void Start()
    {
        // displayImage가 설정되지 않은 경우 자동으로 설정
        if (displayImage == null)
        {
            displayImage = GetComponent<Image>();
        }

        // endText가 설정되지 않은 경우 자동으로 설정
        if (endText == null)
        {
            endText = GameObject.Find("EndText").GetComponent<Text>();
        }

        // pageNumberText와 descriptionText가 설정되지 않은 경우 자동으로 설정
        if (pageNumberText == null)
        {
            pageNumberText = GameObject.Find("PageNumberText").GetComponent<Text>();
        }

        if (descriptionText == null)
        {
            descriptionText = GameObject.Find("DescriptionText").GetComponent<Text>();
        }

        endText.gameObject.SetActive(false); // 초기에는 숨김

        // 이미지 배열의 길이에 맞게 descriptions 배열 초기화
        if (descriptions == null || descriptions.Length != images.Length)
        {
            descriptions = new string[images.Length];
        }

        // 설명 텍스트의 초기 위치 저장
        descriptionStartPos = descriptionText.GetComponent<RectTransform>().anchoredPosition;

        UpdatePageInfo(); // 페이지 정보 업데이트

        // 이미지 크기를 화면 크기에 맞게 조정
        AdjustImageSize();
    }

    public void StartFadeIn()
    {
        if (images.Length > 0)
        {
            displayImage.sprite = images[currentImageIndex];
            displayImage.canvasRenderer.SetAlpha(0.0f);
            AdjustImageSize(); // 새 이미지에 맞게 조정
            StartCoroutine(FadeInInitialImage());
        }
    }

    private void AdjustImageSize()
    {
        RectTransform rt = displayImage.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, 200); // 아래쪽 공백 200픽셀
        rt.offsetMax = new Vector2(0, -200); // 위쪽 공백 200픽셀
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
        if (!isSwipeEnabled || isTransitioning) return; // 스와이프가 비활성화되거나 이미지 전환 중일 때는 입력을 받지 않음

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

                    if (swipeDistance < -swipeThreshold) // 왼쪽으로 스와이프
                    {
                        // 왼쪽으로 스와이프하여 첫 번째 화면으로 이동
                        isWaitingForSwipe = false;
                        currentImageIndex = 0;
                        endText.gameObject.SetActive(false); // EndText 숨김
                        descriptionText.gameObject.SetActive(true); // descriptionText 표시
                        UpdatePageInfo(); // 페이지 정보 업데이트
                        StartCoroutine(FadeOutAndIn());
                    }
                    else if (swipeDistance > swipeThreshold) // 오른쪽으로 스와이프
                    {
                        // 오른쪽으로 스와이프하여 마지막 페이지로 이동
                        isWaitingForSwipe = false;
                        currentImageIndex = images.Length - 1;
                        endText.gameObject.SetActive(false); // EndText 숨김
                        descriptionText.gameObject.SetActive(true); // descriptionText 표시
                        UpdatePageInfo(); // 페이지 정보 업데이트
                        StartCoroutine(FadeOutAndIn());
                    }
                }
            }
            return; // 스와이프 대기 중일 때는 나머지 입력을 처리하지 않음
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
                // 오른쪽으로 스와이프
                ShowPreviousImage();
            }
            else
            {
                // 왼쪽으로 스와이프
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
            endText.gameObject.SetActive(false); // 첫 이미지에서 마지막 페이지 텍스트 숨김
            descriptionText.gameObject.SetActive(true); // descriptionText 표시
        }
        else
        {
            currentImageIndex = images.Length - 1; // 첫 이미지에서 왼쪽으로 스와이프 시 마지막 이미지로 이동
        }
        UpdatePageInfo(); // 페이지 정보 업데이트
        StartCoroutine(FadeOutAndIn());
    }

    private void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1 && swipeOccurred)
        {
            currentImageIndex++;
            endText.gameObject.SetActive(false); // 마지막 이미지가 아닌 경우 텍스트 숨김
            descriptionText.gameObject.SetActive(true); // descriptionText 표시
        }
        else if (currentImageIndex == images.Length - 1)
        {
            StartCoroutine(ShowEndTextAndWaitForSwipe()); // 텍스트 표시 후 스와이프 대기 코루틴 시작
            return;
        }
        UpdatePageInfo(); // 페이지 정보 업데이트
        StartCoroutine(FadeOutAndIn());
    }

    private IEnumerator ShowEndTextAndWaitForSwipe()
    {
        descriptionText.gameObject.SetActive(false); // descriptionText 숨김
        endText.gameObject.SetActive(true); // 텍스트 활성화
        endText.canvasRenderer.SetAlpha(0.0f); // 텍스트의 초기 알파값을 0으로 설정

        float elapsedTime = 0f;

        // 텍스트 페이드인
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / animationDuration);
            endText.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        isWaitingForSwipe = true; // 스와이프 대기 상태로 설정
    }

    private IEnumerator FadeOutAndIn()
    {
        isTransitioning = true; // 이미지 전환 시작

        float elapsedTime = 0f;

        // 현재 이미지를 페이드 아웃
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / animationDuration);
            displayImage.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        // 이미지를 교체
        displayImage.sprite = images[currentImageIndex];
        AdjustImageSize(); // 새로운 이미지 크기를 화면 크기에 맞게 조정
        displayImage.canvasRenderer.SetAlpha(0.0f);

        // 새로운 이미지를 페이드 인
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / animationDuration);
            displayImage.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        isTransitioning = false; // 이미지 전환 완료
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

            // 설명 텍스트의 위치를 초기 위치로 재설정
            RectTransform rectTransform = descriptionText.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = descriptionStartPos;

            // 유니티 에디터에서 입력된 문자열에서 \n을 실제 줄바꿈 문자로 변환
            string formattedDescription = descriptions[currentImageIndex].Replace("\\n", "\n");
            Debug.Log("Formatted Description: " + formattedDescription); // 디버깅 출력

            descriptionText.text = formattedDescription;
            descriptionCoroutine = StartCoroutine(FadeInDescription(formattedDescription));
        }
        else
        {
            descriptionText.text = ""; // 설명이 없는 경우 빈 텍스트 설정
        }
    }

    private IEnumerator FadeInDescription(string text)
    {
        descriptionText.text = text;
        RectTransform rectTransform = descriptionText.GetComponent<RectTransform>();
        Vector2 startPos = descriptionStartPos;
        Vector2 endPos = startPos + new Vector2(0, 20); // 20 픽셀 위로 이동
        float elapsedTime = 0f;

        descriptionText.canvasRenderer.SetAlpha(0.0f); // 초기 알파값을 0으로 설정
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
