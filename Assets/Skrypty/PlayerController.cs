using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Ustawienia Ruchu i Skoku")]
    public float speed = 10f;
    public float jumpForce = 15f;
    public float wallJumpForceX = 14f;  // Zwiekszona sila X dla lepszego wybicia
    public float wallJumpForceY = 16f;  // Zwiekszona sila Y
    public float wallJumpMomentumDuration = 0.3f; // Dluższy czas blokowania inputu po odbiciu

    [Header("Wykrywanie Ziemi/Scian")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask collisionLayer;
    public float checkRadius = 0.2f;        // Promien OverlapCircle

    [Header("Mechanika Gry")]
    public float deathMaxYDrop = 3f;

    // STAN PRYWATNY
    private Rigidbody2D rb;
    private float horizontalInput;

    private bool isGrounded;
    private bool isWalled;

    // FLAGA KONTROLUJĄCA SKOK: Możesz skoczyć TYLKO, gdy ta flaga jest true.
    private bool jumpAvailable;

    private bool isWallJumping;
    private float maxHeightY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
        maxHeightY = transform.position.y;
        jumpAvailable = true; // Zaczynamy z możliwością skoku
    }

    void Update()
    {
        // 1. GAME OVER
        if (transform.position.y > maxHeightY)
        {
            maxHeightY = transform.position.y;
        }
        if (transform.position.y < maxHeightY - deathMaxYDrop)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // 2. CZYTANIE INPUTU
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 3. SPRAWDZANIE KOLIZJI
        // Najwazniejsze: Sprawdzamy, czy dotykamy ziemi
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, collisionLayer);
        isWalled = Physics2D.OverlapCircle(wallCheck.position, checkRadius, collisionLayer);

        // ODCZEPIENIE ZIEMIA: Jeśli dotknęliśmy ziemi, odzyskujemy możliwość skoku
        if (isGrounded)
        {
            jumpAvailable = true;
            isWallJumping = false; // Koniec blokady Wall Jumpa
        }

        // 4. SKOK / WALL JUMP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // SKOK Z ZIEMI: Wymaga, by byc uziemionym i miec dostępny skok
            if (isGrounded && jumpAvailable)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpAvailable = false; // BLOKADA SPAMOWANIA: Zabraniamy kolejnego skoku
            }
            // ODBICIE OD ŚCIANY: Wymaga, byc przy scianie, ale NIE na ziemi
            else if (isWalled && !isGrounded)
            {
                // Wektor odbicia jest odwracany (odpycha od sciany)
                rb.linearVelocity = new Vector2(
                    wallJumpForceX * (transform.localScale.x * -1), // Siła X odpycha w przeciwną stronę
                    wallJumpForceY
                );

                // Wlaczamy blokade, ktora jest KLUCZEM do braku przyklejania
                isWallJumping = true;
                Invoke("StopWallJumpMomentum", wallJumpMomentumDuration);
            }
        }

        // 5. Obracanie (jestesmy w Update, bo jest wizualne)
        if (!isWallJumping)
        {
            if (horizontalInput > 0 && transform.localScale.x < 0) Flip();
            else if (horizontalInput < 0 && transform.localScale.x > 0) Flip();
        }
    }

    void FixedUpdate()
    {
        // 6. FIZYKA RUCHU (Usuwamy problem przyklejania i zachowujemy pęd)

        // Ruch poziomy jest możliwy TYLKO WTEDY, gdy NIE jesteśmy w trakcie Wall Jumpa (isWallJumping = false)
        if (!isWallJumping)
        {
            // Zachowujemy pęd Y, zmieniając tylko X
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        }
    }

    void StopWallJumpMomentum()
    {
        // Ta funkcja jest wywoływana po upływie timera i pozwala na przejęcie inputu
        isWallJumping = false;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}