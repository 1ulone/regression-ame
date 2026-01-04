using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class TitleScreenLogic : MonoBehaviour
{
    [SerializeField] GameObject popup;
    [SerializeField] GameObject options; 
    [SerializeField] TextMeshProUGUI message;

    private void Start()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        popup.SetActive(false);
    }

    public void onPlayGame()
    {
        StartCoroutine(loadingLevel());
    }

    private IEnumerator loadingLevel()
    {
        SaveData currentData = GameController.instances.LoadData();
        yield return SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(0.05f);
        GameObject.FindFirstObjectByType<TimeController>().countdown = currentData.time;
        // GameObject.FindFirstObjectByType<PlayerController>().buffs = currentData.playerSkill;

        yield return SceneManager.UnloadSceneAsync(0);
    }

    public void onExit()
    {
        Application.Quit();
    }

    public void onClearSave()
    {
        GameController.instances.ClearSave();
        StartCoroutine(ConfirmPopup());
    }

    public void onClearSavePopup()
    {
        popup.SetActive(true);
        if (!GameController.instances.CheckForFile())
        {
            message.text = "No Save File found.";
            StartCoroutine(CancelPopup());
            options.SetActive(false);
        }
        else 
        {
            message.text = "Are you sure you want to delete your Save File?";
            options.SetActive(true);
        }
    }

    public void onClearSavePopupCancel()
    {
        popup.SetActive(false);
    }

    private IEnumerator CancelPopup()
    {
        yield return new WaitForSecondsRealtime(2);
        popup.SetActive(false);
    }

    private IEnumerator ConfirmPopup()
    {
        message.text = "";
        options.SetActive(false);

        yield return new WaitForSecondsRealtime(0.1f);
        message.text = "Save File succesfully removed";

        yield return new WaitForSecondsRealtime(2);
        popup.SetActive(false);
    }
}
