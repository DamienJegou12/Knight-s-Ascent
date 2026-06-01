using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    [SerializeField]
    public float jumpSpeed = 7f;
    [SerializeField]
    public float climbSpeed = 10f;
    [SerializeField]
    public bool isAlive = true;
    [SerializeField]
    public float deathKick = 5f;
    [SerializeField]
    public CinemachineStateDrivenCamera stateDrivenCamera;
    private Vector2 moveInput;
    private Rigidbody2D myRigidbody;
    private Animator myAnimator;
    private BoxCollider2D myFeetCollider;
    private CapsuleCollider2D myBodyCollider;
    private bool isClimbing = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { 
            myRigidbody.linearVelocity = Vector2.zero;
            return;
        }
        Run();
        FlipSprite();
        ClimbLadder();
        EnnemiCollision();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        // Debug.Log("Move Input: " + moveInput);
    }

    void Run()
    {
        if(Mathf.Abs(moveInput.x) < Mathf.Epsilon)
        {
            myRigidbody.linearVelocity = new Vector2(0, myRigidbody.linearVelocity.y);
            myAnimator.SetBool("isRunning", false);
            return;
        }
        myAnimator.SetBool("isRunning", true);
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidbody.linearVelocity.y); 
        myRigidbody.linearVelocity = playerVelocity;
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocity.x) * 5f, 5f);
        }
    }

    void OnJump(InputValue value)
    {
        if (!value.isPressed) { return; }
        Debug.Log("Jump Input: " + value);
        if (isTouchingTheGround())
        {
            myRigidbody.linearVelocity += new Vector2(0, jumpSpeed);
            Debug.Log("Jumping with velocity: " + myRigidbody.linearVelocity);
        }
    }

    bool isTouchingTheGround(){
        return myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Platforms"));
    }

    bool isTouchingLadder(){
        return myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladders"));
    }

    void ClimbLadder()
    {
        if (!isTouchingLadder()) {
            if(isClimbing)
            {
                if (moveInput.y > 0) 
                {
                    // Ajuste cette valeur (ex: 1f ou 2f) selon la hauteur de ta marche
                    myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, 2f); 
                }
                else
                {
                    myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, 0f);
                }
                isClimbing = false;
                // myAnimator.SetBool("isClimbing", false);
            }
            myRigidbody.gravityScale = 1.5f;
            return;
        }
        float climbSpeed = moveInput.y * this.climbSpeed;
        myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, climbSpeed);
        myRigidbody.gravityScale = 0f;
        isClimbing = Mathf.Abs(climbSpeed) > Mathf.Epsilon;
        // myAnimator.SetBool("isClimbing", Mathf.Abs(climbSpeed) > Mathf.Epsilon);
    }

    void EnnemiCollision()
    {
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ennemi")))
        {
            return;
        }
        Die();
    }
    public void Die()
    {
        isAlive = false;
        myAnimator.SetTrigger("Dying");
        myRigidbody.linearVelocity = new Vector2(0f, deathKick);
        myBodyCollider.enabled = false;
        myFeetCollider.enabled = false;
        stateDrivenCamera.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            Debug.Log("Entered water trigger " + other.gameObject.name);
            Die();
        }
        // Debug.Log("Entered water trigger " + other.gameObject.name);
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Spikes")) && Mathf.Abs(myRigidbody.linearVelocity.y) > 0.1f)
        {
            Debug.Log("Entered spikes trigger " + other.gameObject.name + " with velocity " + myRigidbody.linearVelocity);
            Die();
        }
    }

}
