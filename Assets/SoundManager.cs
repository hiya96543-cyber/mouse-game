using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource;

    public AudioClip bgm1;
    public AudioClip bgm2;
    public AudioClip bgm3;
    public AudioClip bgm4;

    [Header("BGM音量")]
    [Range(0f, 1f)] public float bgm1Volume = 1f;
    [Range(0f, 1f)] public float bgm2Volume = 1f;
    [Range(0f, 1f)] public float bgm3Volume = 1f;
    [Range(0f, 1f)] public float bgm4Volume = 1f;

    [Header("SE")]
    public AudioSource seSource;

    [Header("効果音")]
    public AudioClip cheese;
    public AudioClip attackWarning;
    public AudioClip pawAttack;
    public AudioClip fishAttack;
    public AudioClip crossAttack;
    public AudioClip meteor;
    public AudioClip gameOver;
    public AudioClip clear;

    [Header("効果音 音量")]
    [Range(0f, 1f)] public float cheeseVolume = 1f;
    [Range(0f, 1f)] public float attackWarningVolume = 1f;
    [Range(0f, 1f)] public float pawAttackVolume = 1f;
    [Range(0f, 1f)] public float fishAttackVolume = 1f;
    [Range(0f, 1f)] public float crossAttackVolume = 1f;
    [Range(0f, 1f)] public float meteorVolume = 1f;
    [Range(0f, 1f)] public float gameOverVolume = 1f;
    [Range(0f, 1f)] public float clearVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadBGM(bgm1);
            LoadBGM(bgm2);
            LoadBGM(bgm3);
            LoadBGM(bgm4);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Clear画面
        if (sceneName.ToLower() == "clear")
        {
            PlayBGM3();
        }

        // EXステージ
        if (sceneName.ToLower() == "ex")
        {
            PlayBGM4();
        }
    }

    // -------------------------
    // BGM
    // -------------------------

    private void LoadBGM(AudioClip clip)
    {
        if (clip != null && !clip.preloadAudioData)
            clip.LoadAudioData();
    }

    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (clip == null || bgmSource == null)
            return;

        if (!clip.isReadyToPlay)
            clip.LoadAudioData();

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayBGM1()
    {
        PlayBGM(bgm1, bgm1Volume);
    }

    public void PlayBGM2()
    {
        PlayBGM(bgm2, bgm2Volume);
    }

    public void PlayBGM3()
    {
        PlayBGM(bgm3, bgm3Volume);
    }

    public void PlayBGM4()
    {
        PlayBGM(bgm4, bgm4Volume);
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    // -------------------------
    // SE
    // -------------------------

    public void PlaySE(AudioClip clip, float volume = 1f)
    {
        if (clip != null && seSource != null)
            seSource.PlayOneShot(clip, volume);
    }

    public void PlayCheese()
    {
        PlaySE(cheese, cheeseVolume);
    }

    public void PlayAttackWarning()
    {
        PlaySE(attackWarning, attackWarningVolume);
    }

    public void PlayPawAttack()
    {
        PlaySE(pawAttack, pawAttackVolume);
    }

    public void PlayFishAttack()
    {
        PlaySE(fishAttack, fishAttackVolume);
    }

    public void PlayCrossAttack()
    {
        PlaySE(crossAttack, crossAttackVolume);
    }

    public void PlayMeteor()
    {
        PlaySE(meteor, meteorVolume);
    }

    public void PlayGameOver()
    {
        PlaySE(gameOver, gameOverVolume);
    }

    public void PlayClear()
    {
        PlaySE(clear, clearVolume);
    }
}