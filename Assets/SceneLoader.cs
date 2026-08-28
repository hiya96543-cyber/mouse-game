using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private string currentStage;

    private Canvas fadeCanvas;
    private Image fadeImage;
    private TMP_Text dayText;

    [Header("フェード")]
    public float fadeOutTime = 0.5f;
    public float fadeInTime = 0.5f;

    [Header("DAY表示")]
    public float dayTextSize = 120f;
    public float dayDisplayTime = 1.2f;
    public float dayFadeTime = 0.5f;

    private bool isLoading;

    // ========================================
    // 初期化
    // ========================================
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateUI();
    }

    // ========================================
    // UI作成
    // ========================================
    private void CreateUI()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);

        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);

        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = Color.black;

        SetFullScreen(imageObj);
        SetFade(0);

        GameObject textObj = new GameObject("DayText");
        textObj.transform.SetParent(canvasObj.transform);

        dayText = textObj.AddComponent<TextMeshProUGUI>();
        dayText.fontSize = dayTextSize;
        dayText.alignment = TextAlignmentOptions.Center;
        dayText.color = Color.white;
        dayText.alpha = 0;

        SetFullScreen(textObj);
    }

    private void SetFullScreen(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // ========================================
    // DEBUG
    // Shift + D
    // ========================================
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKeyDown(KeyCode.D))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.DebugSetCheeseMax();
            else
                Debug.LogWarning("【DEBUG】GameManagerが見つかりません！");
        }
    }

    // ========================================
    // フェード
    // ========================================
    private IEnumerator Fade(float from, float to, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            SetFade(Mathf.Lerp(from, to, time / duration));
            yield return null;
        }

        SetFade(to);
    }

    private void SetFade(float alpha)
    {
        if (fadeImage == null) return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    // ========================================
    // DAY表示
    // ========================================
    private IEnumerator ShowDay(string stageName)
    {
        if (dayText == null) yield break;

        switch (stageName)
        {
            case "Day1":
                dayText.text = "Stage1\n2/1";
                break;

            case "Day2":
                dayText.text = "Stage2\n2/2";
                break;

            case "EX":
                dayText.text = "EX\n0000";
                break;

            default:
                dayText.text = stageName;
                break;
        }

        dayText.alpha = 1;

        yield return new WaitForSecondsRealtime(dayDisplayTime);

        float time = 0;

        while (time < dayFadeTime)
        {
            time += Time.unscaledDeltaTime;
            dayText.alpha = 1 - Mathf.Clamp01(time / dayFadeTime);
            yield return null;
        }

        dayText.alpha = 0;
        dayText.text = "";
    }

    // ========================================
    // Title → Map + Day1
    // ========================================
    public void StartGame(string firstStage)
    {
        if (!isLoading)
            StartCoroutine(StartGameCoroutine(firstStage));
    }

    private IEnumerator StartGameCoroutine(string firstStage)
    {
        isLoading = true;

        yield return Fade(0, 1, fadeOutTime);

        currentStage = firstStage;

        yield return SceneManager.LoadSceneAsync("Map");

        yield return SceneManager.LoadSceneAsync(
            currentStage,
            LoadSceneMode.Additive
        );

        SetActiveStage(currentStage);

        Debug.Log("ゲーム開始: " + currentStage);

        yield return ShowDay(currentStage);
        yield return Fade(1, 0, fadeInTime);

        isLoading = false;
    }

    // ========================================
    // Day → 次のDay
    // ========================================
    public void ChangeStage(string nextStage)
    {
        if (!isLoading)
            StartCoroutine(ChangeStageCoroutine(nextStage));
    }

    private IEnumerator ChangeStageCoroutine(string nextStage)
    {
        isLoading = true;

        Debug.Log(currentStage + " → " + nextStage);

        yield return Fade(0, 1, fadeOutTime);

        yield return UnloadScene(currentStage);
        yield return UnloadScene("Map");

        currentStage = nextStage;

        yield return SceneManager.LoadSceneAsync("Map");

        yield return SceneManager.LoadSceneAsync(
            currentStage,
            LoadSceneMode.Additive
        );

        SetActiveStage(currentStage);

        Debug.Log("Map + " + currentStage + " の読み込み完了");

        yield return ShowDay(currentStage);
        yield return Fade(1, 0, fadeInTime);

        isLoading = false;
    }

    // ========================================
    // 死亡時リロード
    // ========================================
    public void ReloadAfterDeath()
    {
        // タイトルではリロードしない
        if (SceneManager.GetActiveScene().name == "title")
        {
            ReloadTitle();
            return;
        }

        if (!isLoading)
            StartCoroutine(ReloadAfterDeathCoroutine());
    }

    private IEnumerator ReloadAfterDeathCoroutine()
    {
        isLoading = true;

        if (string.IsNullOrEmpty(currentStage))
        {
            Debug.LogWarning("現在のDayが設定されていません");
            isLoading = false;
            yield break;
        }

        Debug.Log("死亡。Map + " + currentStage + " をリロードします");

        yield return Fade(0, 1, fadeOutTime);

        yield return UnloadScene(currentStage);
        yield return UnloadScene("Map");

        yield return SceneManager.LoadSceneAsync("Map");

        yield return SceneManager.LoadSceneAsync(
            currentStage,
            LoadSceneMode.Additive
        );

        SetActiveStage(currentStage);

        Debug.Log("Map + " + currentStage + " のリロード完了");

        yield return Fade(1, 0, fadeInTime);

        isLoading = false;
    }

    // ========================================
    // シーン削除
    // ========================================
    private IEnumerator UnloadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid() && scene.isLoaded)
            yield return SceneManager.UnloadSceneAsync(scene);
    }

    // ========================================
    // DayをActive Sceneにする
    // ========================================
    private void SetActiveStage(string stageName)
    {
        Scene stage = SceneManager.GetSceneByName(stageName);

        if (stage.IsValid() && stage.isLoaded)
        {
            SceneManager.SetActiveScene(stage);
        }
        else
        {
            Debug.LogError("Dayシーンが見つかりません: " + stageName);
        }
    }

    // ========================================
    // Day → Clear
    // ========================================
    public void LoadClear()
    {
        if (!isLoading)
            StartCoroutine(LoadClearCoroutine());
    }

    private IEnumerator LoadClearCoroutine()
    {
        isLoading = true;

        Time.timeScale = 1f;

        Debug.Log("Clearシーンへ移動します");

        yield return Fade(0, 1, fadeOutTime);

        yield return SceneManager.LoadSceneAsync(
            "clear",
            LoadSceneMode.Single
        );

        yield return Fade(1, 0, fadeInTime);

        isLoading = false;
    }

    // ========================================
    // Clear → Title
    // ========================================
    public void BackToTitle()
    {
        if (!isLoading)
            StartCoroutine(BackToTitleCoroutine());
    }

    private IEnumerator BackToTitleCoroutine()
    {
        isLoading = true;

        Time.timeScale = 1f;

        Debug.Log("Clear → Title");

        // ★Day情報をリセット
        currentStage = "";

        yield return Fade(0, 1, fadeOutTime);

        yield return SceneManager.LoadSceneAsync(
            "title",
            LoadSceneMode.Single
        );

        yield return Fade(1, 0, fadeInTime);

        isLoading = false;
    }

    // ========================================
    // Title再読み込み
    // ========================================
    public void ReloadTitle()
    {
        if (!isLoading)
            StartCoroutine(ReloadTitleCoroutine());
    }

    private IEnumerator ReloadTitleCoroutine()
    {
        isLoading = true;

        Time.timeScale = 1f;

        Debug.Log("タイトルを再読み込みします");

        // ★Day情報をリセット
        currentStage = "";

        yield return Fade(0, 1, fadeOutTime);

        yield return SceneManager.LoadSceneAsync(
            "title",
            LoadSceneMode.Single
        );

        yield return Fade(1, 0, fadeInTime);

        isLoading = false;
    }

    // ========================================
    // Clear → EX
    // ========================================
    public void LoadEX()
    {
        if (!isLoading)
            StartCoroutine(LoadEXCoroutine());
    }

    private IEnumerator LoadEXCoroutine()
    {
        isLoading = true;

        Time.timeScale = 1f;

        Debug.Log("Clear → EX");

        currentStage = "EX";

        yield return Fade(0, 1, fadeOutTime);

        yield return SceneManager.LoadSceneAsync(
            "EX",
            LoadSceneMode.Single
        );

        yield return Fade(1, 0, fadeInTime);

        isLoading = false;
    }
}