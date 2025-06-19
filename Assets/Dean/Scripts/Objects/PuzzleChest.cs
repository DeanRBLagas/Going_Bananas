using System.Collections.Generic;
using UnityEngine;

public class PuzzleChest : MonoBehaviour
{
    public bool canInteract;
    public Player player;
    public int cost;
    public bool looted;
    public List<GameObject> possibleRewards;
    [SerializeField] private Vector2Int costParameters;
    private ChestManager chestManager;

    private void Start()
    {
        chestManager = FindFirstObjectByType<ChestManager>();
        cost = Random.Range(costParameters.x, costParameters.y + 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (chestManager != null && collision.gameObject.tag == "Player")
        {
            player = collision.GetComponent<Player>();
            chestManager.chest = this;
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (chestManager != null && collision.gameObject.tag == "Player")
        {
            player = null;
            chestManager.chest = null;
            canInteract = false;
        }
    }
}
