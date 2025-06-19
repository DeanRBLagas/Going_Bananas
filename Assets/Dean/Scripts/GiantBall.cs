using UnityEngine;

public class GiantBall : MonoBehaviour
{
    private Player player;
    public bool canKill;
    private float speed;
    void Update()
    {
        speed = GetComponent<Rigidbody>().linearVelocity.magnitude;
        if (speed < 0.5)
        {
            canKill = false;
        }
        else
        {
            canKill = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        player = collision.GetComponentInParent<Player>();
        if (player != null && canKill)
        {
            player.Die();
        }
    }
}
