using UnityEngine;

public class Spike : MonoBehaviour
{
    public float topTolerance = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Vector2 contactPoint = collision.bounds.center;
            Vector2 spikeTop = transform.position + Vector3.up * (GetComponent<Collider2D>().bounds.extents.y - topTolerance);

            if (contactPoint.y > spikeTop.y)
            {
                player.Die();
            }
        }
    }
}
