using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("チーズ設定")]
    public int targetCheese = 3;

    private int currentCheese = 0;

    [Header("UI")]
    public TMP_Text cheeseText;

    private bool isGameOver = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateCheeseUI();
    }


    // ========================================
    // チーズ追加
    // ========================================
    public void AddCheese(int amount = 1)
    {
        currentCheese += amount;

        if (currentCheese > targetCheese)
            currentCheese = targetCheese;

        UpdateCheeseUI();

        Debug.Log(
            "チーズ取得 : " +
            currentCheese +
            " / " +
            targetCheese
        );
    }


    // ========================================
    // チーズリセット
    // ========================================
    public void ResetCheese()
    {
        currentCheese = 0;
        isGameOver = false;

        UpdateCheeseUI();
    }


    // ========================================
    // クリア判定
    // ========================================
    public bool CanClear()
    {
        return currentCheese >= targetCheese;
    }


    // ========================================
    // 現在のチーズ数取得
    // ========================================
    public int GetCheese()
    {
        return currentCheese;
    }


    // ========================================
    // UI更新
    // ========================================
    private void UpdateCheeseUI()
    {
        if (cheeseText != null)
        {
            cheeseText.text =
                currentCheese +
                " / " +
                targetCheese;
        }
    }


    // ========================================
    // デバッグ
    // Shift + D
    // ========================================
    public void DebugSetCheeseMax()
    {
        currentCheese = targetCheese;

        UpdateCheeseUI();

        Debug.Log("【DEBUG】チーズを最大にしました！");
    }


    // ========================================
    // ゲームオーバー
    // ========================================
    public void GameOver()
    {
        // 二重実行防止
        if (isGameOver)
            return;

        isGameOver = true;

        Debug.Log("ゲームオーバー");

        // ゲームオーバー音
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGameOver();
        }

        // ステージをリロード
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.ReloadAfterDeath();
        }
        else
        {
            Debug.LogError("SceneLoaderが見つかりません！");
        }
    }


    // ========================================
    // ゲームオーバー状態
    // ========================================
    public bool IsGameOver()
    {
        return isGameOver;
    }
}