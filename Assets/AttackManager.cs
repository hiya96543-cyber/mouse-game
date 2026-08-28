using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public GameObject pawAttackPrefab;
    public Transform player;

    public float attackInterval = 3f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPawAttack), 2f, attackInterval);
    }

    void SpawnPawAttack()
    {
        Vector3 pos = player.position;

        Instantiate(
            pawAttackPrefab,
            new Vector3(pos.x, pos.y, 0),
            Quaternion.identity
        );
    }
}