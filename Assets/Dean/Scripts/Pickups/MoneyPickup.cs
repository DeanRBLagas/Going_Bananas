using UnityEngine;

public class MoneyPickup : BasePickup
{
    [SerializeField] private int moneyAmount;

    protected override void Collect()
    {
        player.money += moneyAmount;
        Destroy(gameObject);
    }
}
