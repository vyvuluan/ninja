using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.PlayerTag))
        {
            collision.GetComponent<Player>().SavePoint();
        }
    }
}
