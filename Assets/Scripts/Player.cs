using UnityEngine;
public enum StatePlayer
{
    Attack, Throw, Jump, Move, None
}
public class Player : Character
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform kunaiPoint;
    [SerializeField] private GameObject attackArea;
    private int coin;
    private float horizontalValue;
    private bool isGrounded = false;
    private bool isAttack = false;
    private bool isJump = false;
    private StatePlayer statePlayer = StatePlayer.None;
    private Vector3 savePoint;
    private void Awake()
    {
        coin = PlayerPrefs.GetInt(Constants.CoinPlayerPrefs, 0);
    }
    private void Update()
    {
        Debug.Log(statePlayer);
        if (IsDead) return;
        horizontalValue = Input.GetAxis("Horizontal");
        isGrounded = CheckGrounded();
        if (Input.GetButtonDown("Jump"))
        {
            statePlayer = StatePlayer.Jump;
        }
        if (!isAttack)
        {
            if (Input.GetKeyDown(KeyCode.C) && isGrounded && !isJump)
            {
                Attack();
                statePlayer = StatePlayer.Attack;
            }
            if (Input.GetKeyDown(KeyCode.V) && isGrounded && !isJump)
            {
                Throw();
                statePlayer = StatePlayer.Throw;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            return;
        }

    }
    private void FixedUpdate()
    {
        if (IsDead) return;
        if (isGrounded)
        {

            switch (statePlayer)
            {
                case StatePlayer.None:
                    ChangeAnim(Constants.IdleAnim);
                    rb.velocity = Vector2.zero;
                    break;
                case StatePlayer.Move:
                    //Move();
                    break;
                case StatePlayer.Jump:
                    Jump();
                    break;
                default: return;
            }
            if (Mathf.Abs(horizontalValue) > 0.1f)
            {
                ChangeAnim(Constants.RunAnim);
            }

        }
        else
        {
            if (rb.velocity.y < 0)
            {
                ChangeAnim(Constants.FallAnim);
                statePlayer = StatePlayer.None;
                isJump = false;
            }
        }
        if (Mathf.Abs(horizontalValue) > 0.1f)
        {
            statePlayer = StatePlayer.Move;
            Move();
        }
        else
        {
            statePlayer = StatePlayer.None;
        }

    }
    public override void OnInit()
    {

        base.OnInit();
        isAttack = false;
        transform.position = savePoint;
        ChangeAnim(Constants.IdleAnim);
        DeActiveAttack();
        SavePoint();
        UIManager.Instance.SetCoin(coin);
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
    }
    internal void SavePoint()
    {
        savePoint = transform.position;
    }
    public void Jump()
    {
        ChangeAnim(Constants.JumpAnim);
        rb.AddForce(jumpForce * Vector2.up);
        isJump = true;
    }
    private void Move()
    {
        if (isGrounded && !isJump)
        {
            ChangeAnim(Constants.RunAnim);
        }
        rb.velocity = new Vector2(horizontalValue * speed, rb.velocity.y);
        transform.rotation = Quaternion.Euler(0, horizontalValue >= 0 ? 0 : 180, 0);
    }
    public void Attack()
    {
        isAttack = true;
        ChangeAnim(Constants.AttackAnim);
        Invoke(nameof(ResetAttack), 0.6f);
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.6f);
    }
    public void Throw()
    {
        isAttack = true;
        ChangeAnim(Constants.ThrowAnim);
        Invoke(nameof(ResetAttack), 0.6f);
        Instantiate(kunaiPrefab.gameObject, kunaiPoint.position, kunaiPoint.rotation);
    }
    private void ResetAttack()
    {
        statePlayer = StatePlayer.None;
        isAttack = false;
        ChangeAnim(Constants.IdleAnim);
    }
    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundMask);
        return hit.collider != null;
    }
    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }
    private void DeActiveAttack()
    {
        attackArea.SetActive(false);
    }
    public void SetMove(float horizontal)
    {
        this.horizontalValue = horizontal;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.CoinTag))
        {
            coin++;
            PlayerPrefs.SetInt(Constants.CoinPlayerPrefs, coin);
            UIManager.Instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag(Constants.DeathZoneTag))
        {
            OnHit(100f);
            ChangeAnim(Constants.DeathAnim);
            Invoke(nameof(OnInit), 1f);
        }
    }
}
