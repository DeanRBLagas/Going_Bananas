using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;

    public void Initialize(float speed, int damage)
    {
        this.speed = speed;
        this.damage = damage;
    }

    private void Start()
    {
        Destroy(gameObject, 4f);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
        Destroy(gameObject);
    }
}
