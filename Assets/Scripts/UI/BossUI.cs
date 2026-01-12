using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BossUI : MonoBehaviour
{
    [SerializeField] private Image healthbar;
    [SerializeField] private TextMeshProUGUI bossName;
    private float maxHealth;
    private CanvasGroup panel;
    private bool healthBarAnimation;

    public bool startBossBattle;

    private void Start()
    {
        panel = GetComponent<CanvasGroup>();
        panel.alpha = 0;
        startBossBattle = false;
    }

    private void Update()
    {
        if (healthBarAnimation)
            healthbar.fillAmount += Time.deltaTime * 2;
    }

    public void StartBossUI()
    {
        StartCoroutine(startPanel());
    }

    private IEnumerator startPanel()
    {
        panel.alpha = 1;
        healthBarAnimation = true;
        healthbar.fillAmount = 0;

        yield return new WaitUntil(()=> healthbar.fillAmount == 1);

        bossName.text = "NINOMAE INA'NIS";
        healthBarAnimation = false;

        yield return new WaitForSeconds(0.5f);
        startBossBattle = true;
    }

    public void setMaxHealth(int h) 
    {
        maxHealth = (float)h;
    }

    public void UpdateHealthbar(int h)
    {
        if (healthBarAnimation)
            return;

        healthbar.fillAmount = h / maxHealth; 
    }
}
