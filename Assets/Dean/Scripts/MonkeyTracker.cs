using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonkeyTracker : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float startSpeed;
    [SerializeField] private Image monkeyMarkerColour;

    private Transform playerPos;
    private bool chasing;
    [SerializeField] private float speed;

    private void Start()
    {
        playerPos = FindFirstObjectByType<Player>().transform;
        speed = startSpeed;
        StartCoroutine(StartChasing());
    }

    private void Update()
    {
        if (chasing)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            float start = startPoint.position.x;
            float end = endPoint.position.x;

            float diff = (end - start);

            if (playerPos.position.x >= diff / 3 * 2)
            {
                speed = startSpeed * 2f;
                monkeyMarkerColour.color = Color.darkRed;
            }
            else if (playerPos.position.x >= diff / 3)
            {
                speed = startSpeed * 1.5f;
                monkeyMarkerColour.color = Color.orangeRed;
            }

            if (playerPos.position.x >= end)
            {
                chasing = false;
            }

            if (transform.position.x >= playerPos.position.x)
            {
                Player player = playerPos.GetComponent<Player>();
                player.enabled = false;
            }
        }
    }

    private IEnumerator StartChasing()
    {
        yield return new WaitForSeconds(5);
        chasing = true;
    }
}
