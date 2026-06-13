using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;

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
    [Header("Paramètres du Dash")]
    public float dashForce = 25f;       // Va loin et vite
    public float dashDuration = 0.2f;   // Temps du dash
    public float dashCooldown = 2f;     // Cooldown plus long
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Paramètres du Roll")]
    public float rollForce = 12f;       // Moins loin que le dash
    public float rollDuration = 0.35f;  // Dure un peu plus longtemps (ou au choix)
    public float rollCooldown = 0.8f;   // Cooldown plus court
    private bool canRoll = true;
    private bool isRolling = false;
    private Vector2 moveInput;
    private Rigidbody2D myRigidbody;
    private Animator myAnimator;
    private BoxCollider2D myFeetCollider;
    private CapsuleCollider2D myBodyCollider;
    private bool isClimbing = false;
    private bool isJumping = false;
    private bool isJumpingDown = false;
    private bool isFacingRight = true;
    
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
        if (isRolling) { return; }
        if (isDashing) { 
            Debug.Log("Currently dashing, skipping movement update.");
            return;
        }
        Run();
        FlipSprite();
        ClimbLadder();
        EnnemiCollision();
        StatusJump();
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
            isFacingRight = myRigidbody.linearVelocity.x > 0;
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocity.x) * 5f, 5f);
        }
    }

    void OnJump(InputValue value)
    {
        if (!value.isPressed) { return; }
        Debug.Log("Jump Input: " + value);
        if (isTouchingTheGround())
        {
            isJumping = true;
            myAnimator.SetTrigger("Jump");
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
        if(GetComponent<Player>().IsInvulnerable())
        {
            return;
        }
        if(GetComponent<Player>().IsInvincible())
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
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].ProcessPlayerDeath();
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

    void StatusJump()
    {
        if(isJumping && myRigidbody.linearVelocity.y < 0.1f)
        {
            isJumping = false;
            isJumpingDown = true;
            myAnimator.SetTrigger("Jump Down");
        }
        if(isJumpingDown && isTouchingTheGround())
        {
            isJumpingDown = false;
            myAnimator.SetTrigger("onGround");
        }
    }

    void OnDash()
    {
        if (isDashing) return; // Empêche de dash si déjà en train de dash
        if (!canDash) return; // Empêche de dash si en cooldown
        myAnimator.SetBool("isDashing",true);
        StartCoroutine(Dash());
    }

    void OnRoll()
    {
        if (isRolling) return; // Empêche de roll si déjà en train de roll
        if (!canRoll) return; // Empêche de roll si en cooldown
        isRolling = true;
        canRoll = false;
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].setCanRoll(false);
        myAnimator.SetBool("isRolling", true);
        GetComponent<Player>().makeInvulnerable(true);
        myRigidbody.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * rollForce, myRigidbody.linearVelocity.y);
    }

    void EndRoll()
    {
        isRolling = false;
        myAnimator.SetBool("isRolling", false);
        StartCoroutine(RollCooldownRoutine());
        GetComponent<Player>().makeInvulnerable(false);
    }

    private void StartRoll()
    {
        canRoll = false;
        isRolling = true;
        // isInvincible = true;

        // On lance l'animation de roulade (assure-toi d'avoir un trigger "Roll" dans ton Animator)
        // anim.SetTrigger("Roll");

        // On applique la force initiale (et on respecte la gravité)
        // rb.velocity = new Vector2(facingDirection * rollForce, rb.velocity.y);
    }


    private IEnumerator RollCooldownRoutine()
    {
        yield return new WaitForSeconds(rollCooldown);
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].setCanRoll(true);
        canRoll = true;
    }

    private IEnumerator Dash()
    {
        // (Le code du dash reste exactement le même que précédemment)
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].setCanDash(false);
        canDash = false;
        isDashing = true;
        GetComponent<Player>().makeInvulnerable(true);
        myRigidbody.gravityScale = 0f; 
        myRigidbody.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * dashForce, 0f);
        yield return new WaitForSeconds(dashDuration);
        myAnimator.SetBool("isDashing",false);
        myRigidbody.gravityScale = 1.5f;
        isDashing = false;
        GetComponent<Player>().makeInvulnerable(false);
        yield return new WaitForSeconds(dashCooldown);
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].setCanDash(true);
        canDash = true;
    }

    void OnInvincible()
    {
        GetComponent<Player>().makeInvincible(!GetComponent<Player>().IsInvincible());
    }
}
