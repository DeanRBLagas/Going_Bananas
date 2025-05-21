using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    [Header("World References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform monkey;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("UI References")]
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private RectTransform playerMarker;
    [SerializeField] private RectTransform monkeyMarker;

    private float totalDistance;

    private void Start()
    {
        totalDistance = Vector2.Distance(startPoint.position, endPoint.position);
        player = FindFirstObjectByType<Player>().transform;
        monkey = FindFirstObjectByType<MonkeyTracker>().transform;
    }

    private void Update()
    {
        float barWidth = progressBar.rect.width;
        float progressBarPos = progressBar.transform.position.x + 40;

        //Player
        float playerDistance = Vector2.Distance(new Vector2(startPoint.position.x, 0), new Vector2(player.position.x, 0));
        float playerProgress = Mathf.Clamp01(playerDistance / totalDistance);
        Vector2 playerMarkerPos = playerMarker.anchoredPosition;
        playerMarkerPos.x = playerProgress * barWidth - progressBarPos;
        playerMarker.anchoredPosition = playerMarkerPos;

        //Monkey
        float monkeyDistance = Vector2.Distance(new Vector2(startPoint.position.x, 0), new Vector2(monkey.position.x, 0));
        float monkeyProgress = Mathf.Clamp01(monkeyDistance / totalDistance);
        Vector2 monkeyMarkerPos = monkeyMarker.anchoredPosition;
        monkeyMarkerPos.x = monkeyProgress * barWidth - progressBarPos;
        monkeyMarker.anchoredPosition = monkeyMarkerPos;
    }
}
