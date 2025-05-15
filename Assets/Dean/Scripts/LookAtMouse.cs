using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform SpawnPos;

    private void Update()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 direction = mouseScreenPosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (projectilePrefab != null)
            {
                Instantiate(projectilePrefab, SpawnPos.position, transform.rotation);
            }
        }
    }
}
