using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;

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
        Destroy(gameObject);
    }
}
