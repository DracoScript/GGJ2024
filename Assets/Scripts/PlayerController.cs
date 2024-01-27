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

    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public int id = -1;

    private Animator animator;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private float horizontal = 0;
    private float speed = 0;
    private bool onFloor = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (GameController.Instance != null)
            GameController.Instance.NewPlayer(playerName, gameObject);
    }

    void Update()
    {
        // Apply walk
        if (isActive && !isReady)
            rb.velocity = new Vector2(speed, rb.velocity.y);

    }

    void FixedUpdate()
    {
        if (isActive && !isReady)
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

                animator.SetBool("isWalking", true);
            }
            // Walk stop
            else
            {
                speed = 0;

                animator.SetBool("isWalking", false);
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

    public void SetReady()
    {
        isReady = true;
        rb.velocity *= 0;
    }

    public void ClearReady()
    {
        isReady = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (onFloor && context.performed && isActive && !isReady)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            onFloor = false;

            animator.SetBool("isJumping", !onFloor);

        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isReady)
                ClearReady();
            else if (isActive)
                animator.SetTrigger("Attack");
        }
    }

    public void OnMove(InputAction.CallbackContext context) => horizontal = context.ReadValue<Vector2>().x;

    public void OnDisconnect()
    {
        Debug.Log(transform.parent.name + " has disconnected");

        if (isReady)
            ClearReady();
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
}