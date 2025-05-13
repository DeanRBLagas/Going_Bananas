using UnityEngine;
using UnityEngine.UI;

public class ProgressTracker : MonoBehaviour
{
    [Header("World References")]
    public Transform player;
    public Transform startPoint;
    public Transform endPoint;

    [Header("UI References")]
    public RectTransform progressBar; // The background bar
    public RectTransform playerMarker; // The player's icon

    private float totalDistance;

    void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("StartPoint or EndPoint not assigned.");
            return;
        }

        totalDistance = Vector2.Distance(startPoint.position, endPoint.position);
    }

    void Update()
    {
        float playerDistance = Vector2.Distance(startPoint.position, player.position);
        float progress = Mathf.Clamp01(playerDistance / totalDistance); // Ensure value is between 0 and 1

        // Move the player marker across the progress bar
        float barWidth = progressBar.rect.width / 2;
        Vector2 markerPos = playerMarker.anchoredPosition;
        markerPos.x = progress * barWidth;
        playerMarker.anchoredPosition = markerPos;
    }
}
