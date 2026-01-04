using UnityEngine;
using UnityEngine.EventSystems;

public class DeathOptionHolder : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (this.transform.childCount == 1)
            return;

        GameObject dropObj = eventData.pointerDrag; 
        dropObj.GetComponent<DeathOptionUI>().parentAfterDrag = this.transform;
    }
}
