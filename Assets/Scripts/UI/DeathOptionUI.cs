using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class DeathOptionUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image parentGroup;
    public Image icon;
    public TextMeshProUGUI desc;
    public PlayerBuffData data { get; set; }

    public Transform parentAfterDrag { get; set; }

    private Transform isRemoved;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.parent.TryGetComponent<DeathUISelectArea>(out DeathUISelectArea a))
            isRemoved = a.transform;
        
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        parentGroup.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mouse = Mouse.current.position.ReadValue();
        transform.position = mouse;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        if (transform.parent != isRemoved && isRemoved != null)
            GameController.instances.RemoveSkill(data);
            
        isRemoved = null;
        parentGroup.raycastTarget = true;
    }
}
