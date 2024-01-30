using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float accel = 10.0f;
    public float maxSpeed = 10.0f;
    public float jumpForce = 5.0f;

    [Header("Active")]
    public bool isActive = true;
    public bool isReady = false;

    [Header("SwordCollider")]
    public GameObject sword;
    public float knockbackForce = 1.0f;
    private float initialPos = 0.5f;
    private bool attackAllow = true;
    private bool gettingHit = false;

    public float deceleration = 0.2f;
    public GameObject collDefault, collSleep;

    [Header("Sounds")]
    public AudioClip AttackClip;
    public AudioClip JumpClip;
    public AudioClip HitClip;
    public AudioClip CoinClip;
    private AudioSource audioSource;

    [Header("Other")]
    public GameObject endCanvas;

    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public int id = -1;
    [HideInInspector]
    public PlayerReadyZone zone;

    private Animator animator;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private float horizontal = 0;
    private float speed = 0;
    private bool onFloor = false;
    private Color originalSpriteColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        originalSpriteColor = sprite.color;

        if (GameController.Instance != null)
            GameController.Instance.NewPlayer(playerName, gameObject);
    }

    void Update()
    {
        // Apply walk
        if (isActive && !isReady)
        {
            if (!gettingHit)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
        }
    }

    void FixedUpdate()
    {
        if (isActive && !isReady)
        {
            if (!gettingHit)
            {
                collDefault.SetActive(true);
                collSleep.SetActive(false);
                animator.SetBool("isSleeping", false);
                rb.gravityScale = 1;

                // Walk right
                if (horizontal > 0)
                {
                    speed += accel;
                    if (speed > maxSpeed)
                        speed = maxSpeed;

                    if (sprite)
                        sprite.flipX = false;

                    sword.transform.localPosition = new Vector2(initialPos, sword.transform.localPosition.y);

                    animator.SetBool("isWalking", true);
                }
                // Walk left
                else if (horizontal < 0)
                {
                    speed -= accel;
                    if (speed < -maxSpeed)
                        speed = -maxSpeed;

                    if (sprite)
                        sprite.flipX = true;

                    sword.transform.localPosition = new Vector2(initialPos * -1, sword.transform.localPosition.y);

                    animator.SetBool("isWalking", true);
                }
                // Walk stop
                else
                {
                    speed = 0;

                    animator.SetBool("isWalking", false);
                }
            }
        }
        else
        {
            if (rb.velocity != Vector2.zero && speed != 0)
            {
                if (rb.velocity.x > 0)
                {
                    rb.velocity = new Vector2(Mathf.Max(0, rb.velocity.x - deceleration), rb.velocity.y);
                }
                else if (rb.velocity.x < 0)
                {
                    rb.velocity = new Vector2(Mathf.Min(0, rb.velocity.x + deceleration), rb.velocity.y);
                }
            }
            else
            {
                speed = 0;

                if (!gettingHit)
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }

            collDefault.SetActive(false);
            collSleep.SetActive(true);
            animator.SetBool("isSleeping", true);
        }
    }

    public void SetReady(PlayerReadyZone z)
    {
        zone = z;
        sprite.color = z.playerColor;
        transform.parent.GetComponent<PairController>()?.CopySpriteColor();

        isReady = true;
        rb.velocity *= 0;

        GameController.Instance.CheckReady();
    }

    public void ClearReady(bool clearColor = false)
    {
        if (clearColor)
        {
            sprite.color = originalSpriteColor;
            transform.parent.GetComponent<PairController>()?.CopySpriteColor();
        }

        isReady = false;
    }

    public void OnStart(InputAction.CallbackContext context)
    {
        if (context.started && playerName == "main") {
            SceneManager.LoadScene(0);
            GameController.Instance.CloseEndCanvas();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isActive && isReady)
            ClearReady(true);

        if (onFloor && context.performed && isActive && !isReady)
        {
            audioSource.PlayOneShot(JumpClip);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            onFloor = false;

            animator.SetBool("isJumping", !onFloor);

        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && isActive && isReady)
            ClearReady(true);

        if (context.started && isActive && !isReady && attackAllow && !gettingHit)
        {
            animator.SetTrigger("Attack");
            StartCoroutine(TimeAttack());
        }
    }

    public void OnMove(InputAction.CallbackContext context) => horizontal = context.ReadValue<Vector2>().x;

    public void OnDisconnect()
    {
        Debug.Log(transform.parent.name + " has disconnected");

        if (isReady)
            ClearReady(true);
    }

    public void OnReconnect()
    {
        Debug.Log(transform.parent.name + " has reconnected");
    }

    public void StartPointIncrease()
    {
        StartCoroutine("PointIncrease");
    }

    public void StopPointIncrease()
    {
        StopCoroutine("PointIncrease");
    }

    private IEnumerator PointIncrease()
    {
        while (true)
        {
            if (id < 0)
                Debug.LogError("Player precisa de um id setado pra incrementar seus pontos.");
            else
                GameController.Instance.points[id]++;

            yield return new WaitForSeconds(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ceiling"))
            return;

        onFloor = true;

        animator.SetBool("isJumping", !onFloor);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != sword && collision.transform.CompareTag("Attack"))
        {
            float direction = transform.position.x - collision.transform.position.x;
            StartCoroutine(DelayStun(direction, collision.gameObject.transform.parent.gameObject));
        }

        if (collision.transform.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(CoinClip);
        }
    }

    IEnumerator DelayStun(float direction, GameObject player)
    {
        gettingHit = true;
        rb.velocity = Vector2.zero;

        if (direction >= 0f)
            direction = 1f;
        else
            direction = -1f;

        audioSource.PlayOneShot(HitClip);
        if (isActive)
        {
            rb.AddForce(new Vector2(direction / 3f, 0.3f) * knockbackForce, ForceMode2D.Impulse);
            this.gameObject.GetComponentInParent<PairController>().ChangeGame();

            GameController.Instance.points[player.GetComponent<PlayerController>().id] += 5;
        }
        else
        {
            rb.AddForce(new Vector2(direction, 1f) * (knockbackForce / 10f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.7f);
        gettingHit = false;
    }

    IEnumerator TimeAttack()
    {
        sword.SetActive(true);
        attackAllow = false;
        audioSource.PlayOneShot(AttackClip);
        yield return new WaitForSeconds(0.01f);
        sword.transform.Translate(Vector3.forward * 0.01f);
        yield return new WaitForSeconds(0.3f);
        sword.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        attackAllow = true;
    }
}