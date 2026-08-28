using System.Collections;
using UnityEngine;

public class PawAttack : MonoBehaviour
{
    public GameObject warning;
    public GameObject paw;
    public Collider2D hitBox;

    [Header("攻撃設定")]
    public float warningTime = 2f;
    public float hitTime = 0.2f;

    [Header("点滅設定")]
    public float startBlinkInterval = 0.2f;
    public float endBlinkInterval = 0.05f;

    private CameraShake cameraShake;

    private IEnumerator Start()
    {
        // CameraShakeを取得
        cameraShake = FindFirstObjectByType<CameraShake>();

        // 初期状態
        warning.SetActive(true);
        paw.SetActive(false);
        hitBox.enabled = false;

        // 予告音
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(
                SoundManager.Instance.attackWarning
            );
        }

        // 点滅開始
        StartCoroutine(BlinkWarning());

        // 予告時間
        yield return new WaitForSeconds(warningTime);

        // 攻撃
        warning.SetActive(false);
        paw.SetActive(true);
        hitBox.enabled = true;

        // 攻撃音
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(
                SoundManager.Instance.pawAttack
            );
        }

        // 画面揺れ
        if (cameraShake != null)
        {
            cameraShake.Shake(0.15f, 0.2f);
        }

        // 攻撃時間
        yield return new WaitForSeconds(hitTime);

        // 攻撃終了
        hitBox.enabled = false;

        Destroy(gameObject);
    }

    private IEnumerator BlinkWarning()
    {
        SpriteRenderer sr = warning.GetComponent<SpriteRenderer>();

        float timer = 0f;

        while (timer < warningTime)
        {
            sr.enabled = !sr.enabled;

            // 徐々に点滅を速くする
            float interval = Mathf.Lerp(
                startBlinkInterval,
                endBlinkInterval,
                timer / warningTime
            );

            yield return new WaitForSeconds(interval);

            timer += interval;
        }

        sr.enabled = true;
    }
}