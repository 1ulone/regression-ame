using UnityEngine;
using System.Collections;

public class DeathUI : MonoBehaviour
{
    public static DeathUI instances;

    [SerializeField] private CanvasGroup deathScreen;

    private void Awake()
        => instances = this;

    public void StartDeathTransition(int diedTo)
    {
        deathScreen.alpha = 0;
        Time.timeScale = 0;
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
}
