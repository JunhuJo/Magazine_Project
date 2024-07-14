using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject exitConfirmationUI;
    [SerializeField] private PageSwiper pageSwiper; // PageSwiper ��ũ��Ʈ�� �����ϱ� ���� ����
    private bool isExitConfirmationActive = false;

    void Update()
    {
        // �ȵ���̵忡���� �ڷΰ��� ��ư ó��
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
            // ���� Ȯ�� UI�� Ȱ��ȭ�� ���¿����� �߰����� �Է��� ����
            return;
        }

        ShowExitConfirmationUI();
    }

    private void ShowExitConfirmationUI()
    {
        exitConfirmationUI.SetActive(true);
        isExitConfirmationActive = true;
        Time.timeScale = 0; // ���� �Ͻ� ����
        if (pageSwiper != null)
        {
            pageSwiper.isSwipeEnabled = false; // �������� ��Ȱ��ȭ
        }
    }

    private void HideExitConfirmationUI()
    {
        exitConfirmationUI.SetActive(false);
        isExitConfirmationActive = false;
        Time.timeScale = 1; // ���� �簳
        if (pageSwiper != null)
        {
            pageSwiper.isSwipeEnabled = true; // �������� Ȱ��ȭ
        }
    }

    // ���� Ȯ�� UI���� "��" ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnClick_ExitYes()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    // ���� Ȯ�� UI���� "�ƴϿ�" ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnClick_ExitNo()
    {
        HideExitConfirmationUI();
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
