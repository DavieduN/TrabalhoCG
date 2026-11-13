using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Se o Player ou a Bomba/Cristal tocarem na lava...
        if (other.CompareTag("Player") || other.CompareTag("Interactable"))
        {
            // ...chama a função Morrer do GameManager!
            GameManager.Instance.Morrer();
        }
    }
}