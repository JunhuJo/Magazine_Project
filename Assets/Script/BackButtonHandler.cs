using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    void Update()
    {
        // �ȵ���̵忡���� �ڷΰ��� ��ư ó��
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // ���ø����̼� ����
                Application.Quit();
            }
        }
    }
}
