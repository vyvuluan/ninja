using UnityEngine;
public class Enemy : Character
{
    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject attackArea;

    private bool isRight = true;
    private IState currentState;
    private Character target;
    public Character Target => target;


    private void Update()
    {
        if (currentState != null && !IsDead)
        {
            currentState.OnExecute(this);
        }
    }
    public override void OnInit()
    {
        base.OnInit();
        ChangeState(new IdleState());
        DeActiveAttack();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
    protected override void OnDeath()
    {
        ChangeState(null);
        base.OnDeath();
    }
    public void ChangeState(IState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState?.OnEnter(this);
    }
    internal void SetTarget(Character character)
    {
        this.target = character;
        if (IsTargetInRange())
        {
            ChangeState(new AttackState());
        }
        else if (target != null)
        {
            ChangeState(new PatrolState());
        }
        else ChangeState(new IdleState());
    }
    public void Moving()
    {
        ChangeAnim(Constants.RunAnim);
        rb.velocity = transform.right * moveSpeed;
    }
    public void StopMoving()
    {
        ChangeAnim(Constants.IdleAnim);
        rb.velocity = Vector2.zero;
    }
    public void Attack()
    {
        ChangeAnim(Constants.AttackAnim);
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.6f);
    }
    public bool IsTargetInRange()
    {
        if (target == null)
        {
            return false;
        }
        return Vector2.Distance(target.transform.position, transform.position) <= attackRange;
    }
    public void ChangeDirection(bool isRight)
    {
        this.isRight = isRight;
        transform.rotation = isRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.EnemyWallTag))
        {
            ChangeDirection(!isRight);
        }
    }
    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }
    private void DeActiveAttack()
    {
        attackArea.SetActive(false);
    }
}
