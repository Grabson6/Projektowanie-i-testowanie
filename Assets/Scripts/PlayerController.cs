using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public PlayerStats stats;

    public Transform groundCheck;
    public Transform leftWallCheck;
    public Transform rightWallCheck;

    public LayerMask whatIsGround;
    public float fallLimit = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float moveX;

    private bool isGrounded;
    private bool isTouchingLeft;
    private bool isTouchingRight;
    private bool isWalled;

    private bool canJump;
    private bool isWallJumping;

    private float jumpCooldown = 0.2f;
    private float nextJumpTime = 0f;

    private float maxY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 3f;

        transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
        maxY = transform.position.y;
    }

    void Update()
    {
        if (transform.position.y > maxY) maxY = transform.position.y;
        if (transform.position.y < maxY - fallLimit) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        moveX = Input.GetAxisRaw("Horizontal");

        if (Time.time < nextJumpTime) isGrounded = false;
        else isGrounded = Physics2D.OverlapCircle(groundCheck.position, stats.checkRadius, whatIsGround);

        isTouchingLeft = Physics2D.OverlapCircle(leftWallCheck.position, stats.checkRadius, whatIsGround);
        isTouchingRight = Physics2D.OverlapCircle(rightWallCheck.position, stats.checkRadius, whatIsGround);
        isWalled = (isTouchingLeft || isTouchingRight);

        if (isGrounded)
        {
            canJump = true;
            isWallJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && canJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, stats.jumpPower);
                nextJumpTime = Time.time + jumpCooldown;
            }
            else if (isWalled && !isGrounded)
            {
                float wallDir = isTouchingLeft ? 1f : -1f;

                rb.linearVelocity = new Vector2(stats.wallJumpX * wallDir, stats.wallJumpY);

                isWallJumping = true;
                Invoke("ResetWallJump", stats.wallJumpTime);
                nextJumpTime = Time.time + jumpCooldown;

                if (wallDir == 1) sr.flipX = false;
                else sr.flipX = true;
            }
        }

        if (!isWallJumping)
        {
            if (moveX > 0) sr.flipX = false;
            else if (moveX < 0) sr.flipX = true;
        }
    }

    void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(moveX * stats.runSpeed, rb.linearVelocity.y);
        }
    }

    void ResetWallJump()
    {
        isWallJumping = false;
    }

    void OnDrawGizmos()
    {
        if (stats == null) return;

        Gizmos.color = Color.red;
        if (groundCheck) Gizmos.DrawWireSphere(groundCheck.position, stats.checkRadius);

        Gizmos.color = Color.blue;
        if (leftWallCheck) Gizmos.DrawWireSphere(leftWallCheck.position, stats.checkRadius);

        Gizmos.color = Color.yellow;
        if (rightWallCheck) Gizmos.DrawWireSphere(rightWallCheck.position, stats.checkRadius);
    }
}