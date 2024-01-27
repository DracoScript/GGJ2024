using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    public string inputHorizontal = "Horizontal";
    public string inputVertical = "Vertical";

    [Header("Movement")]
    public float accel = 10.0f;
    public float maxSpeed = 10.0f;
    public float jumpForce = 5.0f;

    [Header("Sprite")]
    public Animator animator;
    public SpriteRenderer sprite;

    [Header("Point System")]
    public int teamNumber = 0;

    [Header("Active")]
    public bool isActive = true;
    public bool isReady = false;

    public string playerName;

    private int id = -1;
    private Rigidbody2D rb;
    private float horizontal = 0;
    private float vertical = 0;
    private float speed = 0;
    private bool onFloor = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (GameController.instance != null)
            GameController.instance.NewPlayer(playerName, this.gameObject);
    }

    void Update()
    {
        // Apply walk
        if (isActive && !isReady)
            rb.velocity = new Vector2(speed, rb.velocity.y);

        // Detect floor
        onFloor = Mathf.Abs(rb.velocity.y) < 0.5;

        // Jump animation
        if (animator)
            animator.SetBool("isJumping", !onFloor);
    }

    void FixedUpdate()
    {
        // Walk right
        if (horizontal > 0)
        {
            speed += accel;
            if (speed > maxSpeed)
                speed = maxSpeed;

            if (sprite)
                sprite.flipX = false;
            if (animator)
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
            if (animator)
                animator.SetBool("isWalking", true);
        }
        // Walk stop
        else
        {
            speed = 0;

            if (animator)
            {
                if (isActive)
                    animator.SetBool("isWalking", false);
                else
                    animator.SetBool("isSleeping", false);
            }
        }

        // Apply jump
        if (vertical > 0 && isActive && !isReady)
            rb.velocity = new Vector2(rb.velocity.x, vertical * jumpForce);
    }

    public void SetReady()
    {
        id = GameState.Instance.playerStats.Count;
        GameState.Instance.playerStats.Add(new PlayerStat(gameObject));
    }

    public void ClearReady()
    {

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            vertical = 1;

        if (context.canceled)
            vertical = 0;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isReady)
                ClearReady();
            else if (animator && isActive)
                animator.SetTrigger("Attack");
        }
    }

    public void OnMove(InputAction.CallbackContext context) => horizontal = context.ReadValue<Vector2>().x;

    public void OnDisconnect()
    {
        Debug.Log(transform.parent.name + " has disconnected");

        if (isReady)
            ClearReady();

        GameState.Instance.playerStats[id].IsConnected = false;
    }

    public void OnReconnect()
    {
        Debug.Log(transform.parent.name + " has reconnected");

        GameState.Instance.playerStats[id].IsConnected = true;
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
            else if (id >= GameState.Instance.playerStats.Count)
                Debug.LogError("Id de player fora do range do GameState.playerStats.");
            else
                GameState.Instance.playerStats[id].Points++;

            yield return new WaitForSeconds(1);
        }
    }
}