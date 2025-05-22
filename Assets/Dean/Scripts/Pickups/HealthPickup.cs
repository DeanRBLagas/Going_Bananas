using UnityEngine;

public class HealthPickup : BasePickup
{
    [SerializeField] private int healAmount;

    protected override void Collect()
    {
        player.currentHealth = Mathf.Min(player.currentHealth + healAmount, player.maxHealth);
        Destroy(gameObject);
    }
}
