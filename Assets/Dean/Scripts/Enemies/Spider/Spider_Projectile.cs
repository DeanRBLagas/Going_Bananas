using UnityEngine;

public class Spider_Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float arcHeight = 1;
    [SerializeField] private float knockbackForce = 1f;
    
    private Vector3 startPos;
    private Vector3 targetPos;

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

        transform.SetPositionAndRotation(nextPos, LookAt2D(nextPos - transform.position));

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
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                player.ApplyKnockback(knockbackDir * knockbackForce);
            }
        }
        Arrived();
    }
}
