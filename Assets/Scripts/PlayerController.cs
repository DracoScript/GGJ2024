using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            speed = 0;

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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isActive && isReady)
            ClearReady(true);

        if (onFloor && context.performed && isActive && !isReady)
        {
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
        onFloor = true;

        animator.SetBool("isJumping", !onFloor);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != sword && collision.transform.CompareTag("Attack"))
        {
            float direction = transform.position.x - collision.transform.position.x;
            StartCoroutine(DelayStun(direction));
        }
    }

    IEnumerator DelayStun(float direction)
    {
        gettingHit = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(direction, 0.3f) * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.6f);
        gettingHit = false;
    }

    IEnumerator TimeAttack()
    {
        sword.SetActive(true);
        attackAllow = false;
        yield return new WaitForSeconds(0.01f);
        sword.transform.Translate(Vector3.forward * 0.01f);
        yield return new WaitForSeconds(0.3f);
        sword.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        attackAllow = true;
    }
}