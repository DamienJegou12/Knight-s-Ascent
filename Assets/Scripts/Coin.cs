using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    public int value = 1;
    Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        SelectAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelectAnimation()
    {
        if(value == 1)
        {
            anim.SetTrigger("Coin1");
        }
        else if (value == 5)
        {
            anim.SetTrigger("Coin2");
        }
        else if (value == 10)
        {
            anim.SetTrigger("Coin3");
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player")
        {
            // Ici, tu peux ajouter du code pour augmenter le score du joueur
            // Par exemple : other.GetComponent<PlayerScore>().AddScore(value);
            Destroy(gameObject); // Détruit la pièce après l'avoir ramassée
        }
    }
}
