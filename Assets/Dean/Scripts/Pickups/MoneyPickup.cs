using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [SerializeField] private int moneyAmount;

    private Player player;

    private void OnTriggerEnter(Collider collision)
    {
        player = collision.GetComponentInParent<Player>();
        if (player != null)
        {
            Collect();
        }
    }
    private void Collect()
    {
        player.money += moneyAmount;
        Destroy(gameObject);
    }
}
