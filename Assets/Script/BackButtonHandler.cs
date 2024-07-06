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

    public void OnExitButtonPressed()
    {
#if UNITY_EDITOR
        // Unity Editor���� ���� ���� ���� ���� ��� ������ ����
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ���� ���� ȯ�濡���� ���ø����̼� ����
            Application.Quit();
#endif
    }
}
