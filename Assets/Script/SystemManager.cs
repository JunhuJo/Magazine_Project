
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private GameObject start_Image;
    [SerializeField] private GameObject loby_Iamage;
    [SerializeField] private GameObject rolling_Image;


    [Header("Func")]
    [SerializeField] private Image backGround_Screen;
    [SerializeField] private PageSwiper passion_Open_Image;
    [SerializeField] private Button passion_Btn;
    [SerializeField] private Button photo_Btn;
    [SerializeField] private Button email_Btn;
    public bool next_page = false;


    private bool SetPassion = false;
    
    private float fadeSpeed = 1f;
    private float maxTransparency = 1.0f;
    private float maxsetpassion = 1.0f;
    private float currentTransparency = 0f;
    private float setpassion = 0f;

    private void Start()
    {
        
        passion_Btn.onClick.AddListener(OnClick_Passion);
        photo_Btn.onClick.AddListener(OnClick_Photo);
        email_Btn.onClick.AddListener(OnClick_Email);
    }

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

    public void OnClick_Photo()
    {
        Debug.Log("Photo 눌림");
    }

    public void OnClick_Email()
    {
        Debug.Log("Email 눌림");
    }


    private void Passion_Open()
    {
        if(SetPassion)
        {
            rolling_Image.gameObject.SetActive(true);
            setpassion += fadeSpeed * Time.deltaTime;
            setpassion = Mathf.Clamp(setpassion, 0f, maxsetpassion);
            SetPassion_Open(setpassion);
        }
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
                Loby_Set_Loby_Page();
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

    private void Loby_Set_Loby_Page()
    {
        start_Image.SetActive(false);
        backGround_Screen.gameObject.SetActive(false);
        loby_Iamage.gameObject.SetActive(true);
    }
}
