using UnityEngine;

public class TreasureLogic : MonoBehaviour
{
    public bool isActive = false;

    // Chamado quando você aperta 'E' para agarrar
    public void AtivarTesouro()
    {
        isActive = true;
        
        // Mantém o ritmo tenso, mas no tom normal e constante
        RhythmManager.Instance.pitchAtual = 1.0f; 
        
        RhythmManager.Instance.StartClock();
    }

    public void DesativarTesouro()
    {
        isActive = false;
        // Para a música e faz as plataformas sumirem!
        if (RhythmManager.Instance != null)
        {
            RhythmManager.Instance.StopClock();
        }
    }

    // Se o tesouro for destruído (caiu na lava, etc), para o relógio
    void OnDestroy()
    {
        if (RhythmManager.Instance != null)
        {
            RhythmManager.Instance.StopClock();
        }
    }

    // A Zona de Vitória (Ponto A) será configurada aqui depois
    void OnTriggerEnter(Collider other)
    {
        if (isActive && other.CompareTag("VictoryZone"))
        {
            GameManager.Instance.VencerJogo();
            
            if (RhythmManager.Instance != null) RhythmManager.Instance.StopClock();
            
            Destroy(gameObject); // O tesouro é "entregue" e some da mão
        }
    }
}