using UnityEngine;

public class DamageComponent : DestroyOnCollide 
{
    public int damage { get; set; }
    [SerializeField] protected bool isTrigger = false;
    [SerializeField] protected bool willbeDestroyed = true; //<-- fucking redundant i know. so for melee attack it wont be destroyed until it finished 
    public BaseEnemy enemyReference; //<-- shit here too, shit just need to work for now ong

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = isTrigger;
        damage = 1;
    }
    
    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (isTrigger)
            return;

        if (CheckForCollision(other.gameObject))
        {
            if (other.gameObject.TryGetComponent<IHealthComponent>(out IHealthComponent h))
                h.OnDamage(damage, enemyReference);

            if (willbeDestroyed)
                Pool.instances.DestroyObject(this.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTrigger)
            return;

        if (CheckForCollision(other.gameObject))
        {
            if (other.gameObject.TryGetComponent<IHealthComponent>(out IHealthComponent h))
                h.OnDamage(damage, enemyReference);

            if (willbeDestroyed)
                Pool.instances.DestroyObject(this.gameObject);
        }
    }
}
