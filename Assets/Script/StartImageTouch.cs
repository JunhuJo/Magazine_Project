using UnityEngine;
using UnityEngine.EventSystems;

public class StartImageTouch : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private SystemManager system_Manager;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("���� �������� �Ѿ");
        system_Manager.next_page = true;
    }
}
