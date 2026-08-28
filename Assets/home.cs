using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    [Header("次のシーン名")]
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // チーズ3個未満なら進めない
        if (!GameManager.Instance.CanClear())
        {
            Debug.Log("チーズが足りません！");
            return;
        }

        // Title → Map + Day1
        if (SceneManager.GetSceneByName("title").isLoaded)
        {
            SceneLoader.Instance.StartGame(nextSceneName);
            return;
        }

        // Day2 → Clear
        if (SceneManager.GetActiveScene().name == "Day2")
        {
            SceneLoader.Instance.LoadClear();
            return;
        }

        // Day1 → Day2
        SceneLoader.Instance.ChangeStage(nextSceneName);
    }
}