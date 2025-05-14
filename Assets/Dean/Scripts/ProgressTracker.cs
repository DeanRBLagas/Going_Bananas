using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    [Header("World References")]
    public Transform player;
    public Transform startPoint;
    public Transform endPoint;

    [Header("UI References")]
    public RectTransform progressBar;
    public RectTransform playerMarker;

    private float totalDistance;

    void Start()
    {
        totalDistance = Vector2.Distance(startPoint.position, endPoint.position);
    }

    void Update()
    {
        float playerDistance = Vector2.Distance(startPoint.position, player.position);
        float progress = Mathf.Clamp01(playerDistance / totalDistance);

        // Move the player marker across the bar
        float barWidth = progressBar.rect.width;
        Vector2 newMarkerPos = playerMarker.anchoredPosition;
        float progressBarPos = progressBar.transform.position.x + 40;
        newMarkerPos.x = progress * barWidth - progressBarPos;
        playerMarker.anchoredPosition = newMarkerPos;
    }
}
