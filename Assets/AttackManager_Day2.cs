using System.Collections;
using UnityEngine;

public class AttackManager_Day2 : MonoBehaviour
{
    [Header("十字攻撃")]
    public GameObject crossAttackPrefab;

    [Header("流星群")]
    public GameObject meteorShowerPrefab;
    public float specialAttackInterval = 20f;
    public float specialAttackDuration = 8f;

    [Header("十字攻撃")]
    public int attackCount = 4;
    public float attackInterval = 0.5f;
    public float restTime = 3f;

    [Header("最初の攻撃まで")]
    public float firstAttackDelay = 2f;

    private Transform player;

    private bool isAttacking = false;
    private bool isSpecialAttacking = false;

    private bool diagonal = false;

    private Coroutine attackCoroutine;
    private Coroutine specialAttackCoroutine;


    private void Start()
    {
        FindPlayer();
    }


    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
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
        {
            Debug.Log("Day2：まだ猫に見つかっていないため攻撃しません。");
            return;
        }

        if (player == null)
            FindPlayer();

        if (player == null)
        {
            Debug.LogError("Day2：Playerが見つかりません！");
            return;
        }

        if (crossAttackPrefab == null)
        {
            Debug.LogError("Day2：Cross Attack Prefabが設定されていません！");
            return;
        }

        if (meteorShowerPrefab == null)
        {
            Debug.LogError("Day2：Meteor Shower Prefabが設定されていません！");
            return;
        }

        isAttacking = true;

        attackCoroutine = StartCoroutine(AttackLoop());
        specialAttackCoroutine = StartCoroutine(SpecialAttackLoop());

        Debug.Log("Day2攻撃開始！");
    }


    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(firstAttackDelay);

        while (isAttacking)
        {
            // 流星群中は待機
            if (isSpecialAttacking)
            {
                yield return null;
                continue;
            }

            // 4連撃
            for (int i = 0; i < attackCount; i++)
            {
                if (!isAttacking)
                    yield break;

                // 流星群が始まったら中断
                if (isSpecialAttacking)
                    break;

                Vector3 targetPosition = player.position;

                // 十字攻撃の予告
                yield return new WaitForSeconds(0.5f);

                // 予告中に流星群が始まったらキャンセル
                if (isSpecialAttacking)
                    break;

                SpawnCrossAttack(targetPosition);

                // 次の攻撃まで
                yield return new WaitForSeconds(attackInterval);
            }

            // 流星群中なら休憩を飛ばして待機
            if (isSpecialAttacking)
                continue;

            // 4連撃後の休憩
            yield return new WaitForSeconds(restTime);
        }
    }


    private IEnumerator SpecialAttackLoop()
    {
        yield return new WaitForSeconds(specialAttackInterval);

        while (isAttacking)
        {
            StartSpecialAttack();

            // 流星群中
            yield return new WaitForSeconds(specialAttackDuration);

            EndSpecialAttack();

            // 次の流星群まで
            yield return new WaitForSeconds(specialAttackInterval);
        }
    }


    private void StartSpecialAttack()
    {
        if (meteorShowerPrefab == null)
            return;

        isSpecialAttacking = true;

        Instantiate(
            meteorShowerPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        Debug.Log("🌠 Day2：流星群開始！");
    }


    private void EndSpecialAttack()
    {
        isSpecialAttacking = false;

        Debug.Log("🌠 Day2：流星群終了！");
    }


    private void SpawnCrossAttack(Vector3 targetPosition)
    {
        if (isSpecialAttacking)
            return;

        if (crossAttackPrefab == null)
            return;

        float angle = diagonal ? 45f : 0f;

        Instantiate(
            crossAttackPrefab,
            new Vector3(
                targetPosition.x,
                targetPosition.y,
                0f
            ),
            Quaternion.Euler(
                0f,
                0f,
                angle
            )
        );

        diagonal = !diagonal;
    }


    public void StopAttacks()
    {
        isAttacking = false;
        isSpecialAttacking = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (specialAttackCoroutine != null)
        {
            StopCoroutine(specialAttackCoroutine);
            specialAttackCoroutine = null;
        }

        Debug.Log("Day2攻撃停止");
    }
}