using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenLogic : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    public void onNewGame()
    {
        //set time to 2 minutes;
        GameController.startGameTime = 120;
        StartCoroutine(loadingLevel());
    }

    private IEnumerator loadingLevel()
    {
        yield return SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(0.05f);
        GameObject.FindFirstObjectByType<TimeController>().countdown = GameController.startGameTime;

        yield return SceneManager.UnloadSceneAsync(0);
    }

    public void onLoadGame()
    {

    }

    public void onExit()
    {
        Application.Quit();
    }
}
