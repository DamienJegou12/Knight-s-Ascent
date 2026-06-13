using UnityEngine;

public class WizardMovement : MonoBehaviour
{
    [Header("Cibles et Limites")]
    public Transform player;
    public Transform leftPlatformEdge;
    public Transform rightPlatformEdge;

    [Header("Déplacement")]
    public float moveSpeed = 3f;
    public float safeDistance = 8f; // Distance que le boss essaie de maintenir

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashCooldown = 4f;
    private float lastDashTime = -100f;
    private bool isDashing = false;
    private Vector3 targetDashPosition;

    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (isDashing)
        {
            PerformDash();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si le joueur est trop proche, le boss fuit
        if (distanceToPlayer < safeDistance)
        {
            // Vérifier si le dash est disponible pour fuir vite
            if (Time.time >= lastDashTime + dashCooldown && distanceToPlayer < safeDistance / 2)
            {
                StartDash();
            }
            else
            {
                MoveAwayFromPlayer();
            }
        }
    }

    void LateUpdate()
    {
        // On vérifie que les bords sont bien assignés dans l'inspecteur
        if (leftPlatformEdge == null || rightPlatformEdge == null) return;

        // S'il dépasse le bord gauche, on le téléporte sur le bord gauche
        if (transform.position.x < leftPlatformEdge.position.x)
        {
            transform.position = new Vector3(leftPlatformEdge.position.x, transform.position.y, transform.position.z);
            isDashing = false; // On stoppe le dash s'il fonçait dans le mur
            Debug.Log("Téléportation d'urgence sur le bord gauche !");
        }
        // S'il dépasse le bord droit, on le téléporte sur le bord droit
        else if (transform.position.x > rightPlatformEdge.position.x)
        {
            transform.position = new Vector3(rightPlatformEdge.position.x, transform.position.y, transform.position.z);
            isDashing = false; // On stoppe le dash
            Debug.Log("Téléportation d'urgence sur le bord droit !");
        }
    }

    private void MoveAwayFromPlayer()
    {
        // Détermine la direction opposée au joueur
        int direction = transform.position.x < player.position.x ? -1 : 1;
        
        Vector3 newPos = transform.position + new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);
        
        // Empêche le boss de sortir de la plateforme
        newPos.x = Mathf.Clamp(newPos.x, leftPlatformEdge.position.x, rightPlatformEdge.position.x);
        transform.position = newPos;

        FlipSprite(direction);
    }

    private void StartDash()
    {
        animator.SetTrigger("Dash");
        isDashing = true;
        lastDashTime = Time.time;
        int direction = transform.position.x < player.position.x ? -1 : 1;

        // Calcule la destination du dash
        float dashDistance = 10f;
        targetDashPosition = transform.position + new Vector3(direction * dashDistance, 0, 0);
        
        // Sécurité pour rester sur la plateforme
        targetDashPosition.x = Mathf.Clamp(targetDashPosition.x, leftPlatformEdge.position.x, rightPlatformEdge.position.x);
    }

    private void PerformDash()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetDashPosition, dashSpeed * Time.deltaTime);
    }

    private void FlipSprite(int direction)
    {
        // Optionnel : tourne le sprite pour regarder le joueur (le boss regarde vers le joueur même en fuyant)
        transform.localScale = new Vector3(-direction, 1, 1); 
    }

    public void EndDash()
    {
        isDashing = false;
    }

    public Transform GetPlayerTransform()
    {
        return player;
    }
}