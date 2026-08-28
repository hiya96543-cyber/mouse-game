using UnityEngine;

public class CatBGMManager : MonoBehaviour
{
    private bool lastDetected = false;

    private void Start()
    {
        // 最初はBGM1
        SoundManager.Instance.PlayBGM1();
    }

    private void Update()
    {
        if (CatDetectionManager.Instance == null)
            return;

        bool detected =
            CatDetectionManager.Instance.IsDetected();

        // 状態が変わったときだけBGMを変更
        if (detected != lastDetected)
        {
            lastDetected = detected;

            if (detected)
            {
                // 猫に見つかった
                SoundManager.Instance.PlayBGM2();
            }
            else
            {
                // 猫に見つかっていない
                SoundManager.Instance.PlayBGM1();
            }
        }
    }
}