using UnityEngine;
public enum StatePlayer
{
    Attack, Throw, Jump, Move, None
}
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    private int coin = 0;
    private float horizontalValue;
    private bool isGrounded = false;
    private bool isAttack = false;
    private bool isDeath = false;
    private string currentAnimName;
    private StatePlayer statePlayer = StatePlayer.None;
    private Vector3 savePoint;
    private void Start()
    {
        savePoint = transform.position;
        OnInit();
    }
    private void Update()
    {
        if (isDeath) return;
        horizontalValue = Input.GetAxis("Horizontal");
        isGrounded = CheckGrounded();
        if (Input.GetButtonDown("Jump"))
        {
            statePlayer = StatePlayer.Jump;
        }
        if (!isAttack)
        {
            if (Input.GetKeyDown(KeyCode.C) && isGrounded)
            {
                Attack();
                statePlayer = StatePlayer.Attack;
                isAttack = true;
            }
            if (Input.GetKeyDown(KeyCode.V) && isGrounded)
            {
                Throw();
                statePlayer = StatePlayer.Throw;
                isAttack = true;
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
        if (isDeath) return;
        if (isGrounded)
        {
            switch (statePlayer)
            {
                case StatePlayer.None:
                    ChangeAnim("idle");
                    rb.velocity = Vector2.zero;
                    break;
                case StatePlayer.Move:
                    Move();
                    break;
                case StatePlayer.Jump:
                    Jump();
                    return;
                default: return;
            }
            if (Mathf.Abs(horizontalValue) > 0.1f)
            {
                statePlayer = StatePlayer.Move;
            }
            else
            {
                statePlayer = StatePlayer.None;
            }
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                ChangeAnim("fall");
                statePlayer = StatePlayer.None;
            }
        }
    }
    public void OnInit()
    {
        isDeath = false;
        isAttack = false;
        transform.position = savePoint;
        ChangeAnim("idle");
    }
    internal void SavePoint()
    {
        savePoint = transform.position;
    }
    private void Jump()
    {
        ChangeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }
    private void Move()
    {
        ChangeAnim("run");
        rb.velocity = new Vector2(horizontalValue * speed, rb.velocity.y);
        transform.rotation = Quaternion.Euler(0, horizontalValue > 0 ? 0 : 180, 0);
    }
    private void Attack()
    {
        ChangeAnim("attack");
        Invoke(nameof(ResetAttack), 0.6f);
    }
    private void Throw()
    {
        ChangeAnim("throw");
        Invoke(nameof(ResetAttack), 0.6f);
    }
    private void ResetAttack()
    {
        statePlayer = StatePlayer.None;
        isAttack = false;
        ChangeAnim("idle");
    }
    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundMask);
        return hit.collider != null;
    }
    private void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            animator.ResetTrigger(animName);
            currentAnimName = animName;
            animator.SetTrigger(currentAnimName);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            coin++;
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("DeathZone"))
        {
            isDeath = true;
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }
    }
}
