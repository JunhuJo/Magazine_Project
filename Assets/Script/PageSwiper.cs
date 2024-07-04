using UnityEngine;
using UnityEngine.UI;


public class PageSwiper : MonoBehaviour
{
    public Sprite[] images; // �̹��� �迭
    public Image displayImage; // �̹����� ǥ���� UI Image

    private int currentImageIndex = 0;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float swipeThreshold = 50f; // �������� ���� �Ӱ谪

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    touchEndPos = touch.position;
                    HandleSwipe();
                    break;
            }
        }
    }

    private void HandleSwipe()
    {
        float swipeDistance = touchEndPos.x - touchStartPos.x;

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
            displayImage.sprite = images[currentImageIndex];
        }
    }

    private void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1)
        {
            currentImageIndex++;
            displayImage.sprite = images[currentImageIndex];
        }
    }
}
