using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    private Rigidbody2D myRigidbody;
    private bool isFacingRight = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        myRigidbody.linearVelocity = new Vector2(moveSpeed * (isFacingRight ? 1 : -1), 0);
        FlipEnnemyFacing();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        isFacingRight = !isFacingRight;
    }

    void FlipEnnemyFacing()
    {
        transform.localScale = new Vector2((isFacingRight ? -1f : 1f), 1f);
    }
}
