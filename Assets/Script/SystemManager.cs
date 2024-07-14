using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


public class SystemManager : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private GameObject start_Image;
    [SerializeField] private GameObject loby_Image;
    [SerializeField] private GameObject rolling_Image;

    [Header("Func")]
    [SerializeField] private Image backGround_Screen;
    [SerializeField] private PageSwiper passion_Open_Image;
    public bool next_page = false;

    private bool SetPassion = false;
    private bool lobyGO = false;
    private float fadeSpeed = 1f;
    private float maxTransparency = 1.0f;
    private float maxsetpassion = 1.0f;
    private float currentTransparency = 0f;
    private float setpassion = 0f;

    private void Update()
    {
        On_Fade_BackGround();
        Passion_Open();
    }

    public void OnClick_Passion()
    {
        Debug.Log($"Passion 눌림 : >>>{currentTransparency}");
        SetPassion = true;
    }

    private void Passion_Open()
    {
        if (SetPassion)
        {
            rolling_Image.gameObject.SetActive(true);
            passion_Open_Image.gameObject.SetActive(true); // PageSwiper 스크립트가 붙은 오브젝트 활성화
            StartCoroutine(StartFadeInCoroutine()); // PageSwiper의 초기 페이드 인 효과 호출
            loby_Image.gameObject.SetActive(false);
            next_page = false; // Loby_Set_Loby_Page가 호출되지 않도록 설정
            SetPassion = false; // 한번만 실행되도록 설정
        }
    }

    private IEnumerator StartFadeInCoroutine()
    {
        yield return new WaitForEndOfFrame(); // 한 프레임 대기
        passion_Open_Image.StartFadeIn();
    }

    private void On_Fade_BackGround()
    {
        if (next_page)
        {
            backGround_Screen.gameObject.SetActive(true);
            currentTransparency += fadeSpeed * Time.deltaTime;
            currentTransparency = Mathf.Clamp(currentTransparency, 0f, maxTransparency);
            SetTransparency(currentTransparency);
        }
    }

    void SetTransparency(float alpha)
    {
        if (backGround_Screen != null)
        {
            Color color = backGround_Screen.color;
            color.a = alpha;
            backGround_Screen.color = color;

            if (color.a == 1)
            {
                First_Page_Start();
            }
        }
    }

    void SetPassion_Open(float alpha)
    {
        if (backGround_Screen != null)
        {
            Color color = passion_Open_Image.displayImage.color;
            color.a = alpha;
            passion_Open_Image.displayImage.color = color;
        }
    }

    private void First_Page_Start()
    {
        start_Image.SetActive(false);
        backGround_Screen.gameObject.SetActive(false);
        SetPassion = true;
    }
}
