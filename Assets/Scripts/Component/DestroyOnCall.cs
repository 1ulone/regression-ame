using UnityEngine;

public class DestroyOnCall : MonoBehaviour
{
    public void DestroyObject()
    {
        Pool.instances.DestroyObject(this.gameObject);
    }
}
