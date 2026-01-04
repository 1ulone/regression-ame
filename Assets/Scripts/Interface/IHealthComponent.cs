using UnityEngine;

public interface IHealthComponent  
{
    public void OnDamage(int damage, MonoBehaviour reference = null) {}
}
