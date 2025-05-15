using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public SnakeManager manager;
    public float detectionDistance = 0.5f;  // Distance to look ahead
    public LayerMask obstacleLayer;         // Layer to detect collisions with

    private void Update()
    {
        CheckForObstacles();
    }

    private void CheckForObstacles()
    {
        Vector2 direction = manager.GetCurrentDirection();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, obstacleLayer);

        if (hit.collider != null)
        {
            Debug.Log("Obstacle ahead! Turning.");
            manager.ChangeDirection();
        }
    }
}
