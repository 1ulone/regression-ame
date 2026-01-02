using UnityEngine;

public class DestroyOnCollide : MonoBehaviour
{
    [SerializeField] protected LayerMask collideTarget;

    protected bool CheckForCollision(GameObject g) { return (((1<<g.layer) & collideTarget) != 0); }
    
    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (CheckForCollision(other.gameObject))
            Pool.instances.DestroyObject(this.gameObject);
    }
}
