using UnityEngine;

public class AttackArea : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.PlayerTag) || collision.CompareTag(Constants.EnemyTag))
        {
            collision.GetComponent<Character>().OnHit(30f);
        }
    }
}
