using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    void Update()
    {
        // 안드로이드에서만 뒤로가기 버튼 처리
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 애플리케이션 종료
                Application.Quit();
            }
        }
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
