using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("カメラ設定")]
    public CameraBounds bounds;

    [Header("この部屋で猫に見つかるDay")]
    public bool detectDay1 = false;
    public bool detectDay2 = false;
    public bool detectDay3 = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // ========================================
        // カメラ範囲
        // ========================================

        if (CameraFollow.Instance != null && bounds != null)
        {
            CameraFollow.Instance.SetBounds(bounds);
        }

        // ========================================
        // 現在のDayを取得
        // ========================================

        string currentDay = UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().name;

        bool canDetect = false;

        if (currentDay == "Day1")
        {
            canDetect = detectDay1;
        }
        else if (currentDay == "Day2")
        {
            canDetect = detectDay2;
        }
        else if (currentDay == "Day3")
        {
            canDetect = detectDay3;
        }

        // ========================================
        // このDayでは猫に見つからない
        // ========================================

        if (!canDetect)
        {
            Debug.Log(currentDay + "：この部屋では猫に見つかりません。");
            return;
        }

        // ========================================
        // 猫に見つかる
        // ========================================

        if (CatDetectionManager.Instance == null)
        {
            Debug.LogError("CatDetectionManagerが見つかりません！");
            return;
        }

        CatDetectionManager.Instance.Detected();

        Debug.Log(currentDay + "：この部屋で猫に見つかった！");
    }
}