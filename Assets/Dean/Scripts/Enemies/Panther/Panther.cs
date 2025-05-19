using System.Collections;
using UnityEngine;

public class Panther : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private Vector2 detectionDistance = new Vector2(5f, 3f);
    [SerializeField] private float minDistanceToPlayer = 1.5f;
    [SerializeField] private float attackInterval;
    [SerializeField] private float slowInterval;
    private bool hasDetected;
    private Transform playerPos;
    private Coroutine attackCoroutine;
    private bool hasAttacked;

    private void Start()
    {
        playerPos = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Vector2 mosPos = transform.position;
        Vector2 playerPosition = playerPos.position;
        Vector2 diff = playerPosition - mosPos;

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
                transform.position = Vector3.MoveTowards(transform.position, playerPos.position, step);
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
        StartCoroutine(SlowPlayer());
        yield return new WaitForSeconds(attackInterval);
        attackCoroutine = StartCoroutine(Attack());
        hasAttacked = false;
    }

    private IEnumerator SlowPlayer()
    {
        Player player = playerPos.GetComponent<Player>();
        player.speed = player.maxSpeed / 2;
        yield return new WaitForSeconds(slowInterval);
        player.speed = player.maxSpeed;
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
