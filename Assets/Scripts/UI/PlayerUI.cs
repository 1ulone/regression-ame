using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image healthbar;
    [SerializeField] private TextMeshProUGUI tm;

    [SerializeField] private Image stamina;
    private float maxStamina;

    public void UpdateHealth(int health, int maxHealth)
    {
        healthbar.fillAmount = (float)health/(float)maxHealth;
        tm.text = health.ToString() + "/" + maxHealth.ToString();
    }
    
    public void SetStamina(float mStam)
    {
        maxStamina = mStam;
        stamina.fillAmount = 0;
    }

    private void Update()
    {
        if (stamina.fillAmount < 1)
        {
            stamina.fillAmount += Time.deltaTime / maxStamina;
        }
    }
}
