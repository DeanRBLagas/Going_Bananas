using UnityEngine;

public class SpeedPickup : BasePickup
{
    [SerializeField] private float speedIncrease;

    protected override void Collect()
    {
        player.maxSpeed += speedIncrease;
        if (Mathf.Approximately(player.speed, player.maxSpeed - speedIncrease))
        {
            player.speed = player.maxSpeed;
        }
        Destroy(gameObject);
    }
}
