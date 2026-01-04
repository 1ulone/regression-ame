using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
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

            Dictionary<string, float> nonZeroValue = data.GetNonZeroValues();
            string[] tags = nonZeroValue.Keys.ToArray();
            float[] values = nonZeroValue.Values.ToArray();

            for (int x = 0; x < ( nonZeroValue.Count > 5 ? 5 : nonZeroValue.Count ); x++)
            {
                option.stats[x].gameObject.SetActive(true);
                option.stats[x].text = tags[x] + " " + (values[x] > 0 ? "+" : "-") + values[x];
            }

        }
    }
}
