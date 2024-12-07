using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    //Accesible by Inspector
    public float movementSpeed = 5.0f;

    //Jumping Floats
    public float jumpForce = 8.0f;

    public float runSpeed = 10.0f;  // Maximum running speed
    public float acceleration = 2.0f;  // Acceleration rate
    public float deceleration = 2.0f;  // Deceleration rate

    private float currentSpeed;

    public float fallMultiplier;
    public float lowJumpMultiplier;

    public int availableJumps = 1;
    private int availableJumpsLeft;

    private bool canJump;

    //Wall Jump
    public bool canWallJump = true;
    public float wallJumpTime;

    bool isTouchingWall;
    bool wallHold;
    bool wallJumping;

    public Transform wallCheck;


    private float wallJumpBufferTime = 0.2f;
    private float wallJumpBufferCounter;


    //Not Accesible by Inspector
    private float InputDirection;

    //Related to running and flipping
    private bool isRunning;
    private bool isFacingRight = true;

    //GroundCheck / Bool animator
    public float groundCheckCircle;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;

    // Sound Effects
    public AudioClip walkSound;
    public AudioClip spinSound;
    public AudioClip landingSound;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        availableJumpsLeft = availableJumps;
        currentSpeed = movementSpeed; 
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckifCanJump();

        //WallJump

        if (canWallJump)
{
    isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, groundCheckCircle, whatIsGround);
    wallHold = isTouchingWall && !isGrounded && InputDirection != 0;

if (Input.GetButtonDown("Jump") && isTouchingWall)
{
    if (InputDirection != 0) // Ensure the player presses a direction away from the wall
    {
        wallJumping = true;
        rb.velocity = new Vector2(-InputDirection * movementSpeed * 1.5f, jumpForce);
        Invoke(nameof(ResetWallJumping), wallJumpTime);
        rb.velocity = new Vector2(-InputDirection * movementSpeed * 2.0f, jumpForce);
    }
}

if (isTouchingWall)
{
    wallJumpBufferCounter = wallJumpBufferTime;
}
else
{
    wallJumpBufferCounter -= Time.deltaTime;
}

if (Input.GetButtonDown("Jump") && wallJumpBufferCounter > 0)
{
    PerformWallJump();
}

if (Input.GetButtonDown("Jump") && isTouchingWall && InputDirection != 0 && Mathf.Sign(InputDirection) != Mathf.Sign(transform.localScale.x))
{
    PerformWallJump();
}
}



        //Variable Jump Height
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //Run
        if (Input.GetButton("Run"))
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, movementSpeed, deceleration * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckEnvironment();
    }

    private void CheckInput()
    {
        InputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

private void Jump() 
{ 
    if (canJump)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        availableJumpsLeft--;
        audioSource.PlayOneShot(spinSound);
    }
}


    private void CheckifCanJump()
    {
        if(isGrounded && rb.velocity.y <= 3)
        {
            availableJumpsLeft = availableJumps;
        }
        if(availableJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true; 
        }
    }

    private void PerformWallJump()
{
    wallJumping = true;
    rb.velocity = new Vector2(-InputDirection * movementSpeed * 1.5f, jumpForce);
    Invoke(nameof(ResetWallJumping), wallJumpTime);
}

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(currentSpeed * InputDirection, rb.velocity.y);
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && InputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && InputDirection > 0)
        {
            Flip();
        }

        isRunning = Mathf.Abs(rb.velocity.x) > 0.5f;  // Check if the player is moving
    }

private void UpdateAnimation()
{
    animator.SetBool("isRunning", isRunning);
    animator.SetBool("isGrounded", isGrounded);
    animator.SetBool("wallHold", wallHold);
    animator.SetFloat("yVelocity", rb.velocity.y);

    if (isRunning && isGrounded)
    {
        if (audioSource.clip != walkSound || !audioSource.isPlaying)
        {
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    else
    {
        if (audioSource.clip == walkSound && audioSource.isPlaying)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }
}


private void Flip()
{
    isFacingRight = !isFacingRight;

    // Trigger the turn animation
    animator.SetTrigger("isTurning");

    StartCoroutine(CompleteTurnAnimation());
}

private IEnumerator CompleteTurnAnimation()
{
    yield return new WaitForSeconds(0.2f);
    transform.Rotate(0.0f, 180.0f, 0.0f);

    animator.ResetTrigger("isTurning");
}


private void CheckEnvironment()
{
    bool wasGrounded = isGrounded; 
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckCircle, whatIsGround);

    if (!wasGrounded && isGrounded)
    {
        audioSource.PlayOneShot(landingSound);

        
        if (audioSource.clip == spinSound)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }
}




    //Wall Jump
private void ResetWallJumping()
{
    wallJumping = false;
}



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckCircle);
        Gizmos.DrawWireSphere(wallCheck.position, groundCheckCircle);
    }
}
