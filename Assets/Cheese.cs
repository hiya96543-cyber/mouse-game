using UnityEngine;

public class Cheese : MonoBehaviour
{
    [Header("取得するチーズ数")]
    public int cheeseValue = 1;

    private Collider2D cheeseCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        cheeseCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // チーズを取得
        GameManager.Instance.AddCheese(cheeseValue);

        // チーズを非表示
        SetCollected();
    }

    /// <summary>
    /// チーズを取得済みにする
    /// </summary>
    private void SetCollected()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (cheeseCollider != null)
        {
            cheeseCollider.enabled = false;
        }
    }

    /// <summary>
    /// チーズを復活させる
    /// </summary>
    public void Respawn()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (cheeseCollider != null)
        {
            cheeseCollider.enabled = true;
        }
    }
}