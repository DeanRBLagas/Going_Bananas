using Unity.VisualScripting;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public SnakeManager manager;
    [SerializeField] private Transform checkPos;
    [SerializeField] private Vector2 checkSize = new Vector2(0.1f, 0.01f);
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private int damage;

    private void Update()
    {
        CheckForObstacles();
    }

    private void CheckForObstacles()
    {
        if (Physics2D.OverlapBox(checkPos.position, checkSize, 0f, obstacleLayer))
        {
            manager.ChangeDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);
        }
    }
}
