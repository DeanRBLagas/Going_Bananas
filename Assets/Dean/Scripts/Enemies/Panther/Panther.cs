using System.Collections;
using UnityEngine;

public class Panther : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private Vector2 detectionDistance = new Vector2(1f, 1f);
    [SerializeField] private float minDistanceToPlayer = 0.5f;
    [SerializeField] private float timeToTarget = 0.6f;
    [SerializeField] private float maxHorizontalJump = 1.5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    private Transform playerPos;
    private bool isChasing;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPos = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        if (!isChasing)
        {
            DetectPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;

            Vector2 start = transform.position;
            Vector2 target = playerPos.position;

            float direction = Mathf.Sign(playerPos.position.x - transform.position.x);
            target.x = transform.position.x + direction * maxHorizontalJump;

            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 10f, groundLayer);
            if (platformAbove.collider != null)
            {
                float platformTopY = platformAbove.collider.bounds.max.y;
                float playerY = playerPos.position.y;

                float platformLeft = platformAbove.collider.bounds.min.x;
                float platformRight = platformAbove.collider.bounds.max.x;

                bool pantherUnderPlatform = transform.position.x >= platformLeft && transform.position.x <= platformRight;
                bool platformBelowPlayer = platformTopY < playerY;

                if (pantherUnderPlatform && platformBelowPlayer)
                {
                    target.x = transform.position.x;
                    target.y = platformTopY + 0.1f;
                }
                else
                {
                    target.y = platformTopY + 0.1f;
                }
            }

            float gravity = Mathf.Abs(Physics2D.gravity.y);
            Vector2 distance = target - start;

            float vx = distance.x / timeToTarget;
            float vy = (distance.y + 0.5f * gravity * Mathf.Pow(timeToTarget, 2)) / timeToTarget;

            Vector2 jumpVelocity = new Vector2(vx, vy);
            rb.linearVelocity = jumpVelocity;
        }
    }

    private void DetectPlayer()
    {
        Vector2 panthPos = transform.position;
        Vector2 playerPosition = playerPos.position;
        Vector2 diff = playerPosition - panthPos;

        bool withinHorizontalRange = Mathf.Abs(diff.x) <= detectionDistance.x;
        bool withinVerticalRange = Mathf.Abs(diff.y) <= detectionDistance.y;

        if (withinHorizontalRange && withinVerticalRange)
        {
            isChasing = true;
        }
    }

    private void ChasePlayer()
    {
        if (rb.linearVelocity.y <= 0f)
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        }
        else
        {
            isGrounded = false;
        }

        float direction = Mathf.Sign(playerPos.position.x - transform.position.x);

        bool isPlayerAbove = false;
        float diff = playerPos.position.y - transform.position.y;
        if (diff >= 1)
        {
            isPlayerAbove = true;
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 10f, groundLayer);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                float platformTopY = platformAbove.collider.bounds.max.y;
                if (playerPos.position.y > platformTopY)
                {
                    shouldJump = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isChasing)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = new Vector2((collision.transform.position.x - transform.position.x), 0.3f).normalized;
                player.ApplyKnockback(knockbackDir * knockbackForce);
                IDamageable damageable = collision.GetComponent<IDamageable>();
                damageable?.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
