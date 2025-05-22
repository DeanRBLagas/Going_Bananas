using System.Collections;
using UnityEngine;

public class Dung_Beetle : MonoBehaviour, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private Vector2 detectionDistance = new Vector2(5f, 3f);
    [SerializeField] private float attackInterval;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject moneyDrop;
    private bool hasDetected;
    private Coroutine attackCoroutine;
    private Transform playerPos;

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
        Vector2 beetlePos = transform.position;
        Vector2 playerPosition = playerPos.position;
        Vector2 diff = playerPosition - beetlePos;

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
        Instantiate(moneyDrop);
        Destroy(gameObject);
    }
}
