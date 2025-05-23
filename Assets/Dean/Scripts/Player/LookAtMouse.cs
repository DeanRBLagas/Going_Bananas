using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField] private List<Weapon> weaponTypes = new List<Weapon>();
    [SerializeField] private Transform spawnPos;

    private int currentWeaponIndex = 0;
    private float lastAttackTime;
    private bool isFiringHeld;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 direction = mouseScreenPosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Weapon weapon = weaponTypes[currentWeaponIndex];
        if (weapon.holdToFire && isFiringHeld && Time.time >= lastAttackTime + weapon.attackCooldown)
        {
            FireWeapon();
        }

        if (direction.x < transform.position.x)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        Weapon weapon = weaponTypes[currentWeaponIndex];

        if (weapon.holdToFire)
        {
            if (context.performed)
            {
                isFiringHeld = true;
                FireWeapon();
            }
            else if (context.canceled)
            {
                isFiringHeld = false;
            }
        }
        else
        {
            if (context.performed && Time.time >= lastAttackTime + weapon.attackCooldown)
            {
                FireWeapon();
            }
        }
    }

    private void FireWeapon()
    {
        Weapon weapon = weaponTypes[currentWeaponIndex];

        GameObject bullet = Instantiate(weapon.bulletPrefab, spawnPos.position, transform.rotation);
        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile?.Initialize(weapon.bulletSpeed, weapon.bulletDamage);

        lastAttackTime = Time.time;
    }

    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SetWeapon(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SetWeapon(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SetWeapon(2);
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            SetWeapon(3);
    }

    private void SetWeapon(int index)
    {
        if (index >= 0 && index < weaponTypes.Count)
        {
            currentWeaponIndex = index;
            spriteRenderer.sprite = weaponTypes[index].sprite;
        }
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        weaponTypes.Add(newWeapon);
        currentWeaponIndex = weaponTypes.Count - 1;
    }
}
