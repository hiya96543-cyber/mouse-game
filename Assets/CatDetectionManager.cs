using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CatDetectionManager : MonoBehaviour
{
    public static CatDetectionManager Instance;

    [Header("現在の状態")]
    public bool isDetected = false;

    [Header("画面端")]
    public Image redTop;
    public Image redBottom;
    public Image redLeft;
    public Image redRight;

    [Header("赤い枠の太さ")]
    public float edgeSize = 60f;

    [Header("赤の点滅設定")]
    public float pulseSpeed = 3f;

    [Range(0f, 1f)]
    public float maxAlpha = 0.6f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetupEdges();
        SetRedAlpha(0f);

        // 最初はBGM1
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM1();
        }
    }

    private void Update()
    {
        if (!isDetected)
            return;

        float alpha = Mathf.Lerp(
            0.15f,
            maxAlpha,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f
        );

        SetRedAlpha(alpha);
    }

    // ========================================
    // 猫に見つかった
    // ========================================

    public void Detected()
    {
        if (isDetected)
            return;

        isDetected = true;

        Debug.Log("猫に見つかった！");

        // BGM2に変更
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM2();
        }

        StartCurrentDayAttack();
    }

    // ========================================
    // 現在のDayの攻撃を開始
    // ========================================

    private void StartCurrentDayAttack()
    {
        string currentDay = SceneManager.GetActiveScene().name;

        if (currentDay == "Day1")
        {
            AttackManager_Day attackManager =
                FindFirstObjectByType<AttackManager_Day>();

            if (attackManager != null)
            {
                attackManager.StartAttacks();
                Debug.Log("Day1攻撃開始！");
            }
            else
            {
                Debug.LogError("AttackManager_Dayが見つかりません！");
            }
        }
        else if (currentDay == "Day2")
        {
            AttackManager_Day2 attackManager =
                FindFirstObjectByType<AttackManager_Day2>();

            if (attackManager != null)
            {
                attackManager.StartAttacks();
                Debug.Log("Day2攻撃開始！");
            }
            else
            {
                Debug.LogError("AttackManager_Day2が見つかりません！");
            }
        }
        else
        {
            Debug.Log("このDayには攻撃設定がありません。");
        }
    }

    // ========================================
    // 猫発見状態
    // ========================================

    public void Undetected()
    {
        isDetected = false;

        SetRedAlpha(0f);

        // BGM1に戻す
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM1();
        }

        Debug.Log("猫に見つかっていない");
    }

    public bool IsDetected()
    {
        return isDetected;
    }

    // ========================================
    // 画面端設定
    // ========================================

    private void SetupEdges()
    {
        SetupTop(redTop);
        SetupBottom(redBottom);
        SetupLeft(redLeft);
        SetupRight(redRight);
    }

    private void SetupTop(Image image)
    {
        if (image == null) return;

        RectTransform rect = image.rectTransform;

        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = new Vector2(0f, -edgeSize);
        rect.offsetMax = new Vector2(0f, 0f);
    }

    private void SetupBottom(Image image)
    {
        if (image == null) return;

        RectTransform rect = image.rectTransform;

        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.offsetMin = new Vector2(0f, 0f);
        rect.offsetMax = new Vector2(0f, edgeSize);
    }

    private void SetupLeft(Image image)
    {
        if (image == null) return;

        RectTransform rect = image.rectTransform;

        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.offsetMin = new Vector2(0f, 0f);
        rect.offsetMax = new Vector2(edgeSize, 0f);
    }

    private void SetupRight(Image image)
    {
        if (image == null) return;

        RectTransform rect = image.rectTransform;

        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = new Vector2(-edgeSize, 0f);
        rect.offsetMax = new Vector2(0f, 0f);
    }

    private void SetRedAlpha(float alpha)
    {
        SetAlpha(redTop, alpha);
        SetAlpha(redBottom, alpha);
        SetAlpha(redLeft, alpha);
        SetAlpha(redRight, alpha);
    }

    private void SetAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}