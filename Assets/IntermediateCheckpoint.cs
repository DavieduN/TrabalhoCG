using UnityEngine;

public class IntermediateCheckpoint : MonoBehaviour
{
    [Header("Posições de Respawn - IDA (Fase 2 - Cristal)")]
    public Transform spawnPlayerFase2;
    public Transform pedestalCristalFase2;

    [Header("Posições de Respawn - VOLTA (Fase 3 - Tesouro)")]
    public Transform spawnPlayerFase3;
    public Transform pedestalTesouroFase3;

    private bool ativadoFase2 = false;
    private bool ativadoFase3 = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Pergunta para o gerente em qual fase estamos
            int fase = GameManager.Instance.faseAtual;

            // Se for a Ida e ainda não ativou a ida...
            if (fase == 2 && !ativadoFase2)
            {
                ativadoFase2 = true;
                GameManager.Instance.AtualizarCheckpointFase2(spawnPlayerFase2, pedestalCristalFase2);
            }
            // Se for a Volta e ainda não ativou a volta...
            else if (fase == 3 && !ativadoFase3)
            {
                ativadoFase3 = true;
                GameManager.Instance.AtualizarCheckpointFase3(spawnPlayerFase3, pedestalTesouroFase3);
            }
        }
    }
}