using UnityEngine;

public class DamageComponent : DestroyOnCollide 
{
    [SerializeField] protected int damage = 1;
    [SerializeField] protected bool isTrigger = false;

    private void Awake()
        => GetComponent<BoxCollider2D>().isTrigger = isTrigger;
    
    
    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (isTrigger)
            return;

        if (CheckForCollision(other.gameObject))
        {
            if (other.gameObject.TryGetComponent<IHealthComponent>(out IHealthComponent h))
                h.OnDamage(damage);

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
                h.OnDamage(damage);

            Pool.instances.DestroyObject(this.gameObject);
        }
    }
}
