using System.Collections;
using UnityEngine;

public class Mosquito : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private Vector2 detectionDistance = new Vector2(5f, 3f);
    [SerializeField] private float minDistanceToPlayer = 1.5f;
    [SerializeField] private float attackInterval;
    [SerializeField] private float slowInterval;
    [SerializeField] private GameObject moneyDrop;
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
        Player player = playerPos.GetComponent<Player>();
        player.StartCoroutine(player.Slow(slowInterval));
        yield return new WaitForSeconds(attackInterval);
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
        GameObject money = Instantiate(moneyDrop, transform.position + new Vector3(0, 0, 20), transform.rotation);
        Rigidbody moneyrb = money.GetComponent<Rigidbody>();

        float x = Random.Range(-5f, 10f);
        float y = Random.Range(1f, 10f);
        float z = Random.Range(-5f, 10f);
        Vector3 force = new Vector3(x, y, z);

        moneyrb.AddForce(force, ForceMode.Impulse);
        Destroy(gameObject);
    }
}
