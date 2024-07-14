using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject exitConfirmationUI;
    [SerializeField] private PageSwiper pageSwiper; // PageSwiper 스크립트를 참조하기 위한 변수
    private bool isExitConfirmationActive = false;

    void Update()
    {
        // 안드로이드에서만 뒤로가기 버튼 처리
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackButton();
            }
        }
    }

    private void HandleBackButton()
    {
        if (isExitConfirmationActive)
        {
            // 종료 확인 UI가 활성화된 상태에서는 추가적인 입력을 무시
            return;
        }

        ShowExitConfirmationUI();
    }

    private void ShowExitConfirmationUI()
    {
        exitConfirmationUI.SetActive(true);
        isExitConfirmationActive = true;
        Time.timeScale = 0; // 게임 일시 정지
        if (pageSwiper != null)
        {
            pageSwiper.isSwipeEnabled = false; // 스와이프 비활성화
        }
    }

    private void HideExitConfirmationUI()
    {
        exitConfirmationUI.SetActive(false);
        isExitConfirmationActive = false;
        Time.timeScale = 1; // 게임 재개
        if (pageSwiper != null)
        {
            pageSwiper.isSwipeEnabled = true; // 스와이프 활성화
        }
    }

    // 종료 확인 UI에서 "예" 버튼을 눌렀을 때 호출되는 메서드
    public void OnClick_ExitYes()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    // 종료 확인 UI에서 "아니오" 버튼을 눌렀을 때 호출되는 메서드
    public void OnClick_ExitNo()
    {
        HideExitConfirmationUI();
    }

    public void OnExitButtonPressed()
    {
#if UNITY_EDITOR
        // Unity Editor에서 실행 중일 때는 종료 대신 에디터 정지
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드 환경에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
}
