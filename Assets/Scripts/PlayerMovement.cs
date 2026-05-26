using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    private Vector2 moveInput;
    private Rigidbody2D myRigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
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
            return;
        }
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, 0); // Adjust the speed as needed
        myRigidbody.linearVelocity = playerVelocity;
    }
}
