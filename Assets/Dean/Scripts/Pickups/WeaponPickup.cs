using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapon weaponData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LookAtMouse player = collision.GetComponent<LookAtMouse>();
        if (player != null)
        {
            player.PickupWeapon(weaponData);
            Destroy(gameObject);
        }
    }
}
