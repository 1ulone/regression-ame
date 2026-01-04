using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DeathUI : MonoBehaviour
{
    public static DeathUI instances;

    [SerializeField] private CanvasGroup deathScreen;
    [SerializeField] private DeathUISelectArea selectArea;
    [SerializeField] private Transform[] holder; 
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private GameObject optionsTime;
    [SerializeField] private GameObject dropArea;
    [SerializeField] private GameObject skillRestartButton;

    private void Awake()
    { 
        instances = this; 

        deathScreen.alpha = 0;
        message.text = "";
        foreach(Transform i in holder)
            i.gameObject.SetActive(false);

        skillRestartButton.SetActive(false);
        dropArea.SetActive(false);
        optionsTime.SetActive(false);
    }

    public void StartDeathTransitionByTime()
    {
        deathScreen.alpha = 0;
        Time.timeScale = 0;

        skillRestartButton.SetActive(false);
        dropArea.SetActive(false);
        optionsTime.SetActive(true);

        StartCoroutine(TransitionByTime());
    }

    public void StartDeathTransition(EnemyData dataGet)
    {
        deathScreen.alpha = 0;
        Time.timeScale = 0;
        message.text = "";

        skillRestartButton.SetActive(true);
        dropArea.SetActive(true);
        optionsTime.SetActive(false);


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

    private IEnumerator TransitionByTime()
    {
        yield return TransitionCoroutine();
        message.text = "You are given more time, Do you give up now?";
        optionsTime.SetActive(true);
    }

    public void ReturnToTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    public void ReturnToTitleByTime()
    {
        GameController.instances.AddTime(30);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    public void RestartLevel()
    {
        GameController.instances.RestartLevel();
    }

    public void RestartLevelByTime()
    {
        GameController.instances.AddTime(30);
        GameController.instances.RestartLevel();
    }
}
