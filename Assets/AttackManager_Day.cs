using System.Collections;
using UnityEngine;

public class AttackManager_Day : MonoBehaviour
{
    [Header("攻撃Prefab")]
    public GameObject pawAttackPrefab;
    public GameObject fishAttackPrefab;

    [Header("爪攻撃")]
    public int pawAttackCount = 4;
    public float pawAttackInterval = 0.5f;
    public float pawRestTime = 3f;

    [Header("魚攻撃")]
    public float fishAttackInterval = 5f;

    [Header("最初の攻撃まで")]
    public float firstAttackDelay = 2f;
    public float firstFishAttackDelay = 4f;

    private Transform player;
    private bool isAttacking = false;


    private void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;
    }


    public void StartAttacks()
    {
        if (isAttacking)
            return;

        if (CatDetectionManager.Instance == null)
        {
            Debug.LogError("CatDetectionManagerが見つかりません！");
            return;
        }

        if (!CatDetectionManager.Instance.IsDetected())
            return;

        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
                player = obj.transform;
        }

        if (player == null)
        {
            Debug.LogError("Playerが見つかりません！");
            return;
        }

        isAttacking = true;

        // 爪攻撃
        StartCoroutine(PawAttackLoop());

        // 魚攻撃
        InvokeRepeating(
            nameof(SpawnFishAttack),
            firstFishAttackDelay,
            fishAttackInterval
        );

        Debug.Log("Day1攻撃開始！");
    }


    public void StopAttacks()
    {
        isAttacking = false;

        StopCoroutine(PawAttackLoop());

        CancelInvoke(nameof(SpawnFishAttack));
    }


    private IEnumerator PawAttackLoop()
    {
        // 最初の攻撃まで待つ
        yield return new WaitForSeconds(firstAttackDelay);

        while (isAttacking)
        {
            // 4連撃
            for (int i = 0; i < pawAttackCount; i++)
            {
                if (!isAttacking)
                    yield break;

                SpawnPawAttack();

                yield return new WaitForSeconds(pawAttackInterval);
            }

            // 休憩
            yield return new WaitForSeconds(pawRestTime);
        }
    }


    private void SpawnPawAttack()
    {
        if (!IsDetected())
            return;

        if (pawAttackPrefab == null)
            return;

        if (player == null)
            return;

        Instantiate(
            pawAttackPrefab,
            new Vector3(
                player.position.x,
                player.position.y,
                0
            ),
            Quaternion.identity
        );
    }


    private void SpawnFishAttack()
    {
        if (!IsDetected())
            return;

        if (fishAttackPrefab == null)
            return;

        if (player == null)
            return;

        GameObject obj = Instantiate(
            fishAttackPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        FishAttack attack = obj.GetComponent<FishAttack>();

        if (attack != null)
            attack.player = player;
    }


    private bool IsDetected()
    {
        return CatDetectionManager.Instance != null &&
               CatDetectionManager.Instance.IsDetected();
    }
}