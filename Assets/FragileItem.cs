using UnityEngine;

public class FragileItem : MonoBehaviour
{
    public bool isActive = false;
    
    [Header("Áudio")]
    public AudioClip somExplosao;

    void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            // Se bateu na parede que bloqueia a passagem...
            if (collision.gameObject.CompareTag("FragileWall"))
            {
                if (somExplosao != null) 
                    AudioSource.PlayClipAtPoint(somExplosao, transform.position, 1.0f);

                Destroy(collision.gameObject); 
                GameManager.Instance.AvancarParaFase2(); 
                Destroy(gameObject);
            }
            // Se bateu em qualquer outra coisa (que não seja o chão inicial)...
            else if (collision.gameObject.name != "Plane")
            {
                // Toca o som de falha também!
                if (somExplosao != null) 
                    AudioSource.PlayClipAtPoint(somExplosao, transform.position, 1.0f);

                GameManager.Instance.Morrer();
                Destroy(gameObject); 
            }
        }
    }
}