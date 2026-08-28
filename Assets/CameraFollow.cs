using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    [Header("追従するプレイヤー")]
    public Transform target;

    [Header("追従速度")]
    public float smoothSpeed = 5f;

    [Header("カメラの位置補正")]
    public Vector3 offset = new Vector3(0, 0, -10);

    private CameraBounds currentBounds;
    private Camera cam;

    private Vector3 basePosition;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    public void SetBounds(CameraBounds bounds)
    {
        currentBounds = bounds;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        if (currentBounds != null)
        {
            Bounds bounds = currentBounds.GetBounds();

            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                bounds.min.x + camWidth,
                bounds.max.x - camWidth
            );

            targetPosition.y = Mathf.Clamp(
                targetPosition.y,
                bounds.min.y + camHeight,
                bounds.max.y - camHeight
            );
        }

        basePosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = basePosition;
    }
}