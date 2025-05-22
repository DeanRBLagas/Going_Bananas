using UnityEngine;

public class BasePickup : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.GetComponent<Player>();
        if (player != null)
        {
            Collect();
        }
    }

    protected virtual void Collect()
    {

    }
}
