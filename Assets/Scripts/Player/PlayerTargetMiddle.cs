using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetMiddle : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 5;

    private void FixedUpdate()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 offset = new Vector2((mouse.x - player.position.x) / 2, (mouse.y - player.position.y) / 2);
        transform.position = player.position + Vector3.ClampMagnitude(offset, maxDistance);
    }
}
