using System.Collections;
using System.Collections.Generic;
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
}
