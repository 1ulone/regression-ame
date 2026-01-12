using UnityEngine;

public class BossTriggerComponent : MonoBehaviour
{
    [SerializeField] private BossUI ui;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.name == "Player")
        {
            ui.StartBossUI();
            gameObject.SetActive(false);
        }
    }
}
