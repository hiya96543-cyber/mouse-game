using System.Collections;
using UnityEngine;

public class FishAttack : MonoBehaviour
{
    [Header("プレイヤー")]
    public Transform player;

    [Header("魚Prefab")]
    public GameObject fishPrefab;

    [Header("攻撃設定")]
    public float warningTime = 2f;
    public float targetDistance = 3f;
    public float spawnDistance = 10f;
    public float continueDistance = 10f;
    public float fishSpeed = 15f;

    private GameObject[] fishes = new GameObject[3];

    private Vector2[] directions =
    {
        Vector2.up,
        new Vector2(1, 1).normalized,
        Vector2.right,
        new Vector2(1, -1).normalized,
        Vector2.down,
        new Vector2(-1, -1).normalized,
        Vector2.left,
        new Vector2(-1, 1).normalized
    };

    private void Start()
    {
        if (player == null)
        {
            GameObject obj =
                GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
                player = obj.transform;
        }

        if (player == null || fishPrefab == null)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        Vector3 playerPos = player.position;

        int[] selected = new int[3];

        for (int i = 0; i < 3; i++)
        {
            do
            {
                selected[i] = Random.Range(0, 8);
            }
            while (
                (i > 0 && selected[i] == selected[0]) ||
                (i > 1 && selected[i] == selected[1])
            );
        }

        // 魚を3匹生成
        for (int i = 0; i < 3; i++)
        {
            Vector2 targetDir =
                directions[selected[i]];

            Vector3 targetPoint =
                playerPos +
                (Vector3)(targetDir * targetDistance);

            Vector2 attackDir =
                new Vector2(
                    -targetDir.y,
                    targetDir.x
                ).normalized;

            Vector3 spawnPos =
                targetPoint +
                (Vector3)(attackDir * spawnDistance);

            GameObject fish =
                Instantiate(
                    fishPrefab,
                    spawnPos,
                    Quaternion.identity
                );

            fishes[i] = fish;

            // 魚の向き
            float angle =
                Mathf.Atan2(
                    -attackDir.y,
                    -attackDir.x
                ) * Mathf.Rad2Deg;

            fish.transform.rotation =
                Quaternion.Euler(
                    0,
                    0,
                    angle + 180
                );

            // 予告中は当たり判定OFF
            Collider2D col =
                fish.GetComponent<Collider2D>();

            if (col != null)
                col.enabled = false;
        }

        // 予告
        yield return new WaitForSeconds(warningTime);

        // 攻撃音
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(
                SoundManager.Instance.fishAttack
            );
        }

        // 攻撃開始
        for (int i = 0; i < 3; i++)
        {
            if (fishes[i] == null)
                continue;

            Collider2D col =
                fishes[i].GetComponent<Collider2D>();

            if (col != null)
                col.enabled = true;

            Vector2 targetDir =
                directions[selected[i]];

            Vector3 targetPoint =
                playerPos +
                (Vector3)(targetDir * targetDistance);

            Vector2 attackDir =
                new Vector2(
                    -targetDir.y,
                    targetDir.x
                ).normalized;

            StartCoroutine(
                MoveFish(
                    fishes[i],
                    targetPoint,
                    attackDir
                )
            );
        }
    }

    private IEnumerator MoveFish(
        GameObject fish,
        Vector3 targetPoint,
        Vector2 attackDir)
    {
        Vector3 endPoint =
            targetPoint -
            (Vector3)(
                attackDir *
                continueDistance
            );

        while (fish != null)
        {
            fish.transform.position =
                Vector3.MoveTowards(
                    fish.transform.position,
                    endPoint,
                    fishSpeed *
                    Time.deltaTime
                );

            if (
                Vector3.Distance(
                    fish.transform.position,
                    endPoint
                ) < 0.05f
            )
            {
                Destroy(fish);
                CheckFinished();
                yield break;
            }

            yield return null;
        }
    }

    private void CheckFinished()
    {
        foreach (GameObject fish in fishes)
        {
            if (fish != null)
                return;
        }

        Destroy(gameObject);
    }
}