using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
    public static DeathUI instances;

    [SerializeField] private CanvasGroup deathScreen;
    [SerializeField] private DeathUISelectArea selectArea;
    [SerializeField] private Transform[] holder; 

    private void Awake()
    { 
        instances = this; 

        deathScreen.alpha = 0;
        foreach(Transform i in holder)
        {
            i.gameObject.SetActive(false);
        }
    }

    public void StartDeathTransition(EnemyData dataGet)
    {
        deathScreen.alpha = 0;
        Time.timeScale = 0;

        for (int i = 0; i < dataGet.data.Length; i++)
        {
            DeathOptionUI option = Pool.instances.CreateObject("card", transform.position, Vector2.zero).GetComponent<DeathOptionUI>();

            holder[i].gameObject.SetActive(true);

            option.transform.SetParent(holder[i]);
            option.data = dataGet.data[i];
            option.icon.sprite = dataGet.data[i].icon;

            Dictionary<string, float> nonZeroValue = dataGet.data[i].GetNonZeroValues();
            string[] tags = nonZeroValue.Keys.ToArray();
            float[] values = nonZeroValue.Values.ToArray();

            for (int x = 0; x < ( nonZeroValue.Count > 5 ? 5 : nonZeroValue.Count ); x++)
            {
                option.stats[x].gameObject.SetActive(true);
                option.stats[x].text = tags[x] + " " + (values[x] > 0 ? "+" : "-") + values[x];
            }
        }

        selectArea.SyncWithData(GameController.instances.LoadData().playerSkill);
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        deathScreen.alpha = 0.25f;

        yield return new WaitForSecondsRealtime(0.15f);
        deathScreen.alpha = 0.5f;

        yield return new WaitForSecondsRealtime(0.15f);
        deathScreen.alpha = 0.75f;

        yield return new WaitForSecondsRealtime(0.15f);
        deathScreen.alpha = 1f;
    }

    public void RestartLevel()
    {
        GameController.instances.RestartLevel();
    }
}
