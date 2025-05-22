using UnityEngine;

public class DoubleJumpPickup : BasePickup
{
    protected override void Collect()
    {
        player.maxDoubleJumps++;
        Destroy(gameObject);
    }
}
