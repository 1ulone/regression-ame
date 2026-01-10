using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// using UnityEngine.UI;
// using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI instances;

    [SerializeField] private CanvasGroup pausePopup;
    [SerializeField] private GameObject settingsPopup; 
    [SerializeField] private GameObject warningPopup;

    [SerializeField] private Transform[] skillsHolder;

    private CanvasGroup pause;
    private bool onPause;
    private bool onTransitioning;
    private bool isQuitApp;

    private void Awake()
    {
        instances = this;
        pause = GetComponent<CanvasGroup>();

        onPause = false;
        pause.alpha = 0;

        pausePopup.alpha = 0;
        pausePopup.interactable = false;
        pausePopup.blocksRaycasts = false;

        settingsPopup.SetActive(false);
        warningPopup.SetActive(false);

        onTransitioning = false;
    }

    public void TogglePauseMenu()
    {
        if (onTransitioning)
            return;

        if (!onPause)
            StartCoroutine(enterTransition());
        else 
            StartCoroutine(exitTransition());
    }

    private IEnumerator enterTransition()
    {
        onTransitioning = true;
        onPause = true;
        Time.timeScale = 0;

        List<PlayerBuffData> data = GameController.instances.currentSave.playerSkill; 
        for (int i = 0; i < data.Count; i++)
        {
            GameObject card = Pool.instances.CreateObject("card", transform.position, Vector2.zero);
            DeathOptionUI option = card.GetComponent<DeathOptionUI>();

            card.transform.SetParent(skillsHolder[i]);
            option.data = data[i];
            option.icon.sprite = data[i].icon;
            option.desc.text = data[i].GetDescription();
        }

        yield return new WaitForSecondsRealtime(0.25f);

        pause.alpha = 1;
        onTransitioning = false;
    }

    private IEnumerator exitTransition()
    {
        onTransitioning = true;
        yield return new WaitForSecondsRealtime(0.25f);

        foreach(Transform t in skillsHolder)
        {
            if (t.childCount != 0)
                Pool.instances.DestroyObject(t.GetChild(0).gameObject);
        }

        pause.alpha = 0;
        onPause = false;
        Time.timeScale = 1;
        onTransitioning = false;
    }

    public void onContinue()
    {
        StartCoroutine(exitTransition());
    }

    private IEnumerator enterSettingsPopup(bool forSettings)
    {
        onTransitioning = true;

        pausePopup.alpha = 1;
        pausePopup.interactable = true;
        pausePopup.blocksRaycasts = true;

        yield return new WaitForSecondsRealtime(0.1f);
        if (forSettings)
            settingsPopup.SetActive(true);
        else 
            warningPopup.SetActive(true);

        yield return new WaitForSecondsRealtime(0.1f);
        onTransitioning = false;
    }

    private IEnumerator exitSettingsPopup(bool forSettings)
    {
        onTransitioning = true;

        yield return new WaitForSecondsRealtime(0.1f);
        if (forSettings)
            settingsPopup.SetActive(false);
        else 
            warningPopup.SetActive(false);

        yield return new WaitForSecondsRealtime(0.1f);
        pausePopup.alpha = 0;
        pausePopup.interactable = false;
        pausePopup.blocksRaycasts = false;

        onTransitioning = false;
    }

    public void onSettings()
    {
        if (onTransitioning)
            return; 
        
        StartCoroutine(enterSettingsPopup(true));
    }

    public void onToTitle()
    {
        if (onTransitioning)
            return;

        isQuitApp = false;
        StartCoroutine(enterSettingsPopup(false));
    }

    public void onToExit()
    {
        if (onTransitioning)
            return;

        isQuitApp = true;
        StartCoroutine(enterSettingsPopup(false));
    }

    // NOTE: Settings Popup Functions
    public void SettingsSaveAndExit()
    {
        if (onTransitioning)
            return;

        // TODO: save settings
        StartCoroutine(exitSettingsPopup(true));
    }

    public void SettingsDiscardAndExit()
    {
        if (onTransitioning)
            return;

        // TODO: dont save settings
        StartCoroutine(exitSettingsPopup(true));
    }
    
    // NOTE: Warning Popup Buttons Function
    public void OkButtonWarningPopup() 
    {
        if (onTransitioning)
            return;

        if (isQuitApp)
            Application.Quit();
        else 
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    public void CancelButtonWarningPopup() 
    {
        if (onTransitioning)
            return;

        StartCoroutine(exitSettingsPopup(false));
    }
}
