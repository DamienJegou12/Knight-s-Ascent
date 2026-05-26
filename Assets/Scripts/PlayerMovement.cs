using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    private Vector2 moveInput;
    private Rigidbody2D myRigidbody;
    private Animator myAnimator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
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
        if(moveInput.x != 1 && moveInput.x != -1)
        {
            myRigidbody.linearVelocity = Vector2.zero; // Stop horizontal movement when no input
            myAnimator.SetBool("isRunning", false);
            return;
        }
        myAnimator.SetBool("isRunning", true);
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, 0); // Adjust the speed as needed
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
}
