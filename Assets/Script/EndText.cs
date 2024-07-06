using UnityEngine;

public class EndText : MonoBehaviour
{
    [SerializeField] PageSwiper pageSwiper;

    private void Update()
    {
        ReSet_Text();
    }

    private void ReSet_Text()
    {
        if (pageSwiper.currentImageIndex  ==  0)
        {
            gameObject.SetActive(false);
        }
    }
}
