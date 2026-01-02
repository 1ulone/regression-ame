using UnityEngine;

public class DamageComponent : DestroyOnCollide 
{
    [SerializeField] protected int damage = 1;
    
    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (CheckForCollision(other.gameObject))
        {
            if (other.gameObject.TryGetComponent<IHealthComponent>(out IHealthComponent h))
                h.OnDamage(damage);

            Pool.instances.DestroyObject(this.gameObject);
        }
    }
}
