using UnityEngine;

public class MiniMapPlayer : MonoBehaviour
{
    public Transform player;
    public RectTransform marker;

    public Vector2 mapMin;
    public Vector2 mapMax;

    private RectTransform mapRect;

    void Start()
    {
        mapRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (player == null || marker == null)
            return;

        float x = Mathf.InverseLerp(
            mapMin.x,
            mapMax.x,
            player.position.x
        );

        float y = Mathf.InverseLerp(
            mapMin.y,
            mapMax.y,
            player.position.y
        );

        float mapX = (x - 0.5f) * mapRect.rect.width;
        float mapY = (y - 0.5f) * mapRect.rect.height;

        marker.anchoredPosition = new Vector2(mapX, mapY);
    }
}