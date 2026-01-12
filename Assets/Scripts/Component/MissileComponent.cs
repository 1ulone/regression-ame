using System;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class MissileComponent : MonoBehaviour
{
    [SerializeField] private ShakeData explosionData;

    public BaseEnemy enemyRef { get; set; }
    public Transform target { get; set;}

    public float chaseSpeed = 1;
    private float speed = 0.1f;

    public void SetTarget(string tname)
    {
        if (tname != "Player")
        {
            target = GameObject.FindFirstObjectByType<BaseEnemy>().transform;
        } else {
            target = GameObject.FindFirstObjectByType<PlayerController>().transform;
        }
    }
    
    public void Update()
    {
        if (target == null)
            return;

        speed += Time.deltaTime / 2f;
        transform.position = Vector2.MoveTowards(transform.position, target.position, Mathf.Clamp(speed, speed, chaseSpeed) * Time.deltaTime);
    }

    private void OnDisable()
    {
        if (enemyRef != null)
            explosionHurtPlayer().Invoke();
    }

    private Action explosionHurtPlayer()
    {
        return ()=>
        {
            CameraShakerHandler.Shake(explosionData);
            DamageComponent e = Pool.instances.CreateObject("explosionHurtPlayer", transform.position, Vector2.zero).GetComponent<DamageComponent>();
            e.enemyReference = enemyRef;
            e.damage = 1;
        };
    }
}
