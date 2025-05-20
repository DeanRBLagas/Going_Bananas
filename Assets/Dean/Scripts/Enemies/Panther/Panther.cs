using System.Collections;
using UnityEngine;

public class Panther : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private int damage;
    [SerializeField] private Vector2 detectionDistance = new Vector2(1f, 1f);
    [SerializeField] private float minDistanceToPlayer = 0.5f;
    [SerializeField] private float attackInterval;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool shouldJump;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float timeToTarget = 0.6f;
    [SerializeField] private float maxHorizontalJump = 1.5f;

    private bool hasDetected;
    private Coroutine attackCoroutine;
    private bool hasAttacked;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerPos = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        if (rb.linearVelocity.y <= 0f) // Only check if falling or stationary
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        }
        else
        {
            isGrounded = false;
        }

        float direction = Mathf.Sign(playerPos.position.x - transform.position.x);

        //bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 10f, 1 << playerPos.gameObject.layer);
        bool isPlayerAbove = false;
        float diff = playerPos.position.y - transform.position.y;
        if (diff >= 1)
        {
            isPlayerAbove = true;
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2 (direction * speed, rb.linearVelocity.y);

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
        //DetectPlayer();
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;

            Vector2 start = transform.position;
            Vector2 target = playerPos.position;

            float direction = Mathf.Sign(playerPos.position.x - transform.position.x);

            // Default: jump a short distance forward
            target.x = transform.position.x + direction * maxHorizontalJump;

            // Raycast to find platform above
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 10f, groundLayer);
            if (platformAbove.collider != null)
            {
                float platformTopY = platformAbove.collider.bounds.max.y;
                float playerY = playerPos.position.y;

                float platformLeft = platformAbove.collider.bounds.min.x;
                float platformRight = platformAbove.collider.bounds.max.x;

                // Check if Panther is horizontally under the platform
                bool pantherUnderPlatform = transform.position.x >= platformLeft && transform.position.x <= platformRight;

                // Check if the platform is below the player
                bool platformBelowPlayer = platformTopY < playerY;

                if (pantherUnderPlatform && platformBelowPlayer)
                {
                    // Jump straight up to the platform
                    target.x = transform.position.x;
                    target.y = platformTopY + 0.1f;
                }
                else
                {
                    // Default behavior: jump forward, but Y to platform height
                    target.y = platformTopY + 0.1f;
                }
            }

            // Physics-based jump arc
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

        hasDetected = withinHorizontalRange && withinVerticalRange;
        if (hasDetected && !hasAttacked)
        {
            float distanceToPlayer = diff.magnitude;

            if (distanceToPlayer > minDistanceToPlayer)
            {
                transform.rotation = playerPos.position.x < transform.position.x ? Quaternion.Euler(0, -180, 0) : Quaternion.identity;
                float step = speed * Time.deltaTime;
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }
            else
            {
                attackCoroutine ??= StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        hasAttacked = true;
        IDamageable damageable = playerPos.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
        yield return new WaitForSeconds(attackInterval);
        attackCoroutine = StartCoroutine(Attack());
        hasAttacked = false;
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
