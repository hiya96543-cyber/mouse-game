using UnityEngine;

public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player以外は無視
        if (!other.CompareTag("Player"))
            return;

        // ゲームオーバー
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManagerが見つかりません！");
        }
    }
}