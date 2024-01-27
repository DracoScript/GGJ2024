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

    private CustomInput customInput;
    private Rigidbody2D rb;
    private float horizontal = 0;
    private float vertical = 0;
    private float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        customInput = new CustomInput();
    }

    // Update is called once per frame
    void Update()
    {
        //horizontal = Input.GetAxisRaw(inputHorizontal);
        horizontal = customInput.Player.Move.ReadValue<Vector2>().x;

        // Apply walk
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    void FixedUpdate()
    {
        // Walk right
        if (horizontal > 0)
        {
            speed += accel;
            if (speed > maxSpeed)
                speed = maxSpeed;
        }
        // Walk left
        else if (horizontal < 0)
        {
            speed -= accel;
            if (speed < -maxSpeed)
                speed = -maxSpeed;
        }
        // Walk stop
        else
        {
            speed = 0;
        }

        // Apply jump
        if (vertical > 0)
            rb.velocity = new Vector2(rb.velocity.x, vertical * jumpForce);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            vertical = 1;

        if (context.canceled)
            vertical = 0;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log(name + ": Attack!");
    }
}