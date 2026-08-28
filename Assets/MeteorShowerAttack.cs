using System.Collections;
using UnityEngine;

public class MeteorShowerAttack : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject meteorPrefab;
    public GameObject warning;

    [Header("流星群")]
    public float duration = 8f;
    public int meteorCount = 3;
    public float meteorInterval = 0.8f;

    [Header("予告")]
    public float warningTime = 0.8f;

    [Header("着弾範囲")]
    public float spawnRangeX = 6f;
    public float spawnRangeY = 4f;

    [Header("隕石")]
    public float fallDistance = 5f;
    public float fallTime = 0.3f;
    public float hitTime = 0.2f;
    public float meteorRotation = -45f;

    private Transform player;


    private void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj == null)
        {
            Debug.LogError("MeteorShowerAttack：Playerが見つかりません！");
            return;
        }

        player = obj.transform;

        if (warning != null)
            warning.SetActive(false);

        StartCoroutine(ShowerCoroutine());
    }


    private IEnumerator ShowerCoroutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            for (int i = 0; i < meteorCount; i++)
                StartCoroutine(SpawnMeteor());

            yield return new WaitForSeconds(meteorInterval);
            timer += meteorInterval;
        }

        // 最後の隕石が終わるまで待つ
        yield return new WaitForSeconds(warningTime + fallTime + hitTime);

        Destroy(gameObject);
    }


    private IEnumerator SpawnMeteor()
    {
        if (meteorPrefab == null || player == null)
            yield break;

        // 着弾地点
        Vector3 target = player.position + new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            0f
        );

        // 予告
        GameObject warningClone = null;

        if (warning != null)
        {
            warningClone = Instantiate(
                warning,
                target,
                Quaternion.identity,
                transform
            );

            warningClone.SetActive(true);
        }

        yield return new WaitForSeconds(warningTime);

        if (warningClone != null)
            Destroy(warningClone);


        // 隕石生成
        Vector3 start = target + new Vector3(
            fallDistance,
            fallDistance,
            0f
        );

        GameObject meteor = Instantiate(
            meteorPrefab,
            start,
            Quaternion.Euler(0f, 0f, meteorRotation)
        );

        yield return StartCoroutine(
            MoveMeteor(meteor, target)
        );
    }


    private IEnumerator MoveMeteor(
        GameObject meteor,
        Vector3 target)
    {
        if (meteor == null)
            yield break;

        Collider2D hitBox = meteor.GetComponent<Collider2D>();

        if (hitBox != null)
            hitBox.enabled = false;

        Vector3 start = meteor.transform.position;
        float timer = 0f;

        // 右上 → 左下
        while (timer < fallTime)
        {
            if (meteor == null)
                yield break;

            timer += Time.deltaTime;

            meteor.transform.position = Vector3.Lerp(
                start,
                target,
                Mathf.Clamp01(timer / fallTime)
            );

            yield return null;
        }

        meteor.transform.position = target;

        // 着弾
        if (hitBox != null)
            hitBox.enabled = true;

        // 着弾音
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMeteor();
        }

        yield return new WaitForSeconds(hitTime);

        if (meteor != null)
            Destroy(meteor);
    }
}