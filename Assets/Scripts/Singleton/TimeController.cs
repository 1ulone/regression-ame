using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class TimeController : MonoBehaviour
{
    public static TimeController instances;

    [SerializeField] private TextMeshProUGUI timer;
    public float countdown;
    private Coroutine currentCoroutine;

    private void Awake()
        => instances = this;

    private void Start()
    {
        countdown = GameController.instances.currentSave.time;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (countdown <= 0)
        {
            DeathUI.instances.StartDeathTransitionByTime();
            return;
        }

        countdown -= Time.deltaTime;

        TimeSpan timeSpan = TimeSpan.FromSeconds(countdown);
        string timeFormat = @"mm\:ss"; 
        string formattedTime = timeSpan.ToString(timeFormat);

        timer.text = formattedTime; 
    }

    public void HitStop(float t)
    {
        if (currentCoroutine != null)
            return;

        currentCoroutine = StartCoroutine(HitStopCoroutine(t));
    }

    private IEnumerator HitStopCoroutine(float t)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(t);
        
        Time.timeScale = 1; 
        currentCoroutine = null;
    }

}
