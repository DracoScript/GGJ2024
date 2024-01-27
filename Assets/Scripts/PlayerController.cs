using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float accel = 10.0f;
    public float maxSpeed = 10.0f;
    public float jumpForce = 5.0f;

    private Rigidbody2D rb;
    private float horizontal = 0;
    private float vertical = 0;
    private float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

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
}