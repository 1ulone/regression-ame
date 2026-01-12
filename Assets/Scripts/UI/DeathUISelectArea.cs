using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DeathUISelectArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (this.transform.childCount >= 3)
            return;

        GameObject dropObj = eventData.pointerDrag; 
        DeathOptionUI card = dropObj.GetComponent<DeathOptionUI>();
        card.parentAfterDrag = this.transform;

        Audio.instances.PlaySFX("Confirm");
        GameController.instances.AddSkill(card.data);
    }

    public void SyncWithData(List<PlayerBuffData> datas)
    {
        foreach (PlayerBuffData data in datas)
        {
            GameObject card = Pool.instances.CreateObject("card", transform.position, Vector2.zero);
            card.transform.SetParent(this.transform);
            DeathOptionUI option = card.GetComponent<DeathOptionUI>();

            option.data = data;
            option.icon.sprite = data.icon;
            option.title.text = data.tag;
            option.desc.text = data.GetDescription();
        }
    }
}
