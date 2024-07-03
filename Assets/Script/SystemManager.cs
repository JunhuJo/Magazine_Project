
using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private GameObject start_Image;
    [SerializeField] private GameObject loby_Iamage;


    [Header("Func")]
    [SerializeField] private Image backGround_Screen;
    [SerializeField] private Button next_Btn;
    [SerializeField] private Button passion_Btn;
    [SerializeField] private Button photo_Btn;
    [SerializeField] private Button email_Btn;


    private bool next_page = false;
    private float fadeSpeed = 0.5f;
    private float maxTransparency = 1f;
    private float currentTransparency = 0f;

    private void Start()
    {
        next_Btn.onClick.AddListener(OnClick_Next_Page);
        passion_Btn.onClick.AddListener(OnClick_Passion);
        photo_Btn.onClick.AddListener(OnClick_Photo);
        email_Btn.onClick.AddListener(OnClick_Email);
    }

    private void Update()
    {
        On_Fade_BackGround();
    }

    public void OnClick_Next_Page()
    {
        next_page = true;
    }

    public void OnClick_Passion()
    {
        Debug.Log("Passion 눌림");
    }

    public void OnClick_Photo()
    {
        Debug.Log("Photo 눌림");
    }

    public void OnClick_Email()
    {
        Debug.Log("Email 눌림");
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


    private void Loby_Set_Loby_Page()
    {
        start_Image.SetActive(false);
        backGround_Screen.gameObject.SetActive(false);
        loby_Iamage.gameObject.SetActive(true);
    }
}
