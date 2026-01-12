using UnityEngine;
using UnityEngine.EventSystems;

public class DeathUITrashArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropObj = eventData.pointerDrag; 
        DeathOptionUI card = dropObj.GetComponent<DeathOptionUI>();

        Audio.instances.PlaySFX("Cancel");

        GameController.instances.RemoveSkill(card.data);
        Pool.instances.DestroyObject(card.gameObject);
    }
}   
