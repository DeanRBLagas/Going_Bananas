using System.Collections;
using UnityEngine;

public class Spider : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 turnInterval;
    [SerializeField] private Transform check1Pos;
    [SerializeField] private Transform check2Pos;
    [SerializeField] private Vector2 checkSize1 = new Vector2(0.1f, 0.01f);
    [SerializeField] private Vector2 checkSize2 = new Vector2(0.1f, 0.01f);
    [SerializeField] private LayerMask walkOnLayer;
    [SerializeField] private LayerMask notWalkOnLayer;
    [SerializeField] private Vector2 detectionDistance = new Vector2(10f, 10f);
    [SerializeField] private float attackInterval;
    [SerializeField] private GameObject projectilePrefab;
    private bool hasDetected;
    private Coroutine attackCoroutine;
    private Transform playerPos;
    private Vector2 currentDirection;
    private float turnTimer;
    private float directionChangeCooldown = 0.1f;
    private float lastDirectionChangeTime;

    private void Start()
    {
        playerPos = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        GetComponent<Rigidbody2D>().linearVelocity = currentDirection * speed;
        CheckForObstacles();
        DetectPlayer();
        if (!hasDetected)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                ChangeDirection();
            }
        }
    }

    private void DetectPlayer()
    {
        Vector2 spiderPos = transform.position;
        Vector2 playerPosition = playerPos.position;
        Vector2 diff = playerPosition - spiderPos;

        bool withinHorizontalRange = Mathf.Abs(diff.x) <= detectionDistance.x;
        bool withinVerticalRange = Mathf.Abs(diff.y) <= detectionDistance.y;

        hasDetected = withinHorizontalRange && withinVerticalRange;
        if (hasDetected)
        {
            transform.rotation = playerPos.position.x < transform.position.x ? Quaternion.Euler(0, -180, 0) : Quaternion.identity;
            attackCoroutine ??= StartCoroutine(Attack());
        }
        else if (!hasDetected && attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private void CheckForObstacles()
    {
        if (Time.time - lastDirectionChangeTime < directionChangeCooldown) return;

        bool ground1 = Physics2D.OverlapBox(check1Pos.position, checkSize1, 0f, walkOnLayer);
        bool ground2 = Physics2D.OverlapBox(check2Pos.position, checkSize2, 0f, walkOnLayer);
        bool other1 = Physics2D.OverlapBox(check1Pos.position, checkSize1, 0f, notWalkOnLayer);
        bool other2 = Physics2D.OverlapBox(check2Pos.position, checkSize2, 0f, notWalkOnLayer);

        if (!ground1 || !ground2 || other1 || other2)
        {
            ChangeDirection();
            lastDirectionChangeTime = Time.time;
        }
    }


    public void ChangeDirection()
    {
        float interval = (Random.Range(turnInterval.x, turnInterval.y));
        turnTimer = interval;
        currentDirection = (currentDirection == Vector2.right) ? Vector2.left : Vector2.right;
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackInterval);
        Instantiate(projectilePrefab, transform.position + transform.right * 1f, transform.rotation);
        attackCoroutine = StartCoroutine(Attack());
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
