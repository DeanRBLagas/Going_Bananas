using UnityEngine;

[System.Serializable]
public class Weapon
{
    public string name;
    public Sprite sprite;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public int bulletDamage = 1;
    public float attackCooldown = 0.5f;
    public bool holdToFire = false;
    public int amount = 1;
    public float spread = 0f;
}
