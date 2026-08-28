using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MouseMovement : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ゲームオーバーなら操作停止
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver())
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput = moveInput.normalized;

        // 左右の向きを変更
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        // ゲームオーバーなら完全停止
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }
}