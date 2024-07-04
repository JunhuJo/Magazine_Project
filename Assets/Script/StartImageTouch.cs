using UnityEngine;
using UnityEngine.EventSystems;

public class StartImageTouch : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private SystemManager system_Manager;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("다음 페이지로 넘어감");
        system_Manager.next_page = true;
    }
}
