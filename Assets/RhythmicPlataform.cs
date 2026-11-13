using UnityEngine;

public class RhythmicPlatform : MonoBehaviour
{
    [Header("Configuração de Ritmo")]
    [Tooltip("Marque em quais dos 8 segmentos (0 a 7) esta pedra existe")]
    public bool[] segmentosAtivos = new bool[8]; 

    [Header("Visuais")]
    public Material materialSolido;     // Material normal da pedra
    public Material materialFantasma;   // Material transparente/outline

    private Collider meuCollider;
    private Renderer meuRenderer;

    void Start()
    {
        meuCollider = GetComponent<Collider>();
        meuRenderer = GetComponent<Renderer>();

        // Começa desativada (Fantasma) antes do jogador pegar o cristal
        meuCollider.enabled = false;
        if (meuRenderer != null && materialFantasma != null)
        {
            meuRenderer.material = materialFantasma;
        }
    }

    void Update()
    {
        // Se o relógio ainda não começou, força o estado fantasma e sai do Update
        if (RhythmManager.Instance == null || !RhythmManager.Instance.isClockRunning)
        {
            meuCollider.enabled = false;
            meuRenderer.material = materialFantasma;
            return;
        }

        // Verifica no RhythmManager qual é o segmento atual (0 a 7)
        int segAtual = RhythmManager.Instance.currentSegment;
        
        // Descobre se, neste exato segmento, a plataforma deveria ser sólida
        bool deveEstarAtiva = segmentosAtivos[segAtual];

        // Atualiza a colisão (pode pisar ou cai direto?)
        meuCollider.enabled = deveEstarAtiva;

        // Troca o material de acordo com o estado
        meuRenderer.material = deveEstarAtiva ? materialSolido : materialFantasma; 
    }
}