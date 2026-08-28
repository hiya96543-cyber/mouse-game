using System.Collections;
using UnityEngine;

public class CrossAttack : MonoBehaviour
{
    [Header("予告")]
    public GameObject warning;

    [Header("攻撃イラスト")]
    public GameObject attackVisual;

    [Header("攻撃判定")]
    public Collider2D verticalHitBox;
    public Collider2D horizontalHitBox;

    [Header("時間設定")]
    public float warningTime = 0.8f;
    public float attackTime = 0.2f;

    [Header("予告の点滅")]
    public float blinkInterval = 0.15f;

    [Header("プレイヤー追跡")]
    public float trackingTime = 0.5f;

    private Transform player;


    private void Start()
    {
        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;

        StartCoroutine(AttackSequence());
    }


    private IEnumerator AttackSequence()
    {
        // 初期状態
        if (warning != null)
            warning.SetActive(true);

        if (attackVisual != null)
            attackVisual.SetActive(false);

        if (verticalHitBox != null)
            verticalHitBox.enabled = false;

        if (horizontalHitBox != null)
            horizontalHitBox.enabled = false;


        // ========================================
        // プレイヤー追跡
        // ========================================
        float trackingElapsed = 0f;

        while (trackingElapsed < trackingTime)
        {
            if (player != null)
            {
                transform.position = new Vector3(
                    player.position.x,
                    player.position.y,
                    transform.position.z
                );
            }

            yield return null;

            trackingElapsed += Time.deltaTime;
        }


        // ========================================
        // 残りの予告時間
        // ========================================
        float elapsed = trackingTime;

        while (elapsed < warningTime)
        {
            if (warning != null)
                warning.SetActive(!warning.activeSelf);

            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval;
        }


        // 予告終了
        if (warning != null)
            warning.SetActive(false);


        // ========================================
        // 攻撃開始
        // ========================================
        if (attackVisual != null)
            attackVisual.SetActive(true);

        if (verticalHitBox != null)
            verticalHitBox.enabled = true;

        if (horizontalHitBox != null)
            horizontalHitBox.enabled = true;


        // 十字攻撃音
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(
                SoundManager.Instance.crossAttack
            );
        }


        // 攻撃時間
        yield return new WaitForSeconds(attackTime);


        // ========================================
        // 攻撃終了
        // ========================================
        if (verticalHitBox != null)
            verticalHitBox.enabled = false;

        if (horizontalHitBox != null)
            horizontalHitBox.enabled = false;

        if (attackVisual != null)
            attackVisual.SetActive(false);

        Destroy(gameObject);
    }
}