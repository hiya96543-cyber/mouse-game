using UnityEngine;

public class ClearPoint : MonoBehaviour
{
    public bool isEX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (isEX)
            SceneLoader.Instance.LoadEX();
        else
            SceneLoader.Instance.BackToTitle();
    }
}