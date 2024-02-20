using Unity.Mathematics;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected CombatText combatTextPrefab;
    [SerializeField] private GameEvent gameEvent;

    private float hp;
    private string currentAnimName;
    public bool IsDead => hp <= 0;

    public virtual void OnInit()
    {
        hp = 100;
        healthBar.OnInit(100, transform);
    }
    private void Start()
    {
        OnInit();
    }
    public virtual void OnDespawn()
    {

    }
    protected virtual void OnDeath()
    {
        ChangeAnim(Constants.DeathAnim);
        gameEvent.TriggerEvent();
        Invoke(nameof(OnDespawn), 2f);
    }
    public void OnHit(float damage)
    {
        if (!IsDead)
        {
            hp -= damage;
            if (IsDead)
            {
                hp = 0;
                OnDeath();
            }
            healthBar.SetNewHp(hp);
            Instantiate(combatTextPrefab, transform.position + Vector3.up, quaternion.identity).OnInit(damage);
        }
    }
    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            animator.ResetTrigger(animName);
            currentAnimName = animName;
            animator.SetTrigger(currentAnimName);
        }
    }

}
