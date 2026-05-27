using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    [SerializeField]
    public float jumpSpeed = 10f;
    private Vector2 moveInput;
    private Rigidbody2D myRigidbody;
    private Animator myAnimator;
    private BoxCollider2D myFeetCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("Move Input: " + moveInput);
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
}
