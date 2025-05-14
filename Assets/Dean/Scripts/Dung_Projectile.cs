using UnityEngine;

public class Dung_Projectile : MonoBehaviour
{
    public Vector3 targetPos;
    public float speed = 10;
    public float arcHeight = 1;
    public float knockbackForce = 10f;


    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        Transform player = FindFirstObjectByType<Player>()?.transform;
        if (player != null)
        {
            targetPos = player.position;
        }
    }

    void Update()
    {
        float x0 = startPos.x;
        float x1 = targetPos.x;
        float dist = x1 - x0;
        float nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
        float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
        float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
        Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);

        transform.rotation = LookAt2D(nextPos - transform.position);
        transform.position = nextPos;

        if (nextPos == targetPos) Arrived();
    }

    void Arrived()
    {
        Destroy(gameObject);
    }
    static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Hit player.");
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                player.ApplyKnockback(knockbackDir * knockbackForce);
            }
            Arrived();
        }
    }
}
